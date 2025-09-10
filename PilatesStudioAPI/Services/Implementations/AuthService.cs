using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PilatesStudioAPI.Configuration;
using PilatesStudioAPI.Data.Context;
using PilatesStudioAPI.Models.DTOs.Auth;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly PilatesStudioDbContext _context;
    private readonly ILogger<AuthService> _logger;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtService jwtService,
        PilatesStudioDbContext context,
        ILogger<AuthService> logger,
        JwtSettings jwtSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _context = context;
        _logger = logger;
        _jwtSettings = jwtSettings;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        try
        {
            _logger.LogInformation("Attempting login for email: {Email}", request.Email);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email {Email}", request.Email);
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed: User account is inactive for {Email}", request.Email);
                throw new UnauthorizedAccessException("Cuenta inactiva");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed: Invalid password for email {Email}", request.Email);
                
                if (result.IsLockedOut)
                    throw new UnauthorizedAccessException("Cuenta bloqueada temporalmente");
                
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            // Load related data
            await LoadUserRelatedDataAsync(user);

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Store refresh token (you might want to store this in a separate table)
            // For now, we'll store it in a simple way
            user.SecurityStamp = refreshToken; // Temporary storage
            await _userManager.UpdateAsync(user);

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = MapToUserInfoDto(user)
            };

            _logger.LogInformation("Login successful for user {UserId}", user.Id);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email {Email}", request.Email);
            throw;
        }
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            _logger.LogInformation("Attempting registration for email: {Email}", request.Email);

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed: Email already exists {Email}", request.Email);
                throw new InvalidOperationException("El email ya está registrado");
            }

            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                Role = request.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("User creation failed for {Email}: {Errors}", request.Email, errors);
                throw new InvalidOperationException($"Error al crear usuario: {errors}");
            }

            // Add to role
            await _userManager.AddToRoleAsync(user, request.Role);

            // Create profile based on role
            await CreateUserProfileAsync(user, request);

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Store refresh token
            user.SecurityStamp = refreshToken;
            await _userManager.UpdateAsync(user);

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = MapToUserInfoDto(user)
            };

            _logger.LogInformation("Registration successful for user {UserId}", user.Id);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email {Email}", request.Email);
            throw;
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        try
        {
            _logger.LogInformation("Attempting to refresh token");

            // Find user by refresh token
            var user = await _context.Users.FirstOrDefaultAsync(u => u.SecurityStamp == request.RefreshToken);
            if (user == null)
            {
                _logger.LogWarning("Refresh token not found");
                throw new UnauthorizedAccessException("Token de actualización inválido");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Refresh token failed: User account is inactive for {UserId}", user.Id);
                throw new UnauthorizedAccessException("Cuenta inactiva");
            }

            // Load related data
            await LoadUserRelatedDataAsync(user);

            // Generate new tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Update refresh token
            user.SecurityStamp = refreshToken;
            await _userManager.UpdateAsync(user);

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = MapToUserInfoDto(user)
            };

            _logger.LogInformation("Token refresh successful for user {UserId}", user.Id);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            throw;
        }
    }

    public async Task<bool> ChangePasswordAsync(long userId, ChangePasswordRequestDto request)
    {
        try
        {
            _logger.LogInformation("Attempting to change password for user {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("Change password failed: User not found {UserId}", userId);
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Change password failed for user {UserId}: {Errors}", userId, errors);
                throw new InvalidOperationException($"Error al cambiar contraseña: {errors}");
            }

            _logger.LogInformation("Password changed successfully for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> LogoutAsync(long userId)
    {
        try
        {
            _logger.LogInformation("Attempting logout for user {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                // Invalidate refresh token
                user.SecurityStamp = Guid.NewGuid().ToString();
                await _userManager.UpdateAsync(user);

                await _signInManager.SignOutAsync();
            }

            _logger.LogInformation("Logout successful for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> VerifyEmailAsync(string token)
    {
        // TODO: Implement email verification
        await Task.CompletedTask;
        throw new NotImplementedException("Email verification not implemented yet");
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        // TODO: Implement forgot password
        await Task.CompletedTask;
        throw new NotImplementedException("Forgot password not implemented yet");
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        // TODO: Implement password reset
        await Task.CompletedTask;
        throw new NotImplementedException("Password reset not implemented yet");
    }

    private async Task LoadUserRelatedDataAsync(User user)
    {
        if (user.Role == "instructor")
        {
            user.Instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == user.Id);
        }
        else if (user.Role == "student")
        {
            user.Student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
        }
    }

    private async Task CreateUserProfileAsync(User user, RegisterRequestDto request)
    {
        if (request.Role == "instructor")
        {
            var instructor = new Instructor
            {
                UserId = user.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Instructors.Add(instructor);
        }
        else if (request.Role == "student")
        {
            var student = new Student
            {
                UserId = user.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                BirthDate = request.BirthDate.HasValue ? DateOnly.FromDateTime(request.BirthDate.Value) : null,
                EmergencyContact = request.EmergencyContact,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Students.Add(student);
        }

        await _context.SaveChangesAsync();
    }

    private static UserInfoDto MapToUserInfoDto(User user)
    {
        var firstName = "";
        var lastName = "";

        if (user.Instructor != null)
        {
            firstName = user.Instructor.FirstName;
            lastName = user.Instructor.LastName;
        }
        else if (user.Student != null)
        {
            firstName = user.Student.FirstName;
            lastName = user.Student.LastName;
        }

        return new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = firstName,
            LastName = lastName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
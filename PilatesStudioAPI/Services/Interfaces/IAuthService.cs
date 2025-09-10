using PilatesStudioAPI.Models.DTOs.Auth;

namespace PilatesStudioAPI.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<bool> ChangePasswordAsync(long userId, ChangePasswordRequestDto request);
    Task<bool> LogoutAsync(long userId);
    Task<bool> VerifyEmailAsync(string token);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(string token, string newPassword);
}
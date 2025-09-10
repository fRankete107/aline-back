using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PilatesStudioAPI.Data.Context;
using PilatesStudioAPI.Models.DTOs.Users;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Services.Implementations;

public class InstructorService : IInstructorService
{
    private readonly PilatesStudioDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<InstructorService> _logger;

    public InstructorService(
        PilatesStudioDbContext context,
        IMapper mapper,
        UserManager<User> userManager,
        ILogger<InstructorService> logger)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IEnumerable<InstructorDto>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Getting all instructors");
            
            var instructors = await _context.Instructors
                .Include(i => i.User)
                .OrderBy(i => i.FirstName)
                .ThenBy(i => i.LastName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<InstructorDto>>(instructors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all instructors");
            throw;
        }
    }

    public async Task<InstructorDto?> GetByIdAsync(long id)
    {
        try
        {
            _logger.LogInformation("Getting instructor by ID: {InstructorId}", id);

            var instructor = await _context.Instructors
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instructor == null)
            {
                _logger.LogWarning("Instructor not found with ID: {InstructorId}", id);
                return null;
            }

            return _mapper.Map<InstructorDto>(instructor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting instructor by ID: {InstructorId}", id);
            throw;
        }
    }

    public async Task<InstructorDto?> GetByUserIdAsync(long userId)
    {
        try
        {
            _logger.LogInformation("Getting instructor by user ID: {UserId}", userId);

            var instructor = await _context.Instructors
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.UserId == userId);

            if (instructor == null)
            {
                _logger.LogWarning("Instructor not found with user ID: {UserId}", userId);
                return null;
            }

            return _mapper.Map<InstructorDto>(instructor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting instructor by user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<InstructorDto> CreateAsync(InstructorCreateDto createDto)
    {
        try
        {
            _logger.LogInformation("Creating new instructor with email: {Email}", createDto.Email);

            var existingUser = await _userManager.FindByEmailAsync(createDto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("User with email {Email} already exists", createDto.Email);
                throw new InvalidOperationException("El email ya está registrado");
            }

            var user = new User
            {
                UserName = createDto.Email,
                Email = createDto.Email,
                Role = "instructor",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, createDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("User creation failed for {Email}: {Errors}", createDto.Email, errors);
                throw new InvalidOperationException($"Error al crear usuario: {errors}");
            }

            await _userManager.AddToRoleAsync(user, "instructor");

            var instructor = _mapper.Map<Instructor>(createDto);
            instructor.UserId = user.Id;
            instructor.CreatedAt = DateTime.UtcNow;
            instructor.UpdatedAt = DateTime.UtcNow;

            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            instructor.User = user;
            _logger.LogInformation("Instructor created successfully with ID: {InstructorId}", instructor.Id);

            return _mapper.Map<InstructorDto>(instructor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating instructor with email: {Email}", createDto.Email);
            throw;
        }
    }

    public async Task<InstructorDto?> UpdateAsync(long id, InstructorUpdateDto updateDto)
    {
        try
        {
            _logger.LogInformation("Updating instructor with ID: {InstructorId}", id);

            var instructor = await _context.Instructors
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instructor == null)
            {
                _logger.LogWarning("Instructor not found with ID: {InstructorId}", id);
                return null;
            }

            _mapper.Map(updateDto, instructor);
            instructor.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Instructor updated successfully with ID: {InstructorId}", id);
            return _mapper.Map<InstructorDto>(instructor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating instructor with ID: {InstructorId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        try
        {
            _logger.LogInformation("Deleting instructor with ID: {InstructorId}", id);

            var instructor = await _context.Instructors
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instructor == null)
            {
                _logger.LogWarning("Instructor not found with ID: {InstructorId}", id);
                return false;
            }

            var hasClasses = await _context.Classes.AnyAsync(c => c.InstructorId == id);
            if (hasClasses)
            {
                _logger.LogWarning("Cannot delete instructor {InstructorId} - has associated classes", id);
                throw new InvalidOperationException("No se puede eliminar el instructor porque tiene clases asociadas");
            }

            _context.Instructors.Remove(instructor);
            
            if (instructor.User != null)
            {
                await _userManager.DeleteAsync(instructor.User);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Instructor deleted successfully with ID: {InstructorId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting instructor with ID: {InstructorId}", id);
            throw;
        }
    }

    public async Task<bool> ActivateAsync(long id)
    {
        try
        {
            _logger.LogInformation("Activating instructor with ID: {InstructorId}", id);

            var instructor = await _context.Instructors
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instructor == null)
            {
                _logger.LogWarning("Instructor not found with ID: {InstructorId}", id);
                return false;
            }

            instructor.IsActive = true;
            instructor.UpdatedAt = DateTime.UtcNow;

            if (instructor.User != null)
            {
                instructor.User.IsActive = true;
                instructor.User.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Instructor activated successfully with ID: {InstructorId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating instructor with ID: {InstructorId}", id);
            throw;
        }
    }

    public async Task<bool> DeactivateAsync(long id)
    {
        try
        {
            _logger.LogInformation("Deactivating instructor with ID: {InstructorId}", id);

            var instructor = await _context.Instructors
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instructor == null)
            {
                _logger.LogWarning("Instructor not found with ID: {InstructorId}", id);
                return false;
            }

            instructor.IsActive = false;
            instructor.UpdatedAt = DateTime.UtcNow;

            if (instructor.User != null)
            {
                instructor.User.IsActive = false;
                instructor.User.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Instructor deactivated successfully with ID: {InstructorId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating instructor with ID: {InstructorId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<InstructorDto>> GetActiveInstructorsAsync()
    {
        try
        {
            _logger.LogInformation("Getting active instructors");

            var instructors = await _context.Instructors
                .Include(i => i.User)
                .Where(i => i.IsActive)
                .OrderBy(i => i.FirstName)
                .ThenBy(i => i.LastName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<InstructorDto>>(instructors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active instructors");
            throw;
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        try
        {
            return await _context.Instructors.AnyAsync(i => i.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if instructor exists with ID: {InstructorId}", id);
            throw;
        }
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PilatesStudioAPI.Data.Context;
using PilatesStudioAPI.Models.DTOs.Users;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly PilatesStudioDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<StudentService> _logger;

    public StudentService(
        PilatesStudioDbContext context,
        IMapper mapper,
        UserManager<User> userManager,
        ILogger<StudentService> logger)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IEnumerable<StudentDto>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Getting all students");

            var students = await _context.Students
                .Include(s => s.User)
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudentDto>>(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all students");
            throw;
        }
    }

    public async Task<StudentDto?> GetByIdAsync(long id)
    {
        try
        {
            _logger.LogInformation("Getting student by ID: {StudentId}", id);

            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                _logger.LogWarning("Student not found with ID: {StudentId}", id);
                return null;
            }

            return _mapper.Map<StudentDto>(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student by ID: {StudentId}", id);
            throw;
        }
    }

    public async Task<StudentDto?> GetByUserIdAsync(long userId)
    {
        try
        {
            _logger.LogInformation("Getting student by user ID: {UserId}", userId);

            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                _logger.LogWarning("Student not found with user ID: {UserId}", userId);
                return null;
            }

            return _mapper.Map<StudentDto>(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student by user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<StudentDto> CreateAsync(StudentCreateDto createDto)
    {
        try
        {
            _logger.LogInformation("Creating new student with email: {Email}", createDto.Email);

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
                Role = "student",
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

            await _userManager.AddToRoleAsync(user, "student");

            var student = _mapper.Map<Student>(createDto);
            student.UserId = user.Id;
            student.CreatedAt = DateTime.UtcNow;
            student.UpdatedAt = DateTime.UtcNow;

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            student.User = user;
            _logger.LogInformation("Student created successfully with ID: {StudentId}", student.Id);

            return _mapper.Map<StudentDto>(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student with email: {Email}", createDto.Email);
            throw;
        }
    }

    public async Task<StudentDto?> UpdateAsync(long id, StudentUpdateDto updateDto)
    {
        try
        {
            _logger.LogInformation("Updating student with ID: {StudentId}", id);

            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                _logger.LogWarning("Student not found with ID: {StudentId}", id);
                return null;
            }

            _mapper.Map(updateDto, student);
            student.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Student updated successfully with ID: {StudentId}", id);
            return _mapper.Map<StudentDto>(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student with ID: {StudentId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        try
        {
            _logger.LogInformation("Deleting student with ID: {StudentId}", id);

            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                _logger.LogWarning("Student not found with ID: {StudentId}", id);
                return false;
            }

            // TODO: Implementar verificaciones de integridad cuando se implementen las otras entidades
            // var hasEnrollments = await _context.Enrollments.AnyAsync(e => e.StudentId == id);
            // var hasPurchases = await _context.Purchases.AnyAsync(p => p.StudentId == id);
            // var hasAttendance = await _context.Attendances.AnyAsync(a => a.StudentId == id);

            _context.Students.Remove(student);

            if (student.User != null)
            {
                await _userManager.DeleteAsync(student.User);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Student deleted successfully with ID: {StudentId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student with ID: {StudentId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<StudentDto>> SearchAsync(string searchTerm)
    {
        try
        {
            _logger.LogInformation("Searching students with term: {SearchTerm}", searchTerm);

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync();
            }

            var students = await _context.Students
                .Include(s => s.User)
                .Where(s => 
                    s.FirstName.Contains(searchTerm) ||
                    s.LastName.Contains(searchTerm) ||
                    (s.User != null && s.User.Email != null && s.User.Email.Contains(searchTerm)) ||
                    (s.Phone != null && s.Phone.Contains(searchTerm)))
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudentDto>>(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching students with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        try
        {
            return await _context.Students.AnyAsync(s => s.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if student exists with ID: {StudentId}", id);
            throw;
        }
    }
}
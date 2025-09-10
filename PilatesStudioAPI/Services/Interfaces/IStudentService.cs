using PilatesStudioAPI.Models.DTOs.Users;

namespace PilatesStudioAPI.Services.Interfaces;

public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetAllAsync();
    Task<StudentDto?> GetByIdAsync(long id);
    Task<StudentDto?> GetByUserIdAsync(long userId);
    Task<StudentDto> CreateAsync(StudentCreateDto createDto);
    Task<StudentDto?> UpdateAsync(long id, StudentUpdateDto updateDto);
    Task<bool> DeleteAsync(long id);
    Task<IEnumerable<StudentDto>> SearchAsync(string searchTerm);
    Task<bool> ExistsAsync(long id);
}
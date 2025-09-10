using PilatesStudioAPI.Models.DTOs.Users;

namespace PilatesStudioAPI.Services.Interfaces;

public interface IInstructorService
{
    Task<IEnumerable<InstructorDto>> GetAllAsync();
    Task<InstructorDto?> GetByIdAsync(long id);
    Task<InstructorDto?> GetByUserIdAsync(long userId);
    Task<InstructorDto> CreateAsync(InstructorCreateDto createDto);
    Task<InstructorDto?> UpdateAsync(long id, InstructorUpdateDto updateDto);
    Task<bool> DeleteAsync(long id);
    Task<bool> ActivateAsync(long id);
    Task<bool> DeactivateAsync(long id);
    Task<IEnumerable<InstructorDto>> GetActiveInstructorsAsync();
    Task<bool> ExistsAsync(long id);
}
using PilatesStudioAPI.Models.Entities;

namespace PilatesStudioAPI.Repositories.Interfaces;

public interface IInstructorRepository
{
    Task<IEnumerable<Instructor>> GetAllAsync();
    Task<Instructor?> GetByIdAsync(long id);
    Task<Instructor> CreateAsync(Instructor instructor);
    Task<Instructor?> UpdateAsync(long id, Instructor instructor);
    Task<bool> DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
    Task<Instructor?> GetByUserIdAsync(long userId);
    Task<IEnumerable<Instructor>> GetActiveInstructorsAsync();
}
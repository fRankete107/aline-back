using PilatesStudioAPI.Models.Entities;

namespace PilatesStudioAPI.Repositories.Interfaces;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(long id);
    Task<Student> CreateAsync(Student student);
    Task<Student?> UpdateAsync(long id, Student student);
    Task<bool> DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
    Task<Student?> GetByUserIdAsync(long userId);
    Task<IEnumerable<Student>> SearchAsync(string searchTerm);
}
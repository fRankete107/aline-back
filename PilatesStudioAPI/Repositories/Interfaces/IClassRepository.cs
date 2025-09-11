using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.Entities;

namespace PilatesStudioAPI.Repositories.Interfaces;

public interface IClassRepository
{
    Task<IEnumerable<Class>> GetAllAsync();
    Task<Class?> GetByIdAsync(long id);
    Task<Class> CreateAsync(Class classEntity);
    Task<Class?> UpdateAsync(long id, Class classEntity);
    Task<bool> DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
    Task<IEnumerable<Class>> GetFilteredAsync(ClassFilterDto filter);
    Task<IEnumerable<Class>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task<IEnumerable<Class>> GetByInstructorAsync(long instructorId);
    Task<IEnumerable<Class>> GetByZoneAsync(long zoneId);
    Task<IEnumerable<Class>> GetAvailableClassesAsync();
    Task<bool> HasScheduleConflictAsync(long instructorId, DateOnly date, TimeOnly startTime, TimeOnly endTime, long? excludeClassId = null);
    Task<bool> HasZoneConflictAsync(long zoneId, DateOnly date, TimeOnly startTime, TimeOnly endTime, long? excludeClassId = null);
    Task<int> GetReservedSpotsCountAsync(long classId);
}
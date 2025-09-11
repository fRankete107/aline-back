using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Services.Interfaces;

public interface IClassService
{
    Task<IEnumerable<ClassDto>> GetAllClassesAsync();
    Task<ClassDto?> GetClassByIdAsync(long id);
    Task<ClassDto> CreateClassAsync(CreateClassDto createClassDto);
    Task<ClassDto?> UpdateClassAsync(long id, UpdateClassDto updateClassDto);
    Task<bool> DeleteClassAsync(long id);
    Task<IEnumerable<ClassDto>> GetFilteredClassesAsync(ClassFilterDto filter);
    Task<IEnumerable<ClassDto>> GetClassesByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task<IEnumerable<ClassDto>> GetClassesByInstructorAsync(long instructorId);
    Task<IEnumerable<ClassDto>> GetClassesByZoneAsync(long zoneId);
    Task<IEnumerable<ClassDto>> GetAvailableClassesAsync();
    Task<bool> ClassExistsAsync(long id);
    Task<bool> HasScheduleConflictAsync(long instructorId, DateOnly date, TimeOnly startTime, TimeOnly endTime, long? excludeClassId = null);
    Task<bool> HasZoneConflictAsync(long zoneId, DateOnly date, TimeOnly startTime, TimeOnly endTime, long? excludeClassId = null);
}
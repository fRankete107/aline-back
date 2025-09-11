using Microsoft.EntityFrameworkCore;
using PilatesStudioAPI.Data.Context;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Repositories.Interfaces;

namespace PilatesStudioAPI.Repositories.Implementations;

public class ClassRepository : IClassRepository
{
    private readonly PilatesStudioDbContext _context;

    public ClassRepository(PilatesStudioDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Class>> GetAllAsync()
    {
        return await _context.Classes
            .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
            .Include(c => c.Zone)
            .Include(c => c.Reservations)
            .OrderBy(c => c.ClassDate)
            .ThenBy(c => c.StartTime)
            .ToListAsync();
    }

    public async Task<Class?> GetByIdAsync(long id)
    {
        return await _context.Classes
            .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
            .Include(c => c.Zone)
            .Include(c => c.Reservations)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Class> CreateAsync(Class classEntity)
    {
        classEntity.CreatedAt = DateTime.UtcNow;
        classEntity.UpdatedAt = DateTime.UtcNow;
        
        _context.Classes.Add(classEntity);
        await _context.SaveChangesAsync();
        
        return await GetByIdAsync(classEntity.Id) ?? classEntity;
    }

    public async Task<Class?> UpdateAsync(long id, Class classEntity)
    {
        var existingClass = await _context.Classes.FindAsync(id);
        if (existingClass == null)
            return null;

        existingClass.InstructorId = classEntity.InstructorId;
        existingClass.ZoneId = classEntity.ZoneId;
        existingClass.ClassDate = classEntity.ClassDate;
        existingClass.StartTime = classEntity.StartTime;
        existingClass.EndTime = classEntity.EndTime;
        existingClass.CapacityLimit = classEntity.CapacityLimit;
        existingClass.ClassType = classEntity.ClassType;
        existingClass.DifficultyLevel = classEntity.DifficultyLevel;
        existingClass.Description = classEntity.Description;
        existingClass.Status = classEntity.Status;
        existingClass.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var classEntity = await _context.Classes.FindAsync(id);
        if (classEntity == null)
            return false;

        var hasReservations = await _context.Reservations
            .AnyAsync(r => r.ClassId == id && r.Status == "confirmed");

        if (hasReservations)
        {
            classEntity.Status = "cancelled";
            classEntity.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _context.Classes.Remove(classEntity);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Classes.AnyAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Class>> GetFilteredAsync(ClassFilterDto filter)
    {
        var query = _context.Classes
            .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
            .Include(c => c.Zone)
            .Include(c => c.Reservations)
            .AsQueryable();

        if (filter.StartDate.HasValue)
            query = query.Where(c => c.ClassDate >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(c => c.ClassDate <= filter.EndDate.Value);

        if (filter.InstructorId.HasValue)
            query = query.Where(c => c.InstructorId == filter.InstructorId.Value);

        if (filter.ZoneId.HasValue)
            query = query.Where(c => c.ZoneId == filter.ZoneId.Value);

        if (!string.IsNullOrEmpty(filter.DifficultyLevel))
            query = query.Where(c => c.DifficultyLevel == filter.DifficultyLevel);

        if (!string.IsNullOrEmpty(filter.Status))
            query = query.Where(c => c.Status == filter.Status);

        if (filter.OnlyAvailable == true)
        {
            query = query.Where(c => c.Status == "scheduled" && 
                c.Reservations.Count(r => r.Status == "confirmed") < c.CapacityLimit);
        }

        return await query
            .OrderBy(c => c.ClassDate)
            .ThenBy(c => c.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Class>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await _context.Classes
            .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
            .Include(c => c.Zone)
            .Include(c => c.Reservations)
            .Where(c => c.ClassDate >= startDate && c.ClassDate <= endDate)
            .OrderBy(c => c.ClassDate)
            .ThenBy(c => c.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Class>> GetByInstructorAsync(long instructorId)
    {
        return await _context.Classes
            .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
            .Include(c => c.Zone)
            .Include(c => c.Reservations)
            .Where(c => c.InstructorId == instructorId)
            .OrderBy(c => c.ClassDate)
            .ThenBy(c => c.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Class>> GetByZoneAsync(long zoneId)
    {
        return await _context.Classes
            .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
            .Include(c => c.Zone)
            .Include(c => c.Reservations)
            .Where(c => c.ZoneId == zoneId)
            .OrderBy(c => c.ClassDate)
            .ThenBy(c => c.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Class>> GetAvailableClassesAsync()
    {
        return await _context.Classes
            .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
            .Include(c => c.Zone)
            .Include(c => c.Reservations)
            .Where(c => c.Status == "scheduled" && 
                c.ClassDate >= DateOnly.FromDateTime(DateTime.Today) &&
                c.Reservations.Count(r => r.Status == "confirmed") < c.CapacityLimit)
            .OrderBy(c => c.ClassDate)
            .ThenBy(c => c.StartTime)
            .ToListAsync();
    }

    public async Task<bool> HasScheduleConflictAsync(long instructorId, DateOnly date, TimeOnly startTime, TimeOnly endTime, long? excludeClassId = null)
    {
        var query = _context.Classes
            .Where(c => c.InstructorId == instructorId && 
                       c.ClassDate == date && 
                       c.Status != "cancelled");

        if (excludeClassId.HasValue)
            query = query.Where(c => c.Id != excludeClassId.Value);

        return await query.AnyAsync(c => 
            (startTime >= c.StartTime && startTime < c.EndTime) ||
            (endTime > c.StartTime && endTime <= c.EndTime) ||
            (startTime <= c.StartTime && endTime >= c.EndTime));
    }

    public async Task<bool> HasZoneConflictAsync(long zoneId, DateOnly date, TimeOnly startTime, TimeOnly endTime, long? excludeClassId = null)
    {
        var query = _context.Classes
            .Where(c => c.ZoneId == zoneId && 
                       c.ClassDate == date && 
                       c.Status != "cancelled");

        if (excludeClassId.HasValue)
            query = query.Where(c => c.Id != excludeClassId.Value);

        return await query.AnyAsync(c => 
            (startTime >= c.StartTime && startTime < c.EndTime) ||
            (endTime > c.StartTime && endTime <= c.EndTime) ||
            (startTime <= c.StartTime && endTime >= c.EndTime));
    }

    public async Task<int> GetReservedSpotsCountAsync(long classId)
    {
        return await _context.Reservations
            .CountAsync(r => r.ClassId == classId && r.Status == "confirmed");
    }
}
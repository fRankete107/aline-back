using Microsoft.EntityFrameworkCore;
using PilatesStudioAPI.Data.Context;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Repositories.Interfaces;

namespace PilatesStudioAPI.Repositories.Implementations;

public class InstructorRepository : IInstructorRepository
{
    private readonly PilatesStudioDbContext _context;

    public InstructorRepository(PilatesStudioDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Instructor>> GetAllAsync()
    {
        return await _context.Instructors
            .Include(i => i.User)
            .OrderBy(i => i.FirstName)
            .ThenBy(i => i.LastName)
            .ToListAsync();
    }

    public async Task<Instructor?> GetByIdAsync(long id)
    {
        return await _context.Instructors
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Instructor> CreateAsync(Instructor instructor)
    {
        instructor.CreatedAt = DateTime.UtcNow;
        instructor.UpdatedAt = DateTime.UtcNow;
        
        _context.Instructors.Add(instructor);
        await _context.SaveChangesAsync();
        return instructor;
    }

    public async Task<Instructor?> UpdateAsync(long id, Instructor instructor)
    {
        var existingInstructor = await _context.Instructors.FindAsync(id);
        if (existingInstructor == null)
            return null;

        existingInstructor.FirstName = instructor.FirstName;
        existingInstructor.LastName = instructor.LastName;
        existingInstructor.Phone = instructor.Phone;
        existingInstructor.Specializations = instructor.Specializations;
        existingInstructor.Bio = instructor.Bio;
        existingInstructor.IsActive = instructor.IsActive;
        existingInstructor.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingInstructor;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var instructor = await _context.Instructors.FindAsync(id);
        if (instructor == null)
            return false;

        var hasClasses = await _context.Classes
            .AnyAsync(c => c.InstructorId == id);

        if (hasClasses)
        {
            instructor.IsActive = false;
            instructor.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _context.Instructors.Remove(instructor);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Instructors.AnyAsync(i => i.Id == id);
    }

    public async Task<Instructor?> GetByUserIdAsync(long userId)
    {
        return await _context.Instructors
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.UserId == userId);
    }

    public async Task<IEnumerable<Instructor>> GetActiveInstructorsAsync()
    {
        return await _context.Instructors
            .Include(i => i.User)
            .Where(i => i.IsActive)
            .OrderBy(i => i.FirstName)
            .ThenBy(i => i.LastName)
            .ToListAsync();
    }
}
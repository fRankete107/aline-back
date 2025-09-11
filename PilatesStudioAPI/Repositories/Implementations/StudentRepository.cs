using Microsoft.EntityFrameworkCore;
using PilatesStudioAPI.Data.Context;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Repositories.Interfaces;

namespace PilatesStudioAPI.Repositories.Implementations;

public class StudentRepository : IStudentRepository
{
    private readonly PilatesStudioDbContext _context;

    public StudentRepository(PilatesStudioDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students
            .Include(s => s.User)
            .OrderBy(s => s.FirstName)
            .ThenBy(s => s.LastName)
            .ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(long id)
    {
        return await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student> CreateAsync(Student student)
    {
        student.CreatedAt = DateTime.UtcNow;
        student.UpdatedAt = DateTime.UtcNow;
        
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student?> UpdateAsync(long id, Student student)
    {
        var existingStudent = await _context.Students.FindAsync(id);
        if (existingStudent == null)
            return null;

        existingStudent.FirstName = student.FirstName;
        existingStudent.LastName = student.LastName;
        existingStudent.Phone = student.Phone;
        existingStudent.BirthDate = student.BirthDate;
        existingStudent.EmergencyContact = student.EmergencyContact;
        existingStudent.MedicalNotes = student.MedicalNotes;
        existingStudent.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingStudent;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return false;

        var hasSubscriptions = await _context.Subscriptions
            .AnyAsync(s => s.StudentId == id);

        if (hasSubscriptions)
        {
            // Soft delete por tener dependencias
            student.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _context.Students.Remove(student);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Students.AnyAsync(s => s.Id == id);
    }

    public async Task<Student?> GetByUserIdAsync(long userId)
    {
        return await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<IEnumerable<Student>> SearchAsync(string searchTerm)
    {
        return await _context.Students
            .Include(s => s.User)
            .Where(s => s.FirstName.Contains(searchTerm) || 
                       s.LastName.Contains(searchTerm) ||
                       (s.User != null && s.User.Email != null && s.User.Email.Contains(searchTerm)))
            .OrderBy(s => s.FirstName)
            .ThenBy(s => s.LastName)
            .ToListAsync();
    }
}
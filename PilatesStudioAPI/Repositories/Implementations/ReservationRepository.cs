using Microsoft.EntityFrameworkCore;
using PilatesStudioAPI.Data.Context;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Repositories.Interfaces;

namespace PilatesStudioAPI.Repositories.Implementations;

public class ReservationRepository : IReservationRepository
{
    private readonly PilatesStudioDbContext _context;

    public ReservationRepository(PilatesStudioDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Reservation>> GetAllAsync()
    {
        return await _context.Reservations
            .Include(r => r.Student)
                .ThenInclude(s => s.User)
            .Include(r => r.Class)
                .ThenInclude(c => c.Instructor)
                    .ThenInclude(i => i.User)
            .Include(r => r.Class.Zone)
            .Include(r => r.Subscription)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Reservation?> GetByIdAsync(long id)
    {
        return await _context.Reservations
            .Include(r => r.Student)
                .ThenInclude(s => s.User)
            .Include(r => r.Class)
                .ThenInclude(c => c.Instructor)
                    .ThenInclude(i => i.User)
            .Include(r => r.Class.Zone)
            .Include(r => r.Subscription)
                .ThenInclude(s => s.Plan)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Reservation> CreateAsync(Reservation reservation)
    {
        reservation.CreatedAt = DateTime.UtcNow;
        reservation.UpdatedAt = DateTime.UtcNow;
        reservation.ReservationDate = DateTime.UtcNow;
        
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
        
        return await GetByIdAsync(reservation.Id) ?? reservation;
    }

    public async Task<Reservation?> UpdateAsync(long id, Reservation reservation)
    {
        var existingReservation = await _context.Reservations.FindAsync(id);
        if (existingReservation == null)
            return null;

        existingReservation.Status = reservation.Status;
        existingReservation.CancellationReason = reservation.CancellationReason;
        existingReservation.CancelledAt = reservation.CancelledAt;
        existingReservation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
            return false;

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Reservations.AnyAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Reservation>> GetFilteredAsync(ReservationFilterDto filter)
    {
        var query = _context.Reservations
            .Include(r => r.Student)
                .ThenInclude(s => s.User)
            .Include(r => r.Class)
                .ThenInclude(c => c.Instructor)
                    .ThenInclude(i => i.User)
            .Include(r => r.Class.Zone)
            .Include(r => r.Subscription)
            .AsQueryable();

        if (filter.ClassId.HasValue)
            query = query.Where(r => r.ClassId == filter.ClassId.Value);

        if (filter.StudentId.HasValue)
            query = query.Where(r => r.StudentId == filter.StudentId.Value);

        if (filter.InstructorId.HasValue)
            query = query.Where(r => r.Class.InstructorId == filter.InstructorId.Value);

        if (filter.ZoneId.HasValue)
            query = query.Where(r => r.Class.ZoneId == filter.ZoneId.Value);

        if (filter.StartDate.HasValue)
            query = query.Where(r => r.Class.ClassDate >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(r => r.Class.ClassDate <= filter.EndDate.Value);

        if (!string.IsNullOrEmpty(filter.Status))
            query = query.Where(r => r.Status == filter.Status);

        if (filter.UpcomingOnly == true)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var now = TimeOnly.FromDateTime(DateTime.Now);
            query = query.Where(r => r.Status == "confirmed" && 
                (r.Class.ClassDate > today || 
                 (r.Class.ClassDate == today && r.Class.StartTime > now)));
        }

        return await query
            .OrderByDescending(r => r.Class.ClassDate)
            .ThenByDescending(r => r.Class.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByStudentAsync(long studentId)
    {
        return await _context.Reservations
            .Include(r => r.Class)
                .ThenInclude(c => c.Instructor)
                    .ThenInclude(i => i.User)
            .Include(r => r.Class.Zone)
            .Include(r => r.Subscription)
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.Class.ClassDate)
            .ThenByDescending(r => r.Class.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByClassAsync(long classId)
    {
        return await _context.Reservations
            .Include(r => r.Student)
                .ThenInclude(s => s.User)
            .Include(r => r.Subscription)
            .Where(r => r.ClassId == classId)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByInstructorAsync(long instructorId)
    {
        return await _context.Reservations
            .Include(r => r.Student)
                .ThenInclude(s => s.User)
            .Include(r => r.Class)
                .ThenInclude(c => c.Zone)
            .Include(r => r.Subscription)
            .Where(r => r.Class.InstructorId == instructorId)
            .OrderByDescending(r => r.Class.ClassDate)
            .ThenByDescending(r => r.Class.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetUpcomingByStudentAsync(long studentId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var now = TimeOnly.FromDateTime(DateTime.Now);

        return await _context.Reservations
            .Include(r => r.Class)
                .ThenInclude(c => c.Instructor)
                    .ThenInclude(i => i.User)
            .Include(r => r.Class.Zone)
            .Where(r => r.StudentId == studentId && 
                       r.Status == "confirmed" &&
                       (r.Class.ClassDate > today || 
                        (r.Class.ClassDate == today && r.Class.StartTime > now)))
            .OrderBy(r => r.Class.ClassDate)
            .ThenBy(r => r.Class.StartTime)
            .ToListAsync();
    }

    public async Task<bool> HasReservationForClassAsync(long studentId, long classId)
    {
        return await _context.Reservations
            .AnyAsync(r => r.StudentId == studentId && 
                          r.ClassId == classId && 
                          r.Status != "cancelled");
    }

    public async Task<int> GetActiveReservationsCountForClassAsync(long classId)
    {
        return await _context.Reservations
            .CountAsync(r => r.ClassId == classId && r.Status == "confirmed");
    }

    public async Task<bool> CanCancelReservationAsync(long reservationId, int hoursBeforeClass = 2)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Class)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null || reservation.Status != "confirmed")
            return false;

        var classDateTime = reservation.Class.ClassDate.ToDateTime(reservation.Class.StartTime);
        var cancellationDeadline = classDateTime.AddHours(-hoursBeforeClass);

        return DateTime.Now < cancellationDeadline;
    }

    public async Task<IEnumerable<Reservation>> GetReservationsToCompleteAsync()
    {
        var yesterday = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        
        return await _context.Reservations
            .Include(r => r.Class)
            .Where(r => r.Status == "confirmed" && r.Class.ClassDate < yesterday)
            .ToListAsync();
    }

    public async Task<bool> CancelReservationAsync(long id, string? reason = null)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null || reservation.Status != "confirmed")
            return false;

        reservation.Status = "cancelled";
        reservation.CancellationReason = reason;
        reservation.CancelledAt = DateTime.UtcNow;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompleteReservationAsync(long id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null || reservation.Status != "confirmed")
            return false;

        reservation.Status = "completed";
        reservation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAsNoShowAsync(long id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null || reservation.Status != "confirmed")
            return false;

        reservation.Status = "no_show";
        reservation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}
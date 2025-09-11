using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.Entities;

namespace PilatesStudioAPI.Repositories.Interfaces;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetAllAsync();
    Task<Reservation?> GetByIdAsync(long id);
    Task<Reservation> CreateAsync(Reservation reservation);
    Task<Reservation?> UpdateAsync(long id, Reservation reservation);
    Task<bool> DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
    Task<IEnumerable<Reservation>> GetFilteredAsync(ReservationFilterDto filter);
    Task<IEnumerable<Reservation>> GetByStudentAsync(long studentId);
    Task<IEnumerable<Reservation>> GetByClassAsync(long classId);
    Task<IEnumerable<Reservation>> GetByInstructorAsync(long instructorId);
    Task<IEnumerable<Reservation>> GetUpcomingByStudentAsync(long studentId);
    Task<bool> HasReservationForClassAsync(long studentId, long classId);
    Task<int> GetActiveReservationsCountForClassAsync(long classId);
    Task<bool> CanCancelReservationAsync(long reservationId, int hoursBeforeClass = 2);
    Task<IEnumerable<Reservation>> GetReservationsToCompleteAsync();
    Task<bool> CancelReservationAsync(long id, string? reason = null);
    Task<bool> CompleteReservationAsync(long id);
    Task<bool> MarkAsNoShowAsync(long id);
}
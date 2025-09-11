using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Services.Interfaces;

public interface IReservationService
{
    Task<IEnumerable<ReservationDto>> GetAllReservationsAsync();
    Task<ReservationDto?> GetReservationByIdAsync(long id);
    Task<ReservationDto> CreateReservationAsync(CreateReservationDto createReservationDto);
    Task<ReservationDto?> UpdateReservationAsync(long id, UpdateReservationDto updateReservationDto);
    Task<bool> DeleteReservationAsync(long id);
    Task<IEnumerable<ReservationDto>> GetFilteredReservationsAsync(ReservationFilterDto filter);
    Task<IEnumerable<ReservationDto>> GetReservationsByStudentAsync(long studentId);
    Task<IEnumerable<ReservationDto>> GetReservationsByClassAsync(long classId);
    Task<IEnumerable<ReservationDto>> GetReservationsByInstructorAsync(long instructorId);
    Task<IEnumerable<ReservationDto>> GetUpcomingReservationsByStudentAsync(long studentId);
    Task<ReservationDto> CancelReservationAsync(long id, CancelReservationDto cancelReservationDto);
    Task<bool> CompleteReservationAsync(long id);
    Task<bool> MarkAsNoShowAsync(long id);
    Task<bool> ReservationExistsAsync(long id);
    Task<bool> CanStudentReserveClassAsync(long studentId, long classId);
    Task<bool> CanCancelReservationAsync(long reservationId);
    Task<int> ProcessCompletedReservationsAsync();
}
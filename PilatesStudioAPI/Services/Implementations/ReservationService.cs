using AutoMapper;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Repositories.Interfaces;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Services.Implementations;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IClassRepository _classRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IMapper _mapper;

    public ReservationService(
        IReservationRepository reservationRepository,
        IClassRepository classRepository,
        IStudentRepository studentRepository,
        ISubscriptionService subscriptionService,
        IMapper mapper)
    {
        _reservationRepository = reservationRepository;
        _classRepository = classRepository;
        _studentRepository = studentRepository;
        _subscriptionService = subscriptionService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReservationDto>> GetAllReservationsAsync()
    {
        var reservations = await _reservationRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
    }

    public async Task<ReservationDto?> GetReservationByIdAsync(long id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);
        return reservation == null ? null : _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<ReservationDto> CreateReservationAsync(CreateReservationDto createReservationDto)
    {
        await ValidateReservationCreationAsync(createReservationDto);

        // Obtener suscripción activa del estudiante
        var activeSubscription = await _subscriptionService.GetActiveSubscriptionByStudentAsync(createReservationDto.StudentId);
        if (activeSubscription == null)
            throw new InvalidOperationException("Student does not have an active subscription.");

        var reservation = _mapper.Map<Reservation>(createReservationDto);
        reservation.SubscriptionId = activeSubscription.Id;

        var createdReservation = await _reservationRepository.CreateAsync(reservation);

        // Decrementar clases de la suscripción
        await _subscriptionService.UseClassFromSubscriptionAsync(createReservationDto.StudentId);

        return _mapper.Map<ReservationDto>(createdReservation);
    }

    public async Task<ReservationDto?> UpdateReservationAsync(long id, UpdateReservationDto updateReservationDto)
    {
        var existingReservation = await _reservationRepository.GetByIdAsync(id);
        if (existingReservation == null)
            return null;

        _mapper.Map(updateReservationDto, existingReservation);
        var updatedReservation = await _reservationRepository.UpdateAsync(id, existingReservation);
        return updatedReservation == null ? null : _mapper.Map<ReservationDto>(updatedReservation);
    }

    public async Task<bool> DeleteReservationAsync(long id)
    {
        if (!await _reservationRepository.ExistsAsync(id))
            return false;

        return await _reservationRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ReservationDto>> GetFilteredReservationsAsync(ReservationFilterDto filter)
    {
        var reservations = await _reservationRepository.GetFilteredAsync(filter);
        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
    }

    public async Task<IEnumerable<ReservationDto>> GetReservationsByStudentAsync(long studentId)
    {
        if (!await _studentRepository.ExistsAsync(studentId))
            throw new ArgumentException($"Student with ID {studentId} not found.");

        var reservations = await _reservationRepository.GetByStudentAsync(studentId);
        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
    }

    public async Task<IEnumerable<ReservationDto>> GetReservationsByClassAsync(long classId)
    {
        if (!await _classRepository.ExistsAsync(classId))
            throw new ArgumentException($"Class with ID {classId} not found.");

        var reservations = await _reservationRepository.GetByClassAsync(classId);
        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
    }

    public async Task<IEnumerable<ReservationDto>> GetReservationsByInstructorAsync(long instructorId)
    {
        var reservations = await _reservationRepository.GetByInstructorAsync(instructorId);
        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
    }

    public async Task<IEnumerable<ReservationDto>> GetUpcomingReservationsByStudentAsync(long studentId)
    {
        if (!await _studentRepository.ExistsAsync(studentId))
            throw new ArgumentException($"Student with ID {studentId} not found.");

        var reservations = await _reservationRepository.GetUpcomingByStudentAsync(studentId);
        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
    }

    public async Task<ReservationDto> CancelReservationAsync(long id, CancelReservationDto cancelReservationDto)
    {
        var existingReservation = await _reservationRepository.GetByIdAsync(id);
        if (existingReservation == null)
            throw new ArgumentException($"Reservation with ID {id} not found.");

        if (existingReservation.Status != "confirmed")
            throw new InvalidOperationException("Only confirmed reservations can be cancelled.");

        if (!await _reservationRepository.CanCancelReservationAsync(id))
            throw new InvalidOperationException("Reservation cannot be cancelled. Cancellation deadline has passed (2 hours before class).");

        var result = await _reservationRepository.CancelReservationAsync(id, cancelReservationDto.CancellationReason);
        if (!result)
            throw new InvalidOperationException("Failed to cancel reservation.");

        // Devolver la clase a la suscripción si la cancelación es exitosa
        // Esto requiere obtener la suscripción y incrementar las clases restantes
        var cancelledReservation = await _reservationRepository.GetByIdAsync(id);
        return _mapper.Map<ReservationDto>(cancelledReservation);
    }

    public async Task<bool> CompleteReservationAsync(long id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);
        if (reservation == null || reservation.Status != "confirmed")
            return false;

        return await _reservationRepository.CompleteReservationAsync(id);
    }

    public async Task<bool> MarkAsNoShowAsync(long id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);
        if (reservation == null || reservation.Status != "confirmed")
            return false;

        return await _reservationRepository.MarkAsNoShowAsync(id);
    }

    public async Task<bool> ReservationExistsAsync(long id)
    {
        return await _reservationRepository.ExistsAsync(id);
    }

    public async Task<bool> CanStudentReserveClassAsync(long studentId, long classId)
    {
        // Verificar si el estudiante existe
        if (!await _studentRepository.ExistsAsync(studentId))
            return false;

        // Verificar si la clase existe
        if (!await _classRepository.ExistsAsync(classId))
            return false;

        // Verificar si el estudiante tiene suscripción activa con clases restantes
        if (!await _subscriptionService.CanStudentReserveClassAsync(studentId))
            return false;

        // Verificar si ya tiene una reserva para esta clase
        if (await _reservationRepository.HasReservationForClassAsync(studentId, classId))
            return false;

        // Verificar si hay capacidad disponible
        var classEntity = await _classRepository.GetByIdAsync(classId);
        if (classEntity == null)
            return false;

        var reservedCount = await _reservationRepository.GetActiveReservationsCountForClassAsync(classId);
        if (reservedCount >= classEntity.CapacityLimit)
            return false;

        // Verificar que la clase no sea en el pasado
        var today = DateOnly.FromDateTime(DateTime.Today);
        var now = TimeOnly.FromDateTime(DateTime.Now);
        
        if (classEntity.ClassDate < today || 
            (classEntity.ClassDate == today && classEntity.StartTime <= now))
            return false;

        return true;
    }

    public async Task<bool> CanCancelReservationAsync(long reservationId)
    {
        return await _reservationRepository.CanCancelReservationAsync(reservationId);
    }

    public async Task<int> ProcessCompletedReservationsAsync()
    {
        var reservationsToComplete = await _reservationRepository.GetReservationsToCompleteAsync();
        int processedCount = 0;

        foreach (var reservation in reservationsToComplete)
        {
            await _reservationRepository.CompleteReservationAsync(reservation.Id);
            processedCount++;
        }

        return processedCount;
    }

    private async Task ValidateReservationCreationAsync(CreateReservationDto createReservationDto)
    {
        if (!await _studentRepository.ExistsAsync(createReservationDto.StudentId))
            throw new ArgumentException($"Student with ID {createReservationDto.StudentId} not found.");

        if (!await _classRepository.ExistsAsync(createReservationDto.ClassId))
            throw new ArgumentException($"Class with ID {createReservationDto.ClassId} not found.");

        if (!await CanStudentReserveClassAsync(createReservationDto.StudentId, createReservationDto.ClassId))
            throw new InvalidOperationException("Student cannot reserve this class. Check subscription status, class capacity, and timing restrictions.");
    }
}
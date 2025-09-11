using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly ILogger<ReservationsController> _logger;

    public ReservationsController(IReservationService reservationService, ILogger<ReservationsController> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all reservations with optional filtering
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations([FromQuery] ReservationFilterDto? filter = null)
    {
        try
        {
            IEnumerable<ReservationDto> reservations;
            
            if (filter != null && (filter.ClassId.HasValue || filter.StudentId.HasValue || 
                filter.InstructorId.HasValue || filter.ZoneId.HasValue ||
                filter.StartDate.HasValue || filter.EndDate.HasValue || 
                !string.IsNullOrEmpty(filter.Status) || filter.UpcomingOnly.HasValue))
            {
                reservations = await _reservationService.GetFilteredReservationsAsync(filter);
            }
            else
            {
                reservations = await _reservationService.GetAllReservationsAsync();
            }

            return Ok(reservations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reservations");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get reservation by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationDto>> GetReservationById(long id)
    {
        try
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
                return NotFound($"Reservation with ID {id} not found");

            return Ok(reservation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reservation with ID {ReservationId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get reservations by student
    /// </summary>
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservationsByStudent(long studentId)
    {
        try
        {
            var reservations = await _reservationService.GetReservationsByStudentAsync(studentId);
            return Ok(reservations);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reservations by student {StudentId}", studentId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get upcoming reservations by student
    /// </summary>
    [HttpGet("student/{studentId}/upcoming")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetUpcomingReservationsByStudent(long studentId)
    {
        try
        {
            var reservations = await _reservationService.GetUpcomingReservationsByStudentAsync(studentId);
            return Ok(reservations);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upcoming reservations by student {StudentId}", studentId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get reservations by class
    /// </summary>
    [HttpGet("class/{classId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservationsByClass(long classId)
    {
        try
        {
            var reservations = await _reservationService.GetReservationsByClassAsync(classId);
            return Ok(reservations);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reservations by class {ClassId}", classId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get reservations by instructor
    /// </summary>
    [HttpGet("instructor/{instructorId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservationsByInstructor(long instructorId)
    {
        try
        {
            var reservations = await _reservationService.GetReservationsByInstructorAsync(instructorId);
            return Ok(reservations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reservations by instructor {InstructorId}", instructorId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new reservation
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ReservationDto>> CreateReservation([FromBody] CreateReservationDto createReservationDto)
    {
        try
        {
            var createdReservation = await _reservationService.CreateReservationAsync(createReservationDto);
            return CreatedAtAction(nameof(GetReservationById), new { id = createdReservation.Id }, createdReservation);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating reservation");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update a reservation
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ReservationDto>> UpdateReservation(long id, [FromBody] UpdateReservationDto updateReservationDto)
    {
        try
        {
            var updatedReservation = await _reservationService.UpdateReservationAsync(id, updateReservationDto);
            if (updatedReservation == null)
                return NotFound($"Reservation with ID {id} not found");

            return Ok(updatedReservation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating reservation with ID {ReservationId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Cancel a reservation
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<ReservationDto>> CancelReservation(long id, [FromBody] CancelReservationDto cancelReservationDto)
    {
        try
        {
            var cancelledReservation = await _reservationService.CancelReservationAsync(id, cancelReservationDto);
            return Ok(cancelledReservation);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling reservation with ID {ReservationId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Complete a reservation (mark as completed)
    /// </summary>
    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> CompleteReservation(long id)
    {
        try
        {
            var result = await _reservationService.CompleteReservationAsync(id);
            if (!result)
                return BadRequest("Reservation could not be completed. Check if it exists and is confirmed.");

            return Ok(new { message = "Reservation completed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing reservation with ID {ReservationId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Mark reservation as no-show
    /// </summary>
    [HttpPost("{id}/no-show")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> MarkAsNoShow(long id)
    {
        try
        {
            var result = await _reservationService.MarkAsNoShowAsync(id);
            if (!result)
                return BadRequest("Reservation could not be marked as no-show. Check if it exists and is confirmed.");

            return Ok(new { message = "Reservation marked as no-show" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking reservation as no-show with ID {ReservationId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a reservation (hard delete - admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteReservation(long id)
    {
        try
        {
            var result = await _reservationService.DeleteReservationAsync(id);
            if (!result)
                return NotFound($"Reservation with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting reservation with ID {ReservationId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Check if student can reserve a specific class
    /// </summary>
    [HttpGet("student/{studentId}/can-reserve/{classId}")]
    public async Task<ActionResult<bool>> CanStudentReserveClass(long studentId, long classId)
    {
        try
        {
            var canReserve = await _reservationService.CanStudentReserveClassAsync(studentId, classId);
            return Ok(new { canReserve });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if student {StudentId} can reserve class {ClassId}", studentId, classId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Check if reservation can be cancelled
    /// </summary>
    [HttpGet("{id}/can-cancel")]
    public async Task<ActionResult<bool>> CanCancelReservation(long id)
    {
        try
        {
            var canCancel = await _reservationService.CanCancelReservationAsync(id);
            return Ok(new { canCancel });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if reservation {ReservationId} can be cancelled", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Process completed reservations (admin maintenance task)
    /// </summary>
    [HttpPost("process-completed")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> ProcessCompletedReservations()
    {
        try
        {
            var processedCount = await _reservationService.ProcessCompletedReservationsAsync();
            return Ok(new { processedCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing completed reservations");
            return StatusCode(500, "Internal server error");
        }
    }
}
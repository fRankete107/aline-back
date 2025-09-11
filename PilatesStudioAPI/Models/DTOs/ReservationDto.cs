namespace PilatesStudioAPI.Models.DTOs;

public class ReservationDto
{
    public long Id { get; set; }
    public long ClassId { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public long SubscriptionId { get; set; }
    
    // Class Information
    public DateOnly ClassDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? ClassType { get; set; }
    public string ClassLevel { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public string ZoneName { get; set; } = string.Empty;
    
    // Reservation Details
    public DateTime ReservationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Computed Properties
    public bool CanCancel { get; set; }
    public bool IsUpcoming { get; set; }
    public bool IsCompleted { get; set; }
    public int HoursUntilClass { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateReservationDto
{
    public long ClassId { get; set; }
    public long StudentId { get; set; }
}

public class UpdateReservationDto
{
    public string? Status { get; set; }
    public string? CancellationReason { get; set; }
}

public class CancelReservationDto
{
    public string? CancellationReason { get; set; }
}

public class ReservationFilterDto
{
    public long? ClassId { get; set; }
    public long? StudentId { get; set; }
    public long? InstructorId { get; set; }
    public long? ZoneId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Status { get; set; }
    public bool? UpcomingOnly { get; set; }
}
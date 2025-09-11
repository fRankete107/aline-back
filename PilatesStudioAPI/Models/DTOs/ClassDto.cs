namespace PilatesStudioAPI.Models.DTOs;

public class ClassDto
{
    public long Id { get; set; }
    public long InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public long ZoneId { get; set; }
    public string ZoneName { get; set; } = string.Empty;
    public DateOnly ClassDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int CapacityLimit { get; set; }
    public int ReservedSpots { get; set; }
    public int AvailableSpots { get; set; }
    public string? ClassType { get; set; }
    public string DifficultyLevel { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateClassDto
{
    public long InstructorId { get; set; }
    public long ZoneId { get; set; }
    public DateOnly ClassDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int CapacityLimit { get; set; }
    public string? ClassType { get; set; }
    public string DifficultyLevel { get; set; } = "beginner";
    public string? Description { get; set; }
}

public class UpdateClassDto
{
    public long? InstructorId { get; set; }
    public long? ZoneId { get; set; }
    public DateOnly? ClassDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public int? CapacityLimit { get; set; }
    public string? ClassType { get; set; }
    public string? DifficultyLevel { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
}

public class ClassFilterDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public long? InstructorId { get; set; }
    public long? ZoneId { get; set; }
    public string? DifficultyLevel { get; set; }
    public string? Status { get; set; }
    public bool? OnlyAvailable { get; set; }
}
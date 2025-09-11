namespace PilatesStudioAPI.Models.DTOs;

public class SubscriptionDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public long PlanId { get; set; }
    public string PlanTitle { get; set; } = string.Empty;
    public decimal PlanPrice { get; set; }
    public int ClassesRemaining { get; set; }
    public int TotalClasses { get; set; }
    public int UsedClasses { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public int DaysRemaining { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsExpired { get; set; }
    public bool IsExpiringSoon { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateSubscriptionDto
{
    public long StudentId { get; set; }
    public long PlanId { get; set; }
    public DateOnly? StartDate { get; set; }
}

public class UpdateSubscriptionDto
{
    public int? ClassesRemaining { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public string? Status { get; set; }
}

public class RenewSubscriptionDto
{
    public long PlanId { get; set; }
    public DateOnly? StartDate { get; set; }
}

public class SubscriptionFilterDto
{
    public long? StudentId { get; set; }
    public long? PlanId { get; set; }
    public string? Status { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool? ExpiringSoon { get; set; }
    public bool? HasClassesRemaining { get; set; }
}
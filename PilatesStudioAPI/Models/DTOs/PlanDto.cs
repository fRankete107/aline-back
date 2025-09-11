namespace PilatesStudioAPI.Models.DTOs;

public class PlanDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int MonthlyClasses { get; set; }
    public int ValidityDays { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int ActiveSubscriptionsCount { get; set; }
}

public class CreatePlanDto
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int MonthlyClasses { get; set; }
    public int ValidityDays { get; set; } = 30;
}

public class UpdatePlanDto
{
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? MonthlyClasses { get; set; }
    public int? ValidityDays { get; set; }
    public bool? IsActive { get; set; }
}
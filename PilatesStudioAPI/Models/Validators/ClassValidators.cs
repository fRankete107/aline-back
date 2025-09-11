using FluentValidation;
using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Models.Validators;

public class CreateClassValidator : AbstractValidator<CreateClassDto>
{
    public CreateClassValidator()
    {
        RuleFor(x => x.InstructorId)
            .GreaterThan(0).WithMessage("Valid instructor ID is required");

        RuleFor(x => x.ZoneId)
            .GreaterThan(0).WithMessage("Valid zone ID is required");

        RuleFor(x => x.ClassDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Class date must be today or in the future");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time");

        RuleFor(x => x.CapacityLimit)
            .GreaterThan(0).WithMessage("Capacity limit must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Capacity limit must not exceed 100");

        RuleFor(x => x.ClassType)
            .MaximumLength(100).WithMessage("Class type must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.ClassType));

        RuleFor(x => x.DifficultyLevel)
            .Must(x => new[] { "beginner", "intermediate", "advanced" }.Contains(x))
            .WithMessage("Difficulty level must be 'beginner', 'intermediate', or 'advanced'");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x)
            .Must(x => (x.EndTime.ToTimeSpan() - x.StartTime.ToTimeSpan()).TotalMinutes >= 30)
            .WithMessage("Class duration must be at least 30 minutes")
            .Must(x => (x.EndTime.ToTimeSpan() - x.StartTime.ToTimeSpan()).TotalHours <= 3)
            .WithMessage("Class duration must not exceed 3 hours");
    }
}

public class UpdateClassValidator : AbstractValidator<UpdateClassDto>
{
    public UpdateClassValidator()
    {
        RuleFor(x => x.InstructorId)
            .GreaterThan(0).WithMessage("Valid instructor ID is required")
            .When(x => x.InstructorId.HasValue);

        RuleFor(x => x.ZoneId)
            .GreaterThan(0).WithMessage("Valid zone ID is required")
            .When(x => x.ZoneId.HasValue);

        RuleFor(x => x.ClassDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Class date must be today or in the future")
            .When(x => x.ClassDate.HasValue);

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time")
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue);

        RuleFor(x => x.CapacityLimit)
            .GreaterThan(0).WithMessage("Capacity limit must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Capacity limit must not exceed 100")
            .When(x => x.CapacityLimit.HasValue);

        RuleFor(x => x.ClassType)
            .MaximumLength(100).WithMessage("Class type must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.ClassType));

        RuleFor(x => x.DifficultyLevel)
            .Must(x => new[] { "beginner", "intermediate", "advanced" }.Contains(x))
            .WithMessage("Difficulty level must be 'beginner', 'intermediate', or 'advanced'")
            .When(x => !string.IsNullOrEmpty(x.DifficultyLevel));

        RuleFor(x => x.Status)
            .Must(x => new[] { "scheduled", "ongoing", "completed", "cancelled" }.Contains(x))
            .WithMessage("Status must be 'scheduled', 'ongoing', 'completed', or 'cancelled'")
            .When(x => !string.IsNullOrEmpty(x.Status));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
using FluentValidation;
using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Models.Validators;

public class CreateSubscriptionValidator : AbstractValidator<CreateSubscriptionDto>
{
    public CreateSubscriptionValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0).WithMessage("Valid student ID is required");

        RuleFor(x => x.PlanId)
            .GreaterThan(0).WithMessage("Valid plan ID is required");

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
            .WithMessage("Start date cannot be more than 1 day in the past")
            .When(x => x.StartDate.HasValue);
    }
}

public class UpdateSubscriptionValidator : AbstractValidator<UpdateSubscriptionDto>
{
    public UpdateSubscriptionValidator()
    {
        RuleFor(x => x.ClassesRemaining)
            .GreaterThanOrEqualTo(0).WithMessage("Classes remaining cannot be negative")
            .LessThanOrEqualTo(1000).WithMessage("Classes remaining must not exceed 1000")
            .When(x => x.ClassesRemaining.HasValue);

        RuleFor(x => x.ExpiryDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Expiry date cannot be in the past")
            .When(x => x.ExpiryDate.HasValue);

        RuleFor(x => x.Status)
            .Must(x => new[] { "active", "expired", "cancelled" }.Contains(x))
            .WithMessage("Status must be 'active', 'expired', or 'cancelled'")
            .When(x => !string.IsNullOrEmpty(x.Status));
    }
}

public class RenewSubscriptionValidator : AbstractValidator<RenewSubscriptionDto>
{
    public RenewSubscriptionValidator()
    {
        RuleFor(x => x.PlanId)
            .GreaterThan(0).WithMessage("Valid plan ID is required");

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Start date cannot be in the past")
            .When(x => x.StartDate.HasValue);
    }
}
using FluentValidation;
using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Models.Validators;

public class CreatePlanValidator : AbstractValidator<CreatePlanDto>
{
    public CreatePlanValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Plan title is required")
            .MaximumLength(255).WithMessage("Plan title must not exceed 255 characters");

        RuleFor(x => x.Subtitle)
            .MaximumLength(255).WithMessage("Plan subtitle must not exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.Subtitle));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Plan description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Plan price must be greater than 0")
            .LessThanOrEqualTo(99999.99m).WithMessage("Plan price must not exceed $99,999.99");

        RuleFor(x => x.MonthlyClasses)
            .GreaterThan(0).WithMessage("Monthly classes must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Monthly classes must not exceed 100");

        RuleFor(x => x.ValidityDays)
            .GreaterThan(0).WithMessage("Validity days must be greater than 0")
            .LessThanOrEqualTo(365).WithMessage("Validity days must not exceed 365 days");
    }
}

public class UpdatePlanValidator : AbstractValidator<UpdatePlanDto>
{
    public UpdatePlanValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(255).WithMessage("Plan title must not exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Subtitle)
            .MaximumLength(255).WithMessage("Plan subtitle must not exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.Subtitle));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Plan description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Plan price must be greater than 0")
            .LessThanOrEqualTo(99999.99m).WithMessage("Plan price must not exceed $99,999.99")
            .When(x => x.Price.HasValue);

        RuleFor(x => x.MonthlyClasses)
            .GreaterThan(0).WithMessage("Monthly classes must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Monthly classes must not exceed 100")
            .When(x => x.MonthlyClasses.HasValue);

        RuleFor(x => x.ValidityDays)
            .GreaterThan(0).WithMessage("Validity days must be greater than 0")
            .LessThanOrEqualTo(365).WithMessage("Validity days must not exceed 365 days")
            .When(x => x.ValidityDays.HasValue);
    }
}
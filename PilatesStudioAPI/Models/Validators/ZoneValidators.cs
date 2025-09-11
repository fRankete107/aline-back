using FluentValidation;
using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Models.Validators;

public class CreateZoneValidator : AbstractValidator<CreateZoneDto>
{
    public CreateZoneValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Zone name is required")
            .MaximumLength(100).WithMessage("Zone name must not exceed 100 characters");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Capacity must not exceed 100 people");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.EquipmentAvailable)
            .MaximumLength(500).WithMessage("Equipment description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.EquipmentAvailable));
    }
}

public class UpdateZoneValidator : AbstractValidator<UpdateZoneDto>
{
    public UpdateZoneValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Zone name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Capacity must not exceed 100 people")
            .When(x => x.Capacity.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.EquipmentAvailable)
            .MaximumLength(500).WithMessage("Equipment description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.EquipmentAvailable));
    }
}
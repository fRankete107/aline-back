using FluentValidation;
using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Models.Validators;

public class CreateReservationValidator : AbstractValidator<CreateReservationDto>
{
    public CreateReservationValidator()
    {
        RuleFor(x => x.ClassId)
            .GreaterThan(0).WithMessage("Valid class ID is required");

        RuleFor(x => x.StudentId)
            .GreaterThan(0).WithMessage("Valid student ID is required");
    }
}

public class UpdateReservationValidator : AbstractValidator<UpdateReservationDto>
{
    public UpdateReservationValidator()
    {
        RuleFor(x => x.Status)
            .Must(x => new[] { "confirmed", "cancelled", "completed", "no_show" }.Contains(x))
            .WithMessage("Status must be 'confirmed', 'cancelled', 'completed', or 'no_show'")
            .When(x => !string.IsNullOrEmpty(x.Status));

        RuleFor(x => x.CancellationReason)
            .MaximumLength(500).WithMessage("Cancellation reason must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.CancellationReason));
    }
}

public class CancelReservationValidator : AbstractValidator<CancelReservationDto>
{
    public CancelReservationValidator()
    {
        RuleFor(x => x.CancellationReason)
            .MaximumLength(500).WithMessage("Cancellation reason must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.CancellationReason));
    }
}
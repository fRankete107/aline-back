using FluentValidation;
using PilatesStudioAPI.Models.DTOs.Auth;

namespace PilatesStudioAPI.Validators.Auth;

public class ForgotPasswordRequestDtoValidator : AbstractValidator<ForgotPasswordRequestDto>
{
    public ForgotPasswordRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es obligatorio")
            .EmailAddress()
            .WithMessage("El formato del email no es válido")
            .MaximumLength(255)
            .WithMessage("El email no puede exceder 255 caracteres");
    }
}
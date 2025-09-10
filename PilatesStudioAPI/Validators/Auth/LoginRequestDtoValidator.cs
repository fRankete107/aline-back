using FluentValidation;
using PilatesStudioAPI.Models.DTOs.Auth;

namespace PilatesStudioAPI.Validators.Auth;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es obligatorio")
            .EmailAddress()
            .WithMessage("El formato del email no es válido")
            .MaximumLength(255)
            .WithMessage("El email no puede exceder 255 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es obligatoria")
            .MinimumLength(8)
            .WithMessage("La contraseña debe tener al menos 8 caracteres")
            .MaximumLength(100)
            .WithMessage("La contraseña no puede exceder 100 caracteres");
    }
}
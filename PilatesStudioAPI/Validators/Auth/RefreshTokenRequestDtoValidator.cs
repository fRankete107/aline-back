using FluentValidation;
using PilatesStudioAPI.Models.DTOs.Auth;

namespace PilatesStudioAPI.Validators.Auth;

public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshTokenRequestDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("El token de actualización es obligatorio")
            .MaximumLength(500)
            .WithMessage("El token de actualización no es válido");
    }
}
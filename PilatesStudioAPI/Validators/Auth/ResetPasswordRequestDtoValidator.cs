using FluentValidation;
using PilatesStudioAPI.Models.DTOs.Auth;

namespace PilatesStudioAPI.Validators.Auth;

public class ResetPasswordRequestDtoValidator : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestDtoValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("El token es obligatorio")
            .MaximumLength(500)
            .WithMessage("El token no es válido");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es obligatoria")
            .MinimumLength(8)
            .WithMessage("La contraseña debe tener al menos 8 caracteres")
            .MaximumLength(100)
            .WithMessage("La contraseña no puede exceder 100 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("La contraseña debe contener al menos: una minúscula, una mayúscula, un dígito y un carácter especial (@$!%*?&)");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("La confirmación de contraseña es obligatoria")
            .Equal(x => x.Password)
            .WithMessage("Las contraseñas no coinciden");
    }
}
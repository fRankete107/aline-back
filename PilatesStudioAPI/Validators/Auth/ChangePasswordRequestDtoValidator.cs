using FluentValidation;
using PilatesStudioAPI.Models.DTOs.Auth;

namespace PilatesStudioAPI.Validators.Auth;

public class ChangePasswordRequestDtoValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("La contraseña actual es obligatoria")
            .MaximumLength(100)
            .WithMessage("La contraseña actual no puede exceder 100 caracteres");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("La nueva contraseña es obligatoria")
            .MinimumLength(8)
            .WithMessage("La nueva contraseña debe tener al menos 8 caracteres")
            .MaximumLength(100)
            .WithMessage("La nueva contraseña no puede exceder 100 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("La nueva contraseña debe contener al menos: una minúscula, una mayúscula, un dígito y un carácter especial (@$!%*?&)")
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("La nueva contraseña debe ser diferente a la actual");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty()
            .WithMessage("La confirmación de la nueva contraseña es obligatoria")
            .Equal(x => x.NewPassword)
            .WithMessage("La confirmación no coincide con la nueva contraseña");
    }
}
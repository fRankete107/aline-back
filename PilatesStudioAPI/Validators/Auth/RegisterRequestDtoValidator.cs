using FluentValidation;
using PilatesStudioAPI.Models.DTOs.Auth;

namespace PilatesStudioAPI.Validators.Auth;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
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
            .WithMessage("La contraseña no puede exceder 100 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$")
            .WithMessage("La contraseña debe contener al menos: una minúscula, una mayúscula, un dígito y un carácter especial (@$!%*?&)");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("La confirmación de contraseña es obligatoria")
            .Equal(x => x.Password)
            .WithMessage("Las contraseñas no coinciden");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio")
            .MinimumLength(2)
            .WithMessage("El nombre debe tener al menos 2 caracteres")
            .MaximumLength(50)
            .WithMessage("El nombre no puede exceder 50 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
            .WithMessage("El nombre solo puede contener letras y espacios");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("El apellido es obligatorio")
            .MinimumLength(2)
            .WithMessage("El apellido debe tener al menos 2 caracteres")
            .MaximumLength(50)
            .WithMessage("El apellido no puede exceder 50 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
            .WithMessage("El apellido solo puede contener letras y espacios");

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .WithMessage("El teléfono no puede exceder 20 caracteres")
            .Matches(@"^[\+]?[0-9\-\s\(\)]*$")
            .WithMessage("El formato del teléfono no es válido")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("El rol es obligatorio")
            .Must(BeValidRole)
            .WithMessage("El rol debe ser: admin, instructor o student");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Today.AddYears(-13))
            .WithMessage("Debe ser mayor de 13 años")
            .GreaterThan(DateTime.Today.AddYears(-120))
            .WithMessage("La fecha de nacimiento no es válida")
            .When(x => x.BirthDate.HasValue);

        RuleFor(x => x.EmergencyContact)
            .MaximumLength(100)
            .WithMessage("El contacto de emergencia no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.EmergencyContact));
    }

    private static bool BeValidRole(string role)
    {
        var validRoles = new[] { "admin", "instructor", "student" };
        return validRoles.Contains(role?.ToLower());
    }
}
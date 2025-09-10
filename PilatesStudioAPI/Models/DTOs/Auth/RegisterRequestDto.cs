using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.DTOs.Auth;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$", 
        ErrorMessage = "La contraseña debe tener al menos una mayúscula, una minúscula, un número y un carácter especial")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
    public string LastName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "El formato del teléfono no es válido")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio")]
    [RegularExpression("^(admin|instructor|student)$", ErrorMessage = "El rol debe ser admin, instructor o student")]
    public string Role { get; set; } = "student";

    public DateTime? BirthDate { get; set; }

    public string? EmergencyContact { get; set; }
}
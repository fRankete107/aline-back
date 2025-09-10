using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.DTOs.Auth;

public class ChangePasswordRequestDto
{
    [Required(ErrorMessage = "La contraseña actual es obligatoria")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]", 
        ErrorMessage = "La contraseña debe tener al menos una mayúscula, una minúscula, un número y un carácter especial")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
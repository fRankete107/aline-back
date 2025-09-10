using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.DTOs.Auth;

public class ResetPasswordRequestDto
{
    [Required(ErrorMessage = "El token es obligatorio")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]", ErrorMessage = "La contraseña debe contener al menos: una minúscula, una mayúscula, un dígito y un carácter especial (@$!%*?&)")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
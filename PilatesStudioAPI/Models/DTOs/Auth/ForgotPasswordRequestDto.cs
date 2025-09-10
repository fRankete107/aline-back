using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.DTOs.Auth;

public class ForgotPasswordRequestDto
{
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; } = string.Empty;
}
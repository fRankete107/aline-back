using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.DTOs.Users;

public class InstructorDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Phone { get; set; }
    public string? Specializations { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // User information
    public string Email { get; set; } = string.Empty;
}

public class InstructorCreateDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Phone(ErrorMessage = "El formato del teléfono no es válido")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    public string? Phone { get; set; }

    [StringLength(500, ErrorMessage = "Las especializaciones no pueden exceder 500 caracteres")]
    public string? Specializations { get; set; }

    [StringLength(1000, ErrorMessage = "La biografía no puede exceder 1000 caracteres")]
    public string? Bio { get; set; }

    public bool IsActive { get; set; } = true;
}

public class InstructorUpdateDto
{
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string? FirstName { get; set; }

    [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
    public string? LastName { get; set; }

    [Phone(ErrorMessage = "El formato del teléfono no es válido")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    public string? Phone { get; set; }

    [StringLength(500, ErrorMessage = "Las especializaciones no pueden exceder 500 caracteres")]
    public string? Specializations { get; set; }

    [StringLength(1000, ErrorMessage = "La biografía no puede exceder 1000 caracteres")]
    public string? Bio { get; set; }

    public bool? IsActive { get; set; }
}
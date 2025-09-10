using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.DTOs.Users;

public class StudentDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Phone { get; set; }
    public DateOnly? BirthDate { get; set; }
    public int? Age => BirthDate?.GetAge();
    public string? Nit { get; set; }
    public string? EmergencyContact { get; set; }
    public string? MedicalNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // User information
    public string Email { get; set; } = string.Empty;

    // Subscription information (will be added later)
    public List<object> ActiveSubscriptions { get; set; } = new();
}

public class StudentCreateDto
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

    public DateOnly? BirthDate { get; set; }

    [StringLength(50, ErrorMessage = "El NIT no puede exceder 50 caracteres")]
    public string? Nit { get; set; }

    [StringLength(255, ErrorMessage = "El contacto de emergencia no puede exceder 255 caracteres")]
    public string? EmergencyContact { get; set; }

    [StringLength(1000, ErrorMessage = "Las notas médicas no pueden exceder 1000 caracteres")]
    public string? MedicalNotes { get; set; }
}

public class StudentUpdateDto
{
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string? FirstName { get; set; }

    [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
    public string? LastName { get; set; }

    [Phone(ErrorMessage = "El formato del teléfono no es válido")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    public string? Phone { get; set; }

    public DateOnly? BirthDate { get; set; }

    [StringLength(50, ErrorMessage = "El NIT no puede exceder 50 caracteres")]
    public string? Nit { get; set; }

    [StringLength(255, ErrorMessage = "El contacto de emergencia no puede exceder 255 caracteres")]
    public string? EmergencyContact { get; set; }

    [StringLength(1000, ErrorMessage = "Las notas médicas no pueden exceder 1000 caracteres")]
    public string? MedicalNotes { get; set; }
}

// Extension method for age calculation
public static class DateOnlyExtensions
{
    public static int GetAge(this DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - birthDate.Year;
        
        if (birthDate > today.AddYears(-age))
            age--;
            
        return age;
    }
}
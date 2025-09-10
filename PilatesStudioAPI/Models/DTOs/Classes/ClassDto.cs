using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.DTOs.Classes;

public class ClassDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DateTime { get; set; }
    public int Duration { get; set; }
    public int MaxCapacity { get; set; }
    public int AvailableSpots { get; set; }
    public long InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateClassDto
{
    [Required(ErrorMessage = "El nombre de la clase es obligatorio")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "La fecha y hora son obligatorias")]
    public DateTime DateTime { get; set; }

    [Range(15, 180, ErrorMessage = "La duración debe estar entre 15 y 180 minutos")]
    public int Duration { get; set; } = 60;

    [Range(1, 50, ErrorMessage = "La capacidad máxima debe estar entre 1 y 50 personas")]
    public int MaxCapacity { get; set; } = 10;

    [Required(ErrorMessage = "El instructor es obligatorio")]
    public long InstructorId { get; set; }
}

public class UpdateClassDto
{
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string? Name { get; set; }

    [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    public string? Description { get; set; }

    public DateTime? DateTime { get; set; }

    [Range(15, 180, ErrorMessage = "La duración debe estar entre 15 y 180 minutos")]
    public int? Duration { get; set; }

    [Range(1, 50, ErrorMessage = "La capacidad máxima debe estar entre 1 y 50 personas")]
    public int? MaxCapacity { get; set; }

    public long? InstructorId { get; set; }
}
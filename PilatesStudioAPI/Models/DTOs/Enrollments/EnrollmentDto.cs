using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.DTOs.Enrollments;

public class EnrollmentDto
{
    public long Id { get; set; }
    public long ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public DateTime ClassDateTime { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateEnrollmentDto
{
    [Required(ErrorMessage = "La clase es obligatoria")]
    public long ClassId { get; set; }

    [Required(ErrorMessage = "El estudiante es obligatorio")]
    public long StudentId { get; set; }

    [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
    public string? Notes { get; set; }
}

public class UpdateEnrollmentDto
{
    [Required(ErrorMessage = "El estado es obligatorio")]
    [RegularExpression("^(confirmed|cancelled|completed)$", ErrorMessage = "El estado debe ser: confirmed, cancelled o completed")]
    public string Status { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
    public string? Notes { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.DTOs.Attendance;

public class AttendanceDto
{
    public long Id { get; set; }
    public long ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public DateTime AttendanceDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateAttendanceDto
{
    [Required(ErrorMessage = "La clase es obligatoria")]
    public long ClassId { get; set; }

    [Required(ErrorMessage = "El estudiante es obligatorio")]
    public long StudentId { get; set; }

    [Required(ErrorMessage = "La fecha de asistencia es obligatoria")]
    public DateTime AttendanceDate { get; set; }

    [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
    public string? Notes { get; set; }
}

public class UpdateAttendanceDto
{
    public DateTime? AttendanceDate { get; set; }

    [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
    public string? Notes { get; set; }
}
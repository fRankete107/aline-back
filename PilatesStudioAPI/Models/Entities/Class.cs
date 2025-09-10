using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PilatesStudioAPI.Models.Entities;

public class Class
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long InstructorId { get; set; }

    [Required]
    public long ZoneId { get; set; }

    [Required]
    public DateOnly ClassDate { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    [Required]
    public int CapacityLimit { get; set; }

    [MaxLength(100)]
    public string? ClassType { get; set; }

    [MaxLength(20)]
    public string DifficultyLevel { get; set; } = "beginner"; // beginner, intermediate, advanced

    public string? Description { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "scheduled"; // scheduled, ongoing, completed, cancelled

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(InstructorId))]
    public Instructor Instructor { get; set; } = null!;

    [ForeignKey(nameof(ZoneId))]
    public Zone Zone { get; set; } = null!;

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
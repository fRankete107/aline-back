using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PilatesStudioAPI.Models.Entities;

public class Subscription
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long StudentId { get; set; }

    [Required]
    public long PlanId { get; set; }

    [Required]
    public int ClassesRemaining { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly ExpiryDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "active"; // active, expired, cancelled

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(StudentId))]
    public Student Student { get; set; } = null!;

    [ForeignKey(nameof(PlanId))]
    public Plan Plan { get; set; } = null!;

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
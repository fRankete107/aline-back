using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PilatesStudioAPI.Models.Entities;

public class Reservation
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long ClassId { get; set; }

    [Required]
    public long StudentId { get; set; }

    [Required]
    public long SubscriptionId { get; set; }

    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "confirmed"; // confirmed, cancelled, completed, no_show

    public string? CancellationReason { get; set; }

    public DateTime? CancelledAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(ClassId))]
    public Class Class { get; set; } = null!;

    [ForeignKey(nameof(StudentId))]
    public Student Student { get; set; } = null!;

    [ForeignKey(nameof(SubscriptionId))]
    public Subscription Subscription { get; set; } = null!;
}
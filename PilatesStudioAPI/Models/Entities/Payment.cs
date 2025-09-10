using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PilatesStudioAPI.Models.Entities;

public class Payment
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long StudentId { get; set; }

    [Required]
    public long PlanId { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(20)]
    public string PaymentMethod { get; set; } = string.Empty; // cash, credit_card, debit_card, bank_transfer, digital_wallet

    [MaxLength(255)]
    public string? PaymentReference { get; set; }

    [MaxLength(100)]
    public string? ReceiptNumber { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "pending"; // pending, completed, failed, refunded

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(StudentId))]
    public Student Student { get; set; } = null!;

    [ForeignKey(nameof(PlanId))]
    public Plan Plan { get; set; } = null!;
}
using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.DTOs.Payments;

public class PaymentDto
{
    public long Id { get; set; }
    public long PurchaseId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreatePaymentDto
{
    [Required(ErrorMessage = "La compra es obligatoria")]
    public long PurchaseId { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio")]
    [Range(0.01, 999999.99, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "El método de pago es obligatorio")]
    [RegularExpression("^(cash|card|transfer|other)$", ErrorMessage = "El método de pago debe ser: cash, card, transfer u other")]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de pago es obligatoria")]
    public DateTime PaymentDate { get; set; }

    [StringLength(100, ErrorMessage = "El ID de transacción no puede exceder 100 caracteres")]
    public string? TransactionId { get; set; }

    [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
    public string? Notes { get; set; }
}

public class UpdatePaymentDto
{
    [RegularExpression("^(pending|completed|failed|refunded)$", ErrorMessage = "El estado debe ser: pending, completed, failed o refunded")]
    public string? Status { get; set; }

    [StringLength(100, ErrorMessage = "El ID de transacción no puede exceder 100 caracteres")]
    public string? TransactionId { get; set; }

    [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
    public string? Notes { get; set; }
}
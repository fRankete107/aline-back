using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.DTOs;

public class PaymentDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public long PlanId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? PaymentReference { get; set; }
    public string? ReceiptNumber { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public string StudentName { get; set; } = string.Empty;
    public string PlanTitle { get; set; } = string.Empty;
    
    // Computed properties
    public bool IsRefundable { get; set; }
    public int DaysSincePayment { get; set; }
}

public class CreatePaymentDto
{
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "StudentId must be a positive number")]
    public long StudentId { get; set; }

    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "PlanId must be a positive number")]
    public long PlanId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(20, ErrorMessage = "PaymentMethod cannot exceed 20 characters")]
    public string PaymentMethod { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "PaymentReference cannot exceed 255 characters")]
    public string? PaymentReference { get; set; }

    [StringLength(100, ErrorMessage = "ReceiptNumber cannot exceed 100 characters")]
    public string? ReceiptNumber { get; set; }

    public DateTime? PaymentDate { get; set; }

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
}

public class UpdatePaymentDto
{
    [StringLength(255, ErrorMessage = "PaymentReference cannot exceed 255 characters")]
    public string? PaymentReference { get; set; }

    [StringLength(100, ErrorMessage = "ReceiptNumber cannot exceed 100 characters")]
    public string? ReceiptNumber { get; set; }

    public DateTime? PaymentDate { get; set; }

    [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
    public string? Status { get; set; }

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
}

public class PaymentFilterDto
{
    public long? StudentId { get; set; }
    public long? PlanId { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class RefundPaymentDto
{
    [Required]
    [StringLength(1000, ErrorMessage = "Reason cannot exceed 1000 characters")]
    public string Reason { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "RefundAmount must be greater than 0")]
    public decimal? RefundAmount { get; set; } // null means full refund
}

public class PaymentStatsDto
{
    public decimal TotalAmount { get; set; }
    public int TotalPayments { get; set; }
    public int PendingPayments { get; set; }
    public int CompletedPayments { get; set; }
    public int FailedPayments { get; set; }
    public int RefundedPayments { get; set; }
    public decimal AveragePaymentAmount { get; set; }
    public Dictionary<string, int> PaymentMethodStats { get; set; } = new();
    public Dictionary<string, decimal> MonthlyRevenue { get; set; } = new();
}
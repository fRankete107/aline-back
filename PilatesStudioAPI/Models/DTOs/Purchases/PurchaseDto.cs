using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.DTOs.Purchases;

public class PurchaseDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public long PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int RemainingClasses { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreatePurchaseDto
{
    [Required(ErrorMessage = "El estudiante es obligatorio")]
    public long StudentId { get; set; }

    [Required(ErrorMessage = "El paquete es obligatorio")]
    public long PackageId { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio")]
    [Range(0.01, 999999.99, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Amount { get; set; }
}

public class UpdatePurchaseDto
{
    [RegularExpression("^(active|expired|cancelled)$", ErrorMessage = "El estado debe ser: active, expired o cancelled")]
    public string? Status { get; set; }

    public DateTime? ExpirationDate { get; set; }

    [Range(0, 999, ErrorMessage = "Las clases restantes deben ser entre 0 y 999")]
    public int? RemainingClasses { get; set; }
}
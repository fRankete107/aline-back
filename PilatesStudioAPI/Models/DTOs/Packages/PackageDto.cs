using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.DTOs.Packages;

public class PackageDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int ClassCount { get; set; }
    public int ValidityDays { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreatePackageDto
{
    [Required(ErrorMessage = "El nombre del paquete es obligatorio")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "El número de clases es obligatorio")]
    [Range(1, 999, ErrorMessage = "El número de clases debe estar entre 1 y 999")]
    public int ClassCount { get; set; }

    [Required(ErrorMessage = "Los días de validez son obligatorios")]
    [Range(1, 365, ErrorMessage = "Los días de validez deben estar entre 1 y 365")]
    public int ValidityDays { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdatePackageDto
{
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string? Name { get; set; }

    [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    public string? Description { get; set; }

    [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal? Price { get; set; }

    [Range(1, 999, ErrorMessage = "El número de clases debe estar entre 1 y 999")]
    public int? ClassCount { get; set; }

    [Range(1, 365, ErrorMessage = "Los días de validez deben estar entre 1 y 365")]
    public int? ValidityDays { get; set; }

    public bool? IsActive { get; set; }
}
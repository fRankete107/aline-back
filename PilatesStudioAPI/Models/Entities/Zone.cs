using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.Entities;

public class Zone
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public int Capacity { get; set; }

    public string? EquipmentAvailable { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Class> Classes { get; set; } = new List<Class>();
}
namespace PilatesStudioAPI.Models.DTOs;

public class ZoneDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public string? EquipmentAvailable { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateZoneDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public string? EquipmentAvailable { get; set; }
}

public class UpdateZoneDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Capacity { get; set; }
    public string? EquipmentAvailable { get; set; }
    public bool? IsActive { get; set; }
}
using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Services.Interfaces;

public interface IZoneService
{
    Task<IEnumerable<ZoneDto>> GetAllZonesAsync();
    Task<ZoneDto?> GetZoneByIdAsync(long id);
    Task<ZoneDto> CreateZoneAsync(CreateZoneDto createZoneDto);
    Task<ZoneDto?> UpdateZoneAsync(long id, UpdateZoneDto updateZoneDto);
    Task<bool> DeleteZoneAsync(long id);
    Task<IEnumerable<ZoneDto>> GetActiveZonesAsync();
    Task<bool> ZoneExistsAsync(long id);
    Task<bool> ZoneNameExistsAsync(string name, long? excludeId = null);
}
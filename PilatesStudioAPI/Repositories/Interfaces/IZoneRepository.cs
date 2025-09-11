using PilatesStudioAPI.Models.Entities;

namespace PilatesStudioAPI.Repositories.Interfaces;

public interface IZoneRepository
{
    Task<IEnumerable<Zone>> GetAllAsync();
    Task<Zone?> GetByIdAsync(long id);
    Task<Zone> CreateAsync(Zone zone);
    Task<Zone?> UpdateAsync(long id, Zone zone);
    Task<bool> DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
    Task<IEnumerable<Zone>> GetActiveZonesAsync();
    Task<Zone?> GetByNameAsync(string name);
}
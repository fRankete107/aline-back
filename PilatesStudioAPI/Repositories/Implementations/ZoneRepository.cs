using Microsoft.EntityFrameworkCore;
using PilatesStudioAPI.Data.Context;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Repositories.Interfaces;

namespace PilatesStudioAPI.Repositories.Implementations;

public class ZoneRepository : IZoneRepository
{
    private readonly PilatesStudioDbContext _context;

    public ZoneRepository(PilatesStudioDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Zone>> GetAllAsync()
    {
        return await _context.Zones
            .OrderBy(z => z.Name)
            .ToListAsync();
    }

    public async Task<Zone?> GetByIdAsync(long id)
    {
        return await _context.Zones
            .FirstOrDefaultAsync(z => z.Id == id);
    }

    public async Task<Zone> CreateAsync(Zone zone)
    {
        zone.CreatedAt = DateTime.UtcNow;
        zone.UpdatedAt = DateTime.UtcNow;
        
        _context.Zones.Add(zone);
        await _context.SaveChangesAsync();
        return zone;
    }

    public async Task<Zone?> UpdateAsync(long id, Zone zone)
    {
        var existingZone = await _context.Zones.FindAsync(id);
        if (existingZone == null)
            return null;

        existingZone.Name = zone.Name;
        existingZone.Description = zone.Description;
        existingZone.Capacity = zone.Capacity;
        existingZone.EquipmentAvailable = zone.EquipmentAvailable;
        existingZone.IsActive = zone.IsActive;
        existingZone.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingZone;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var zone = await _context.Zones.FindAsync(id);
        if (zone == null)
            return false;

        var hasClasses = await _context.Classes
            .AnyAsync(c => c.ZoneId == id);

        if (hasClasses)
        {
            zone.IsActive = false;
            zone.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _context.Zones.Remove(zone);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Zones.AnyAsync(z => z.Id == id);
    }

    public async Task<IEnumerable<Zone>> GetActiveZonesAsync()
    {
        return await _context.Zones
            .Where(z => z.IsActive)
            .OrderBy(z => z.Name)
            .ToListAsync();
    }

    public async Task<Zone?> GetByNameAsync(string name)
    {
        return await _context.Zones
            .FirstOrDefaultAsync(z => z.Name.ToLower() == name.ToLower());
    }
}
using AutoMapper;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Repositories.Interfaces;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Services.Implementations;

public class ZoneService : IZoneService
{
    private readonly IZoneRepository _zoneRepository;
    private readonly IMapper _mapper;

    public ZoneService(IZoneRepository zoneRepository, IMapper mapper)
    {
        _zoneRepository = zoneRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ZoneDto>> GetAllZonesAsync()
    {
        var zones = await _zoneRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ZoneDto>>(zones);
    }

    public async Task<ZoneDto?> GetZoneByIdAsync(long id)
    {
        var zone = await _zoneRepository.GetByIdAsync(id);
        return zone == null ? null : _mapper.Map<ZoneDto>(zone);
    }

    public async Task<ZoneDto> CreateZoneAsync(CreateZoneDto createZoneDto)
    {
        if (await ZoneNameExistsAsync(createZoneDto.Name))
        {
            throw new InvalidOperationException($"A zone with the name '{createZoneDto.Name}' already exists.");
        }

        var zone = _mapper.Map<Zone>(createZoneDto);
        var createdZone = await _zoneRepository.CreateAsync(zone);
        return _mapper.Map<ZoneDto>(createdZone);
    }

    public async Task<ZoneDto?> UpdateZoneAsync(long id, UpdateZoneDto updateZoneDto)
    {
        var existingZone = await _zoneRepository.GetByIdAsync(id);
        if (existingZone == null)
            return null;

        if (!string.IsNullOrEmpty(updateZoneDto.Name) && 
            await ZoneNameExistsAsync(updateZoneDto.Name, id))
        {
            throw new InvalidOperationException($"A zone with the name '{updateZoneDto.Name}' already exists.");
        }

        _mapper.Map(updateZoneDto, existingZone);
        var updatedZone = await _zoneRepository.UpdateAsync(id, existingZone);
        return updatedZone == null ? null : _mapper.Map<ZoneDto>(updatedZone);
    }

    public async Task<bool> DeleteZoneAsync(long id)
    {
        if (!await _zoneRepository.ExistsAsync(id))
            return false;

        return await _zoneRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ZoneDto>> GetActiveZonesAsync()
    {
        var zones = await _zoneRepository.GetActiveZonesAsync();
        return _mapper.Map<IEnumerable<ZoneDto>>(zones);
    }

    public async Task<bool> ZoneExistsAsync(long id)
    {
        return await _zoneRepository.ExistsAsync(id);
    }

    public async Task<bool> ZoneNameExistsAsync(string name, long? excludeId = null)
    {
        var existingZone = await _zoneRepository.GetByNameAsync(name);
        return existingZone != null && (!excludeId.HasValue || existingZone.Id != excludeId.Value);
    }
}
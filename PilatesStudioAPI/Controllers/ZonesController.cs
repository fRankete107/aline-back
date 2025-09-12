using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ZonesController : ControllerBase
{
    private readonly IZoneService _zoneService;
    private readonly ILogger<ZonesController> _logger;

    public ZonesController(IZoneService zoneService, ILogger<ZonesController> logger)
    {
        _zoneService = zoneService;
        _logger = logger;
    }

    /// <summary>
    /// Get all zones
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ZoneDto>>> GetAllZones()
    {
        try
        {
            var zones = await _zoneService.GetAllZonesAsync();
            return Ok(zones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all zones");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get active zones only
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<ZoneDto>>> GetActiveZones()
    {
        try
        {
            var zones = await _zoneService.GetActiveZonesAsync();
            return Ok(zones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active zones");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get zone by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ZoneDto>> GetZoneById(long id)
    {
        try
        {
            var zone = await _zoneService.GetZoneByIdAsync(id);
            if (zone == null)
                return NotFound($"Zone with ID {id} not found");

            return Ok(zone);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting zone with ID {ZoneId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new zone
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ZoneDto>> CreateZone([FromBody] CreateZoneDto createZoneDto)
    {
        try
        {
            var createdZone = await _zoneService.CreateZoneAsync(createZoneDto);
            return CreatedAtAction(nameof(GetZoneById), new { id = createdZone.Id }, createdZone);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating zone");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update a zone
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ZoneDto>> UpdateZone(long id, [FromBody] UpdateZoneDto updateZoneDto)
    {
        try
        {
            var updatedZone = await _zoneService.UpdateZoneAsync(id, updateZoneDto);
            if (updatedZone == null)
                return NotFound($"Zone with ID {id} not found");

            return Ok(updatedZone);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating zone with ID {ZoneId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a zone (soft delete if has classes, hard delete if no classes)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteZone(long id)
    {
        try
        {
            var result = await _zoneService.DeleteZoneAsync(id);
            if (!result)
                return NotFound($"Zone with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting zone with ID {ZoneId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
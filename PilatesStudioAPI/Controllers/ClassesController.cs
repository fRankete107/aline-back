using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClassesController : ControllerBase
{
    private readonly IClassService _classService;
    private readonly ILogger<ClassesController> _logger;

    public ClassesController(IClassService classService, ILogger<ClassesController> logger)
    {
        _classService = classService;
        _logger = logger;
    }

    /// <summary>
    /// Get all classes with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetClasses([FromQuery] ClassFilterDto? filter = null)
    {
        try
        {
            IEnumerable<ClassDto> classes;
            
            if (filter != null && (filter.StartDate.HasValue || filter.EndDate.HasValue || 
                filter.InstructorId.HasValue || filter.ZoneId.HasValue || 
                !string.IsNullOrEmpty(filter.DifficultyLevel) || !string.IsNullOrEmpty(filter.Status) ||
                filter.OnlyAvailable.HasValue))
            {
                classes = await _classService.GetFilteredClassesAsync(filter);
            }
            else
            {
                classes = await _classService.GetAllClassesAsync();
            }

            return Ok(classes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting classes");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get available classes (with free spots)
    /// </summary>
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetAvailableClasses()
    {
        try
        {
            var classes = await _classService.GetAvailableClassesAsync();
            return Ok(classes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available classes");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get classes by date range
    /// </summary>
    [HttpGet("date-range")]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetClassesByDateRange(
        [FromQuery] DateOnly startDate, 
        [FromQuery] DateOnly endDate)
    {
        try
        {
            var classes = await _classService.GetClassesByDateRangeAsync(startDate, endDate);
            return Ok(classes);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting classes by date range");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get classes by instructor
    /// </summary>
    [HttpGet("instructor/{instructorId}")]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetClassesByInstructor(long instructorId)
    {
        try
        {
            var classes = await _classService.GetClassesByInstructorAsync(instructorId);
            return Ok(classes);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting classes by instructor {InstructorId}", instructorId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get classes by zone
    /// </summary>
    [HttpGet("zone/{zoneId}")]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetClassesByZone(long zoneId)
    {
        try
        {
            var classes = await _classService.GetClassesByZoneAsync(zoneId);
            return Ok(classes);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting classes by zone {ZoneId}", zoneId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get class by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ClassDto>> GetClassById(long id)
    {
        try
        {
            var classDto = await _classService.GetClassByIdAsync(id);
            if (classDto == null)
                return NotFound($"Class with ID {id} not found");

            return Ok(classDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting class with ID {ClassId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new class
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<ClassDto>> CreateClass([FromBody] CreateClassDto createClassDto)
    {
        try
        {
            var createdClass = await _classService.CreateClassAsync(createClassDto);
            return CreatedAtAction(nameof(GetClassById), new { id = createdClass.Id }, createdClass);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating class");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update a class
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<ClassDto>> UpdateClass(long id, [FromBody] UpdateClassDto updateClassDto)
    {
        try
        {
            var updatedClass = await _classService.UpdateClassAsync(id, updateClassDto);
            if (updatedClass == null)
                return NotFound($"Class with ID {id} not found");

            return Ok(updatedClass);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating class with ID {ClassId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a class (soft delete if has reservations, hard delete if no reservations)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteClass(long id)
    {
        try
        {
            var result = await _classService.DeleteClassAsync(id);
            if (!result)
                return NotFound($"Class with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting class with ID {ClassId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Check for instructor schedule conflicts
    /// </summary>
    [HttpGet("instructor/{instructorId}/conflicts")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<bool>> CheckInstructorConflicts(
        long instructorId,
        [FromQuery] DateOnly date,
        [FromQuery] TimeOnly startTime,
        [FromQuery] TimeOnly endTime,
        [FromQuery] long? excludeClassId = null)
    {
        try
        {
            var hasConflict = await _classService.HasScheduleConflictAsync(instructorId, date, startTime, endTime, excludeClassId);
            return Ok(new { hasConflict });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking instructor conflicts");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Check for zone booking conflicts
    /// </summary>
    [HttpGet("zone/{zoneId}/conflicts")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<bool>> CheckZoneConflicts(
        long zoneId,
        [FromQuery] DateOnly date,
        [FromQuery] TimeOnly startTime,
        [FromQuery] TimeOnly endTime,
        [FromQuery] long? excludeClassId = null)
    {
        try
        {
            var hasConflict = await _classService.HasZoneConflictAsync(zoneId, date, startTime, endTime, excludeClassId);
            return Ok(new { hasConflict });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking zone conflicts");
            return StatusCode(500, "Internal server error");
        }
    }
}
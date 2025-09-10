using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PilatesStudioAPI.Models.DTOs.Users;
using PilatesStudioAPI.Services.Interfaces;
using System.Security.Claims;

namespace PilatesStudioAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InstructorsController : ControllerBase
{
    private readonly IInstructorService _instructorService;
    private readonly ILogger<InstructorsController> _logger;

    public InstructorsController(IInstructorService instructorService, ILogger<InstructorsController> logger)
    {
        _instructorService = instructorService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todos los instructores
    /// </summary>
    /// <returns>Lista de instructores</returns>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var instructors = await _instructorService.GetAllAsync();
            return Ok(instructors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all instructors");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtener instructores activos
    /// </summary>
    /// <returns>Lista de instructores activos</returns>
    [HttpGet("active")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> GetActive()
    {
        try
        {
            var instructors = await _instructorService.GetActiveInstructorsAsync();
            return Ok(instructors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active instructors");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtener instructor por ID
    /// </summary>
    /// <param name="id">ID del instructor</param>
    /// <returns>Información del instructor</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "instructor" && userId != null)
            {
                var currentInstructor = await _instructorService.GetByUserIdAsync(long.Parse(userId));
                if (currentInstructor?.Id != id)
                {
                    return Forbid();
                }
            }

            var instructor = await _instructorService.GetByIdAsync(id);
            if (instructor == null)
            {
                return NotFound(new { message = "Instructor no encontrado" });
            }

            return Ok(instructor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting instructor with ID: {InstructorId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtener perfil del instructor autenticado
    /// </summary>
    /// <returns>Información del instructor</returns>
    [HttpGet("me")]
    [Authorize(Roles = "instructor")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var instructor = await _instructorService.GetByUserIdAsync(userId);
            if (instructor == null)
            {
                return NotFound(new { message = "Perfil de instructor no encontrado" });
            }

            return Ok(instructor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting instructor profile");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crear nuevo instructor
    /// </summary>
    /// <param name="createDto">Datos del instructor</param>
    /// <returns>Instructor creado</returns>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] InstructorCreateDto createDto)
    {
        try
        {
            var instructor = await _instructorService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = instructor.Id }, instructor);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Error creating instructor: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating instructor");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualizar instructor
    /// </summary>
    /// <param name="id">ID del instructor</param>
    /// <param name="updateDto">Datos a actualizar</param>
    /// <returns>Instructor actualizado</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> Update(long id, [FromBody] InstructorUpdateDto updateDto)
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "instructor" && userId != null)
            {
                var currentInstructor = await _instructorService.GetByUserIdAsync(long.Parse(userId));
                if (currentInstructor?.Id != id)
                {
                    return Forbid();
                }
            }

            var instructor = await _instructorService.UpdateAsync(id, updateDto);
            if (instructor == null)
            {
                return NotFound(new { message = "Instructor no encontrado" });
            }

            return Ok(instructor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating instructor with ID: {InstructorId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Eliminar instructor
    /// </summary>
    /// <param name="id">ID del instructor</param>
    /// <returns>Confirmación de eliminación</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var result = await _instructorService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Instructor no encontrado" });
            }

            return Ok(new { message = "Instructor eliminado exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Error deleting instructor: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting instructor with ID: {InstructorId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Activar instructor
    /// </summary>
    /// <param name="id">ID del instructor</param>
    /// <returns>Confirmación de activación</returns>
    [HttpPost("{id}/activate")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Activate(long id)
    {
        try
        {
            var result = await _instructorService.ActivateAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Instructor no encontrado" });
            }

            return Ok(new { message = "Instructor activado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating instructor with ID: {InstructorId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Desactivar instructor
    /// </summary>
    /// <param name="id">ID del instructor</param>
    /// <returns>Confirmación de desactivación</returns>
    [HttpPost("{id}/deactivate")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Deactivate(long id)
    {
        try
        {
            var result = await _instructorService.DeactivateAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Instructor no encontrado" });
            }

            return Ok(new { message = "Instructor desactivado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating instructor with ID: {InstructorId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
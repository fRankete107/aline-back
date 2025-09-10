using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PilatesStudioAPI.Models.DTOs.Users;
using PilatesStudioAPI.Services.Interfaces;
using System.Security.Claims;

namespace PilatesStudioAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todos los estudiantes
    /// </summary>
    /// <returns>Lista de estudiantes</returns>
    [HttpGet]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var students = await _studentService.GetAllAsync();
            return Ok(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all students");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Buscar estudiantes
    /// </summary>
    /// <param name="q">Término de búsqueda</param>
    /// <returns>Lista de estudiantes filtrados</returns>
    [HttpGet("search")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> Search([FromQuery] string? q)
    {
        try
        {
            var students = await _studentService.SearchAsync(q ?? string.Empty);
            return Ok(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching students with term: {SearchTerm}", q);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtener estudiante por ID
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <returns>Información del estudiante</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "student" && userId != null)
            {
                var currentStudent = await _studentService.GetByUserIdAsync(long.Parse(userId));
                if (currentStudent?.Id != id)
                {
                    return Forbid();
                }
            }

            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound(new { message = "Estudiante no encontrado" });
            }

            return Ok(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student with ID: {StudentId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtener perfil del estudiante autenticado
    /// </summary>
    /// <returns>Información del estudiante</returns>
    [HttpGet("me")]
    [Authorize(Roles = "student")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
            {
                return NotFound(new { message = "Perfil de estudiante no encontrado" });
            }

            return Ok(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student profile");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crear nuevo estudiante
    /// </summary>
    /// <param name="createDto">Datos del estudiante</param>
    /// <returns>Estudiante creado</returns>
    [HttpPost]
    [Authorize(Policy = "InstructorOnly")]
    public async Task<IActionResult> Create([FromBody] StudentCreateDto createDto)
    {
        try
        {
            var student = await _studentService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Error creating student: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualizar estudiante
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <param name="updateDto">Datos a actualizar</param>
    /// <returns>Estudiante actualizado</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<IActionResult> Update(long id, [FromBody] StudentUpdateDto updateDto)
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "student" && userId != null)
            {
                var currentStudent = await _studentService.GetByUserIdAsync(long.Parse(userId));
                if (currentStudent?.Id != id)
                {
                    return Forbid();
                }
            }

            var student = await _studentService.UpdateAsync(id, updateDto);
            if (student == null)
            {
                return NotFound(new { message = "Estudiante no encontrado" });
            }

            return Ok(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student with ID: {StudentId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Eliminar estudiante
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <returns>Confirmación de eliminación</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var result = await _studentService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Estudiante no encontrado" });
            }

            return Ok(new { message = "Estudiante eliminado exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Error deleting student: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student with ID: {StudentId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
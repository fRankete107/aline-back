using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PilatesStudioAPI.Models.DTOs.Auth;
using PilatesStudioAPI.Services.Interfaces;
using System.Security.Claims;

namespace PilatesStudioAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Iniciar sesión
    /// </summary>
    /// <param name="request">Datos de login</param>
    /// <returns>Token de acceso y información del usuario</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Login failed for {Email}: {Message}", request.Email, ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", request.Email);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Registrar nuevo usuario
    /// </summary>
    /// <param name="request">Datos de registro</param>
    /// <returns>Token de acceso y información del usuario</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Registration failed for {Email}: {Message}", request.Email, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for {Email}", request.Email);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Refrescar token de acceso
    /// </summary>
    /// <param name="request">Token de actualización</param>
    /// <returns>Nuevo token de acceso</returns>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Refresh token failed: {Message}", ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Cambiar contraseña del usuario autenticado
    /// </summary>
    /// <param name="request">Contraseña actual y nueva</param>
    /// <returns>Confirmación del cambio</returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var result = await _authService.ChangePasswordAsync(userId, request);
            if (result)
            {
                return Ok(new { message = "Contraseña cambiada exitosamente" });
            }

            return BadRequest(new { message = "No se pudo cambiar la contraseña" });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Change password failed: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Change password failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password change");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Cerrar sesión
    /// </summary>
    /// <returns>Confirmación del logout</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var result = await _authService.LogoutAsync(userId);
            if (result)
            {
                return Ok(new { message = "Sesión cerrada exitosamente" });
            }

            return BadRequest(new { message = "No se pudo cerrar la sesión" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Verificar email (por implementar)
    /// </summary>
    /// <param name="token">Token de verificación</param>
    /// <returns>Confirmación de verificación</returns>
    [HttpPost("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        try
        {
            var result = await _authService.VerifyEmailAsync(token);
            if (result)
            {
                return Ok(new { message = "Email verificado exitosamente" });
            }

            return BadRequest(new { message = "Token de verificación inválido" });
        }
        catch (NotImplementedException)
        {
            return StatusCode(501, new { message = "Funcionalidad no implementada aún" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email verification");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Solicitar restablecimiento de contraseña (por implementar)
    /// </summary>
    /// <param name="request">Email del usuario</param>
    /// <returns>Confirmación del envío</returns>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        try
        {
            var result = await _authService.ForgotPasswordAsync(request.Email);
            if (result)
            {
                return Ok(new { message = "Se ha enviado un email con instrucciones para restablecer la contraseña" });
            }

            return BadRequest(new { message = "No se pudo procesar la solicitud" });
        }
        catch (NotImplementedException)
        {
            return StatusCode(501, new { message = "Funcionalidad no implementada aún" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Restablecer contraseña (por implementar)
    /// </summary>
    /// <param name="request">Token y nueva contraseña</param>
    /// <returns>Confirmación del restablecimiento</returns>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        try
        {
            var result = await _authService.ResetPasswordAsync(request.Token, request.Password);
            if (result)
            {
                return Ok(new { message = "Contraseña restablecida exitosamente" });
            }

            return BadRequest(new { message = "Token inválido o expirado" });
        }
        catch (NotImplementedException)
        {
            return StatusCode(501, new { message = "Funcionalidad no implementada aún" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtener información del usuario autenticado
    /// </summary>
    /// <returns>Información del usuario</returns>
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var userInfo = new
            {
                Id = userIdClaim,
                Email = emailClaim,
                Role = roleClaim,
                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            };

            return Ok(userInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user info");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
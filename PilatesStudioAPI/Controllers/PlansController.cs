using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlansController : ControllerBase
{
    private readonly IPlanService _planService;
    private readonly ILogger<PlansController> _logger;

    public PlansController(IPlanService planService, ILogger<PlansController> logger)
    {
        _planService = planService;
        _logger = logger;
    }

    /// <summary>
    /// Get all plans
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlanDto>>> GetAllPlans()
    {
        try
        {
            var plans = await _planService.GetAllPlansAsync();
            return Ok(plans);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all plans");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get active plans only
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<PlanDto>>> GetActivePlans()
    {
        try
        {
            var plans = await _planService.GetActivePlansAsync();
            return Ok(plans);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active plans");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get plan by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PlanDto>> GetPlanById(long id)
    {
        try
        {
            var plan = await _planService.GetPlanByIdAsync(id);
            if (plan == null)
                return NotFound($"Plan with ID {id} not found");

            return Ok(plan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting plan with ID {PlanId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new plan
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<PlanDto>> CreatePlan([FromBody] CreatePlanDto createPlanDto)
    {
        try
        {
            var createdPlan = await _planService.CreatePlanAsync(createPlanDto);
            return CreatedAtAction(nameof(GetPlanById), new { id = createdPlan.Id }, createdPlan);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating plan");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update a plan
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<PlanDto>> UpdatePlan(long id, [FromBody] UpdatePlanDto updatePlanDto)
    {
        try
        {
            var updatedPlan = await _planService.UpdatePlanAsync(id, updatePlanDto);
            if (updatedPlan == null)
                return NotFound($"Plan with ID {id} not found");

            return Ok(updatedPlan);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating plan with ID {PlanId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a plan (soft delete if has active subscriptions, hard delete if no subscriptions)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeletePlan(long id)
    {
        try
        {
            var result = await _planService.DeletePlanAsync(id);
            if (!result)
                return NotFound($"Plan with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plan with ID {PlanId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Check if plan has active subscriptions
    /// </summary>
    [HttpGet("{id}/active-subscriptions")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<bool>> HasActiveSubscriptions(long id)
    {
        try
        {
            if (!await _planService.PlanExistsAsync(id))
                return NotFound($"Plan with ID {id} not found");

            var hasActiveSubscriptions = await _planService.HasActiveSubscriptionsAsync(id);
            return Ok(new { hasActiveSubscriptions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking active subscriptions for plan with ID {PlanId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly ILogger<SubscriptionsController> _logger;

    public SubscriptionsController(ISubscriptionService subscriptionService, ILogger<SubscriptionsController> logger)
    {
        _subscriptionService = subscriptionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all subscriptions with optional filtering
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "admin,instructor")]
    public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetSubscriptions([FromQuery] SubscriptionFilterDto? filter = null)
    {
        try
        {
            IEnumerable<SubscriptionDto> subscriptions;
            
            if (filter != null && (filter.StudentId.HasValue || filter.PlanId.HasValue || 
                !string.IsNullOrEmpty(filter.Status) || filter.StartDate.HasValue || 
                filter.EndDate.HasValue || filter.ExpiringSoon.HasValue || 
                filter.HasClassesRemaining.HasValue))
            {
                subscriptions = await _subscriptionService.GetFilteredSubscriptionsAsync(filter);
            }
            else
            {
                subscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
            }

            return Ok(subscriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subscriptions");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get active subscriptions
    /// </summary>
    [HttpGet("active")]
    [Authorize(Roles = "admin,instructor")]
    public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetActiveSubscriptions()
    {
        try
        {
            var subscriptions = await _subscriptionService.GetActiveSubscriptionsAsync();
            return Ok(subscriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active subscriptions");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get expired subscriptions
    /// </summary>
    [HttpGet("expired")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetExpiredSubscriptions()
    {
        try
        {
            var subscriptions = await _subscriptionService.GetExpiredSubscriptionsAsync();
            return Ok(subscriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expired subscriptions");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get subscriptions expiring soon
    /// </summary>
    [HttpGet("expiring-soon")]
    [Authorize(Roles = "admin,instructor")]
    public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetExpiringSoon([FromQuery] int daysThreshold = 7)
    {
        try
        {
            var subscriptions = await _subscriptionService.GetExpiringSoonAsync(daysThreshold);
            return Ok(subscriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expiring subscriptions");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get subscription by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SubscriptionDto>> GetSubscriptionById(long id)
    {
        try
        {
            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
            if (subscription == null)
                return NotFound($"Subscription with ID {id} not found");

            return Ok(subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subscription with ID {SubscriptionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get subscriptions by student
    /// </summary>
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetSubscriptionsByStudent(long studentId)
    {
        try
        {
            var subscriptions = await _subscriptionService.GetSubscriptionsByStudentAsync(studentId);
            return Ok(subscriptions);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subscriptions by student {StudentId}", studentId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get active subscription by student
    /// </summary>
    [HttpGet("student/{studentId}/active")]
    public async Task<ActionResult<SubscriptionDto>> GetActiveSubscriptionByStudent(long studentId)
    {
        try
        {
            var subscription = await _subscriptionService.GetActiveSubscriptionByStudentAsync(studentId);
            if (subscription == null)
                return NotFound($"No active subscription found for student {studentId}");

            return Ok(subscription);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active subscription by student {StudentId}", studentId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get subscriptions by plan
    /// </summary>
    [HttpGet("plan/{planId}")]
    [Authorize(Roles = "admin,instructor")]
    public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetSubscriptionsByPlan(long planId)
    {
        try
        {
            var subscriptions = await _subscriptionService.GetSubscriptionsByPlanAsync(planId);
            return Ok(subscriptions);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subscriptions by plan {PlanId}", planId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new subscription
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<SubscriptionDto>> CreateSubscription([FromBody] CreateSubscriptionDto createSubscriptionDto)
    {
        try
        {
            var createdSubscription = await _subscriptionService.CreateSubscriptionAsync(createSubscriptionDto);
            return CreatedAtAction(nameof(GetSubscriptionById), new { id = createdSubscription.Id }, createdSubscription);
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
            _logger.LogError(ex, "Error creating subscription");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update a subscription
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<SubscriptionDto>> UpdateSubscription(long id, [FromBody] UpdateSubscriptionDto updateSubscriptionDto)
    {
        try
        {
            var updatedSubscription = await _subscriptionService.UpdateSubscriptionAsync(id, updateSubscriptionDto);
            if (updatedSubscription == null)
                return NotFound($"Subscription with ID {id} not found");

            return Ok(updatedSubscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating subscription with ID {SubscriptionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Renew a subscription
    /// </summary>
    [HttpPost("{id}/renew")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<SubscriptionDto>> RenewSubscription(long id, [FromBody] RenewSubscriptionDto renewSubscriptionDto)
    {
        try
        {
            var renewedSubscription = await _subscriptionService.RenewSubscriptionAsync(id, renewSubscriptionDto);
            return Ok(renewedSubscription);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error renewing subscription with ID {SubscriptionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a subscription (soft delete if has reservations, hard delete if no reservations)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteSubscription(long id)
    {
        try
        {
            var result = await _subscriptionService.DeleteSubscriptionAsync(id);
            if (!result)
                return NotFound($"Subscription with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting subscription with ID {SubscriptionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Check if student can reserve classes (has active subscription with remaining classes)
    /// </summary>
    [HttpGet("student/{studentId}/can-reserve")]
    public async Task<ActionResult<bool>> CanStudentReserveClass(long studentId)
    {
        try
        {
            var canReserve = await _subscriptionService.CanStudentReserveClassAsync(studentId);
            return Ok(new { canReserve });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if student {StudentId} can reserve class", studentId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Process expired subscriptions (admin maintenance task)
    /// </summary>
    [HttpPost("process-expired")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<int>> ProcessExpiredSubscriptions()
    {
        try
        {
            var processedCount = await _subscriptionService.ProcessExpiredSubscriptionsAsync();
            return Ok(new { processedCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing expired subscriptions");
            return StatusCode(500, "Internal server error");
        }
    }
}
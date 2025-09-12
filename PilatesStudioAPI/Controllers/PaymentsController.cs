using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IValidator<CreatePaymentDto> _createPaymentValidator;
    private readonly IValidator<UpdatePaymentDto> _updatePaymentValidator;
    private readonly IValidator<PaymentFilterDto> _filterValidator;
    private readonly IValidator<RefundPaymentDto> _refundValidator;

    public PaymentsController(
        IPaymentService paymentService,
        IValidator<CreatePaymentDto> createPaymentValidator,
        IValidator<UpdatePaymentDto> updatePaymentValidator,
        IValidator<PaymentFilterDto> filterValidator,
        IValidator<RefundPaymentDto> refundValidator)
    {
        _paymentService = paymentService;
        _createPaymentValidator = createPaymentValidator;
        _updatePaymentValidator = updatePaymentValidator;
        _filterValidator = filterValidator;
        _refundValidator = refundValidator;
    }

    /// <summary>
    /// Get all payments (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAllPayments()
    {
        var payments = await _paymentService.GetAllPaymentsAsync();
        return Ok(payments);
    }

    /// <summary>
    /// Get payment by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetPayment(long id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment == null)
            return NotFound($"Payment with ID {id} not found");

        return Ok(payment);
    }

    /// <summary>
    /// Create a new payment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PaymentDto>> CreatePayment(CreatePaymentDto createPaymentDto)
    {
        var validationResult = await _createPaymentValidator.ValidateAsync(createPaymentDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        try
        {
            var payment = await _paymentService.CreatePaymentAsync(createPaymentDto);
            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Update payment details
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin,instructor")]
    public async Task<ActionResult<PaymentDto>> UpdatePayment(long id, UpdatePaymentDto updatePaymentDto)
    {
        var validationResult = await _updatePaymentValidator.ValidateAsync(updatePaymentDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        try
        {
            var payment = await _paymentService.UpdatePaymentAsync(id, updatePaymentDto);
            if (payment == null)
                return NotFound($"Payment with ID {id} not found");

            return Ok(payment);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Delete a payment (Admin only - only pending/failed payments)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeletePayment(long id)
    {
        try
        {
            var success = await _paymentService.DeletePaymentAsync(id);
            if (!success)
                return NotFound($"Payment with ID {id} not found");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Get filtered payments
    /// </summary>
    [HttpPost("filter")]
    [Authorize(Roles = "admin,instructor")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetFilteredPayments(PaymentFilterDto filter)
    {
        var validationResult = await _filterValidator.ValidateAsync(filter);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var payments = await _paymentService.GetFilteredPaymentsAsync(filter);
        return Ok(payments);
    }

    /// <summary>
    /// Get payments by student
    /// </summary>
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByStudent(long studentId)
    {
        try
        {
            var payments = await _paymentService.GetPaymentsByStudentAsync(studentId);
            return Ok(payments);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Get payments by plan (Admin/Instructor only)
    /// </summary>
    [HttpGet("plan/{planId}")]
    [Authorize(Roles = "admin,instructor")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByPlan(long planId)
    {
        try
        {
            var payments = await _paymentService.GetPaymentsByPlanAsync(planId);
            return Ok(payments);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Get payments by date range (Admin/Instructor only)
    /// </summary>
    [HttpGet("date-range")]
    [Authorize(Roles = "admin,instructor")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByDateRange(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        try
        {
            var payments = await _paymentService.GetPaymentsByDateRangeAsync(startDate, endDate);
            return Ok(payments);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get payments by status (Admin/Instructor only)
    /// </summary>
    [HttpGet("status/{status}")]
    [Authorize(Roles = "admin,instructor")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByStatus(string status)
    {
        try
        {
            var payments = await _paymentService.GetPaymentsByStatusAsync(status);
            return Ok(payments);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get pending payments (Admin only)
    /// </summary>
    [HttpGet("pending")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPendingPayments()
    {
        var payments = await _paymentService.GetPendingPaymentsAsync();
        return Ok(payments);
    }

    /// <summary>
    /// Get refundable payments (Admin only)
    /// </summary>
    [HttpGet("refundable")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetRefundablePayments([FromQuery] int daysLimit = 30)
    {
        var payments = await _paymentService.GetRefundablePaymentsAsync(daysLimit);
        return Ok(payments);
    }

    /// <summary>
    /// Get payment statistics (Admin only)
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<PaymentStatsDto>> GetPaymentStats(
        [FromQuery] DateTime? startDate = null, 
        [FromQuery] DateTime? endDate = null)
    {
        var stats = await _paymentService.GetPaymentStatsAsync(startDate, endDate);
        return Ok(stats);
    }

    /// <summary>
    /// Process payment (Admin only)
    /// </summary>
    [HttpPost("{id}/process")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<PaymentDto>> ProcessPayment(long id, [FromBody] ProcessPaymentDto processDto)
    {
        if (string.IsNullOrEmpty(processDto.Status))
            return BadRequest("Status is required");

        try
        {
            var payment = await _paymentService.ProcessPaymentAsync(id, processDto.Status, processDto.Reference);
            return Ok(payment);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Refund payment (Admin only)
    /// </summary>
    [HttpPost("{id}/refund")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<PaymentDto>> RefundPayment(long id, RefundPaymentDto refundDto)
    {
        var validationResult = await _refundValidator.ValidateAsync(refundDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        try
        {
            var payment = await _paymentService.RefundPaymentAsync(id, refundDto);
            return Ok(payment);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Get student payment history
    /// </summary>
    [HttpGet("student/{studentId}/history")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentHistory(long studentId, [FromQuery] int limit = 10)
    {
        try
        {
            var payments = await _paymentService.GetPaymentHistoryAsync(studentId, limit);
            return Ok(payments);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Get total amount paid by student
    /// </summary>
    [HttpGet("student/{studentId}/total")]
    public async Task<ActionResult<decimal>> GetTotalAmountByStudent(long studentId)
    {
        try
        {
            var total = await _paymentService.GetTotalAmountByStudentAsync(studentId);
            return Ok(new { StudentId = studentId, TotalAmount = total });
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Get payments by payment method (Admin only)
    /// </summary>
    [HttpGet("method/{paymentMethod}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByMethod(string paymentMethod)
    {
        try
        {
            var payments = await _paymentService.GetPaymentsByMethodAsync(paymentMethod);
            return Ok(payments);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Create payment for subscription (simplified endpoint)
    /// </summary>
    [HttpPost("subscription")]
    public async Task<ActionResult<PaymentDto>> CreatePaymentForSubscription(CreateSubscriptionPaymentDto subscriptionPaymentDto)
    {
        try
        {
            var payment = await _paymentService.CreatePaymentForSubscriptionAsync(
                subscriptionPaymentDto.StudentId,
                subscriptionPaymentDto.PlanId,
                subscriptionPaymentDto.PaymentMethod,
                subscriptionPaymentDto.PaymentReference);

            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Process pending payments (Admin only - batch operation)
    /// </summary>
    [HttpPost("process-pending")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> ProcessPendingPayments()
    {
        var processedCount = await _paymentService.ProcessPendingPaymentsAsync();
        return Ok(new { ProcessedCount = processedCount, Message = $"Processed {processedCount} pending payments" });
    }

    /// <summary>
    /// Check if payment exists
    /// </summary>
    [HttpHead("{id}")]
    public async Task<ActionResult> PaymentExists(long id)
    {
        var exists = await _paymentService.PaymentExistsAsync(id);
        return exists ? Ok() : NotFound();
    }
}

// Additional DTOs for specific endpoints
public class ProcessPaymentDto
{
    public string Status { get; set; } = string.Empty;
    public string? Reference { get; set; }
}

public class CreateSubscriptionPaymentDto
{
    public long StudentId { get; set; }
    public long PlanId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? PaymentReference { get; set; }
}
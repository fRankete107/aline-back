using AutoMapper;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Repositories.Interfaces;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IPlanRepository _planRepository;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IMapper _mapper;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IStudentRepository studentRepository,
        IPlanRepository planRepository,
        ISubscriptionService subscriptionService,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _studentRepository = studentRepository;
        _planRepository = planRepository;
        _subscriptionService = subscriptionService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync()
    {
        var payments = await _paymentRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<PaymentDto?> GetPaymentByIdAsync(long id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        return payment == null ? null : _mapper.Map<PaymentDto>(payment);
    }

    public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto)
    {
        await ValidatePaymentCreationAsync(createPaymentDto);

        var payment = _mapper.Map<Payment>(createPaymentDto);
        if (!createPaymentDto.PaymentDate.HasValue)
            payment.PaymentDate = DateTime.UtcNow;

        var createdPayment = await _paymentRepository.CreateAsync(payment);
        return _mapper.Map<PaymentDto>(createdPayment);
    }

    public async Task<PaymentDto?> UpdatePaymentAsync(long id, UpdatePaymentDto updatePaymentDto)
    {
        var existingPayment = await _paymentRepository.GetByIdAsync(id);
        if (existingPayment == null)
            return null;

        // Validate status transitions
        if (!string.IsNullOrEmpty(updatePaymentDto.Status) && 
            !IsValidStatusTransition(existingPayment.Status, updatePaymentDto.Status))
        {
            throw new InvalidOperationException($"Cannot change payment status from {existingPayment.Status} to {updatePaymentDto.Status}");
        }

        var payment = _mapper.Map<Payment>(updatePaymentDto);
        var updatedPayment = await _paymentRepository.UpdateAsync(id, payment);
        return updatedPayment == null ? null : _mapper.Map<PaymentDto>(updatedPayment);
    }

    public async Task<bool> DeletePaymentAsync(long id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null)
            return false;

        // Only allow deletion of pending or failed payments
        if (payment.Status == "completed" || payment.Status == "refunded")
            throw new InvalidOperationException("Cannot delete completed or refunded payments");

        return await _paymentRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<PaymentDto>> GetFilteredPaymentsAsync(PaymentFilterDto filter)
    {
        var payments = await _paymentRepository.GetFilteredAsync(filter);
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentsByStudentAsync(long studentId)
    {
        if (!await _studentRepository.ExistsAsync(studentId))
            throw new ArgumentException($"Student with ID {studentId} not found.");

        var payments = await _paymentRepository.GetByStudentAsync(studentId);
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentsByPlanAsync(long planId)
    {
        if (!await _planRepository.ExistsAsync(planId))
            throw new ArgumentException($"Plan with ID {planId} not found.");

        var payments = await _paymentRepository.GetByPlanAsync(planId);
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be less than or equal to end date.");

        var payments = await _paymentRepository.GetByDateRangeAsync(startDate, endDate);
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentsByStatusAsync(string status)
    {
        var validStatuses = new[] { "pending", "completed", "failed", "refunded" };
        if (!validStatuses.Contains(status.ToLower()))
            throw new ArgumentException("Invalid payment status.");

        var payments = await _paymentRepository.GetByStatusAsync(status);
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<IEnumerable<PaymentDto>> GetPendingPaymentsAsync()
    {
        var payments = await _paymentRepository.GetPendingPaymentsAsync();
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<IEnumerable<PaymentDto>> GetRefundablePaymentsAsync(int daysLimit = 30)
    {
        var payments = await _paymentRepository.GetRefundablePaymentsAsync(daysLimit);
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<PaymentStatsDto> GetPaymentStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        return await _paymentRepository.GetPaymentStatsAsync(startDate, endDate);
    }

    public async Task<PaymentDto> ProcessPaymentAsync(long paymentId, string status, string? reference = null)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);
        if (payment == null)
            throw new ArgumentException($"Payment with ID {paymentId} not found.");

        if (payment.Status != "pending")
            throw new InvalidOperationException("Only pending payments can be processed.");

        var validStatuses = new[] { "completed", "failed" };
        if (!validStatuses.Contains(status.ToLower()))
            throw new ArgumentException("Invalid payment status for processing.");

        var success = await _paymentRepository.ProcessPaymentAsync(paymentId, status, reference);
        if (!success)
            throw new InvalidOperationException("Failed to process payment.");

        // If payment is completed and for a plan, create or extend subscription
        if (status == "completed")
        {
            try
            {
                await _subscriptionService.ProcessPaymentSubscriptionAsync(payment.StudentId, payment.PlanId);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the payment processing
                // In a real application, you might want to implement a retry mechanism
                Console.WriteLine($"Error processing subscription for payment {paymentId}: {ex.Message}");
            }
        }

        var updatedPayment = await _paymentRepository.GetByIdAsync(paymentId);
        return _mapper.Map<PaymentDto>(updatedPayment!);
    }

    public async Task<PaymentDto> RefundPaymentAsync(long paymentId, RefundPaymentDto refundDto)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);
        if (payment == null)
            throw new ArgumentException($"Payment with ID {paymentId} not found.");

        if (payment.Status != "completed")
            throw new InvalidOperationException("Only completed payments can be refunded.");

        if (!await CanRefundPaymentAsync(paymentId))
            throw new InvalidOperationException("Payment is not eligible for refund (exceeded time limit).");

        var refundAmount = refundDto.RefundAmount ?? payment.Amount;
        if (refundAmount > payment.Amount)
            throw new InvalidOperationException("Refund amount cannot exceed original payment amount.");

        var success = await _paymentRepository.RefundPaymentAsync(paymentId, refundAmount, refundDto.Reason);
        if (!success)
            throw new InvalidOperationException("Failed to process refund.");

        var updatedPayment = await _paymentRepository.GetByIdAsync(paymentId);
        return _mapper.Map<PaymentDto>(updatedPayment!);
    }

    public async Task<bool> PaymentExistsAsync(long id)
    {
        return await _paymentRepository.ExistsAsync(id);
    }

    public async Task<bool> CanCreatePaymentAsync(CreatePaymentDto createPaymentDto)
    {
        // Check if student exists
        if (!await _studentRepository.ExistsAsync(createPaymentDto.StudentId))
            return false;

        // Check if plan exists
        var plan = await _planRepository.GetByIdAsync(createPaymentDto.PlanId);
        if (plan == null)
            return false;

        // Validate payment amount matches plan price (with some tolerance for promotions)
        if (!await ValidatePaymentAmountAsync(createPaymentDto.PlanId, createPaymentDto.Amount))
            return false;

        return true;
    }

    public async Task<bool> CanRefundPaymentAsync(long paymentId)
    {
        var refundablePayments = await _paymentRepository.GetRefundablePaymentsAsync();
        return refundablePayments.Any(p => p.Id == paymentId);
    }

    public async Task<decimal> GetTotalAmountByStudentAsync(long studentId)
    {
        if (!await _studentRepository.ExistsAsync(studentId))
            throw new ArgumentException($"Student with ID {studentId} not found.");

        return await _paymentRepository.GetTotalAmountByStudentAsync(studentId);
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentHistoryAsync(long studentId, int limit = 10)
    {
        if (!await _studentRepository.ExistsAsync(studentId))
            throw new ArgumentException($"Student with ID {studentId} not found.");

        var payments = await _paymentRepository.GetPaymentHistoryAsync(studentId, limit);
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<PaymentDto> CreatePaymentForSubscriptionAsync(long studentId, long planId, string paymentMethod, string? paymentReference = null)
    {
        var plan = await _planRepository.GetByIdAsync(planId);
        if (plan == null)
            throw new ArgumentException($"Plan with ID {planId} not found.");

        var createPaymentDto = new CreatePaymentDto
        {
            StudentId = studentId,
            PlanId = planId,
            Amount = plan.Price,
            PaymentMethod = paymentMethod,
            PaymentReference = paymentReference,
            PaymentDate = DateTime.UtcNow
        };

        return await CreatePaymentAsync(createPaymentDto);
    }

    public async Task<bool> ValidatePaymentAmountAsync(long planId, decimal amount)
    {
        var plan = await _planRepository.GetByIdAsync(planId);
        if (plan == null)
            return false;

        // Allow some tolerance (e.g., 5%) for promotional prices or discounts
        var tolerance = plan.Price * 0.05m;
        var minAmount = plan.Price - tolerance;
        var maxAmount = plan.Price + tolerance;

        return amount >= minAmount && amount <= maxAmount;
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentsByMethodAsync(string paymentMethod)
    {
        var validMethods = new[] { "cash", "credit_card", "debit_card", "bank_transfer", "digital_wallet" };
        if (!validMethods.Contains(paymentMethod.ToLower()))
            throw new ArgumentException("Invalid payment method.");

        var payments = await _paymentRepository.GetPaymentsByMethodAsync(paymentMethod);
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<int> ProcessPendingPaymentsAsync()
    {
        var pendingPayments = await _paymentRepository.GetPendingPaymentsAsync();
        int processedCount = 0;

        foreach (var payment in pendingPayments)
        {
            // In a real application, you would integrate with payment gateways here
            // For now, we'll just simulate processing based on payment method
            try
            {
                var status = SimulatePaymentProcessing(payment.PaymentMethod);
                await _paymentRepository.UpdatePaymentStatusAsync(payment.Id, status);
                
                if (status == "completed")
                {
                    await _subscriptionService.ProcessPaymentSubscriptionAsync(payment.StudentId, payment.PlanId);
                }
                
                processedCount++;
            }
            catch (Exception ex)
            {
                // Log error and continue with next payment
                Console.WriteLine($"Error processing payment {payment.Id}: {ex.Message}");
                await _paymentRepository.UpdatePaymentStatusAsync(payment.Id, "failed");
            }
        }

        return processedCount;
    }

    private async Task ValidatePaymentCreationAsync(CreatePaymentDto createPaymentDto)
    {
        if (!await _studentRepository.ExistsAsync(createPaymentDto.StudentId))
            throw new ArgumentException($"Student with ID {createPaymentDto.StudentId} not found.");

        if (!await _planRepository.ExistsAsync(createPaymentDto.PlanId))
            throw new ArgumentException($"Plan with ID {createPaymentDto.PlanId} not found.");

        if (!await ValidatePaymentAmountAsync(createPaymentDto.PlanId, createPaymentDto.Amount))
            throw new ArgumentException("Payment amount does not match plan price.");
    }

    private static bool IsValidStatusTransition(string currentStatus, string newStatus)
    {
        var validTransitions = new Dictionary<string, string[]>
        {
            ["pending"] = ["completed", "failed"],
            ["completed"] = ["refunded"],
            ["failed"] = ["pending"], // Allow retry
            ["refunded"] = [] // Terminal state
        };

        return validTransitions.ContainsKey(currentStatus) && 
               validTransitions[currentStatus].Contains(newStatus);
    }

    private static string SimulatePaymentProcessing(string paymentMethod)
    {
        // Simulate payment processing - in reality this would integrate with payment gateways
        var random = new Random();
        var successRate = paymentMethod switch
        {
            "cash" => 0.95, // Cash payments are almost always successful
            "credit_card" => 0.85,
            "debit_card" => 0.90,
            "bank_transfer" => 0.92,
            "digital_wallet" => 0.88,
            _ => 0.80
        };

        return random.NextDouble() < successRate ? "completed" : "failed";
    }
}
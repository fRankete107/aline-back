using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Services.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync();
    Task<PaymentDto?> GetPaymentByIdAsync(long id);
    Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto);
    Task<PaymentDto?> UpdatePaymentAsync(long id, UpdatePaymentDto updatePaymentDto);
    Task<bool> DeletePaymentAsync(long id);
    Task<IEnumerable<PaymentDto>> GetFilteredPaymentsAsync(PaymentFilterDto filter);
    Task<IEnumerable<PaymentDto>> GetPaymentsByStudentAsync(long studentId);
    Task<IEnumerable<PaymentDto>> GetPaymentsByPlanAsync(long planId);
    Task<IEnumerable<PaymentDto>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<PaymentDto>> GetPaymentsByStatusAsync(string status);
    Task<IEnumerable<PaymentDto>> GetPendingPaymentsAsync();
    Task<IEnumerable<PaymentDto>> GetRefundablePaymentsAsync(int daysLimit = 30);
    Task<PaymentStatsDto> GetPaymentStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<PaymentDto> ProcessPaymentAsync(long paymentId, string status, string? reference = null);
    Task<PaymentDto> RefundPaymentAsync(long paymentId, RefundPaymentDto refundDto);
    Task<bool> PaymentExistsAsync(long id);
    Task<bool> CanCreatePaymentAsync(CreatePaymentDto createPaymentDto);
    Task<bool> CanRefundPaymentAsync(long paymentId);
    Task<decimal> GetTotalAmountByStudentAsync(long studentId);
    Task<IEnumerable<PaymentDto>> GetPaymentHistoryAsync(long studentId, int limit = 10);
    Task<PaymentDto> CreatePaymentForSubscriptionAsync(long studentId, long planId, string paymentMethod, string? paymentReference = null);
    Task<bool> ValidatePaymentAmountAsync(long planId, decimal amount);
    Task<IEnumerable<PaymentDto>> GetPaymentsByMethodAsync(string paymentMethod);
    Task<int> ProcessPendingPaymentsAsync();
}
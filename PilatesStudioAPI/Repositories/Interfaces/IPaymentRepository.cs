using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.Entities;

namespace PilatesStudioAPI.Repositories.Interfaces;

public interface IPaymentRepository
{
    Task<IEnumerable<Payment>> GetAllAsync();
    Task<Payment?> GetByIdAsync(long id);
    Task<Payment> CreateAsync(Payment payment);
    Task<Payment?> UpdateAsync(long id, Payment payment);
    Task<bool> DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
    Task<IEnumerable<Payment>> GetFilteredAsync(PaymentFilterDto filter);
    Task<IEnumerable<Payment>> GetByStudentAsync(long studentId);
    Task<IEnumerable<Payment>> GetByPlanAsync(long planId);
    Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Payment>> GetByStatusAsync(string status);
    Task<IEnumerable<Payment>> GetPendingPaymentsAsync();
    Task<IEnumerable<Payment>> GetRefundablePaymentsAsync(int daysLimit = 30);
    Task<PaymentStatsDto> GetPaymentStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> ProcessPaymentAsync(long paymentId, string status, string? reference = null);
    Task<bool> RefundPaymentAsync(long paymentId, decimal refundAmount, string reason);
    Task<bool> HasPaymentForPlanAsync(long studentId, long planId);
    Task<decimal> GetTotalAmountByStudentAsync(long studentId);
    Task<IEnumerable<Payment>> GetPaymentHistoryAsync(long studentId, int limit = 10);
    Task<bool> UpdatePaymentStatusAsync(long paymentId, string status);
    Task<IEnumerable<Payment>> GetPaymentsByMethodAsync(string paymentMethod);
}
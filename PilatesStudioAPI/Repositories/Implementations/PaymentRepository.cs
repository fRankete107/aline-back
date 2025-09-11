using Microsoft.EntityFrameworkCore;
using PilatesStudioAPI.Data.Context;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Repositories.Interfaces;

namespace PilatesStudioAPI.Repositories.Implementations;

public class PaymentRepository : IPaymentRepository
{
    private readonly PilatesStudioDbContext _context;

    public PaymentRepository(PilatesStudioDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _context.Payments
            .Include(p => p.Student)
            .Include(p => p.Plan)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<Payment?> GetByIdAsync(long id)
    {
        return await _context.Payments
            .Include(p => p.Student)
            .Include(p => p.Plan)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Payment> CreateAsync(Payment payment)
    {
        payment.CreatedAt = DateTime.UtcNow;
        payment.UpdatedAt = DateTime.UtcNow;
        
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        
        return await GetByIdAsync(payment.Id) ?? payment;
    }

    public async Task<Payment?> UpdateAsync(long id, Payment payment)
    {
        var existingPayment = await _context.Payments.FindAsync(id);
        if (existingPayment == null)
            return null;

        existingPayment.PaymentReference = payment.PaymentReference ?? existingPayment.PaymentReference;
        existingPayment.ReceiptNumber = payment.ReceiptNumber ?? existingPayment.ReceiptNumber;
        existingPayment.PaymentDate = payment.PaymentDate;
        existingPayment.Status = payment.Status ?? existingPayment.Status;
        existingPayment.Notes = payment.Notes ?? existingPayment.Notes;
        existingPayment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null)
            return false;

        _context.Payments.Remove(payment);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Payments.AnyAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Payment>> GetFilteredAsync(PaymentFilterDto filter)
    {
        var query = _context.Payments
            .Include(p => p.Student)
            .Include(p => p.Plan)
            .AsQueryable();

        if (filter.StudentId.HasValue)
            query = query.Where(p => p.StudentId == filter.StudentId.Value);

        if (filter.PlanId.HasValue)
            query = query.Where(p => p.PlanId == filter.PlanId.Value);

        if (!string.IsNullOrEmpty(filter.PaymentMethod))
            query = query.Where(p => p.PaymentMethod == filter.PaymentMethod);

        if (!string.IsNullOrEmpty(filter.Status))
            query = query.Where(p => p.Status == filter.Status);

        if (filter.StartDate.HasValue)
            query = query.Where(p => p.PaymentDate >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(p => p.PaymentDate <= filter.EndDate.Value);

        if (filter.MinAmount.HasValue)
            query = query.Where(p => p.Amount >= filter.MinAmount.Value);

        if (filter.MaxAmount.HasValue)
            query = query.Where(p => p.Amount <= filter.MaxAmount.Value);

        return await query
            .OrderByDescending(p => p.PaymentDate)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByStudentAsync(long studentId)
    {
        return await _context.Payments
            .Include(p => p.Student)
            .Include(p => p.Plan)
            .Where(p => p.StudentId == studentId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByPlanAsync(long planId)
    {
        return await _context.Payments
            .Include(p => p.Student)
            .Include(p => p.Plan)
            .Where(p => p.PlanId == planId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Payments
            .Include(p => p.Student)
            .Include(p => p.Plan)
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByStatusAsync(string status)
    {
        return await _context.Payments
            .Include(p => p.Student)
            .Include(p => p.Plan)
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetPendingPaymentsAsync()
    {
        return await GetByStatusAsync("pending");
    }

    public async Task<IEnumerable<Payment>> GetRefundablePaymentsAsync(int daysLimit = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysLimit);
        return await _context.Payments
            .Include(p => p.Student)
            .Include(p => p.Plan)
            .Where(p => p.Status == "completed" && p.PaymentDate >= cutoffDate)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<PaymentStatsDto> GetPaymentStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Payments.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(p => p.PaymentDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(p => p.PaymentDate <= endDate.Value);

        var payments = await query.ToListAsync();

        var stats = new PaymentStatsDto
        {
            TotalAmount = payments.Sum(p => p.Amount),
            TotalPayments = payments.Count,
            PendingPayments = payments.Count(p => p.Status == "pending"),
            CompletedPayments = payments.Count(p => p.Status == "completed"),
            FailedPayments = payments.Count(p => p.Status == "failed"),
            RefundedPayments = payments.Count(p => p.Status == "refunded"),
            AveragePaymentAmount = payments.Count > 0 ? payments.Average(p => p.Amount) : 0,
            PaymentMethodStats = payments
                .GroupBy(p => p.PaymentMethod)
                .ToDictionary(g => g.Key, g => g.Count()),
            MonthlyRevenue = payments
                .Where(p => p.Status == "completed")
                .GroupBy(p => p.PaymentDate.ToString("yyyy-MM"))
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount))
        };

        return stats;
    }

    public async Task<bool> ProcessPaymentAsync(long paymentId, string status, string? reference = null)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null)
            return false;

        payment.Status = status;
        if (!string.IsNullOrEmpty(reference))
            payment.PaymentReference = reference;
        payment.UpdatedAt = DateTime.UtcNow;

        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> RefundPaymentAsync(long paymentId, decimal refundAmount, string reason)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null || payment.Status != "completed")
            return false;

        payment.Status = "refunded";
        payment.Notes = (payment.Notes ?? "") + $"\nRefunded: ${refundAmount} - Reason: {reason}";
        payment.UpdatedAt = DateTime.UtcNow;

        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> HasPaymentForPlanAsync(long studentId, long planId)
    {
        return await _context.Payments
            .AnyAsync(p => p.StudentId == studentId && p.PlanId == planId && p.Status == "completed");
    }

    public async Task<decimal> GetTotalAmountByStudentAsync(long studentId)
    {
        return await _context.Payments
            .Where(p => p.StudentId == studentId && p.Status == "completed")
            .SumAsync(p => p.Amount);
    }

    public async Task<IEnumerable<Payment>> GetPaymentHistoryAsync(long studentId, int limit = 10)
    {
        return await _context.Payments
            .Include(p => p.Plan)
            .Where(p => p.StudentId == studentId)
            .OrderByDescending(p => p.PaymentDate)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> UpdatePaymentStatusAsync(long paymentId, string status)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null)
            return false;

        payment.Status = status;
        payment.UpdatedAt = DateTime.UtcNow;

        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByMethodAsync(string paymentMethod)
    {
        return await _context.Payments
            .Include(p => p.Student)
            .Include(p => p.Plan)
            .Where(p => p.PaymentMethod == paymentMethod)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }
}
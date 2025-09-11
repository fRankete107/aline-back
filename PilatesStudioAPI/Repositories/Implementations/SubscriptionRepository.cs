using Microsoft.EntityFrameworkCore;
using PilatesStudioAPI.Data.Context;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Repositories.Interfaces;

namespace PilatesStudioAPI.Repositories.Implementations;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly PilatesStudioDbContext _context;

    public SubscriptionRepository(PilatesStudioDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Subscription>> GetAllAsync()
    {
        return await _context.Subscriptions
            .Include(s => s.Student)
                .ThenInclude(st => st.User)
            .Include(s => s.Plan)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<Subscription?> GetByIdAsync(long id)
    {
        return await _context.Subscriptions
            .Include(s => s.Student)
                .ThenInclude(st => st.User)
            .Include(s => s.Plan)
            .Include(s => s.Reservations)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Subscription> CreateAsync(Subscription subscription)
    {
        subscription.CreatedAt = DateTime.UtcNow;
        subscription.UpdatedAt = DateTime.UtcNow;
        
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
        
        return await GetByIdAsync(subscription.Id) ?? subscription;
    }

    public async Task<Subscription?> UpdateAsync(long id, Subscription subscription)
    {
        var existingSubscription = await _context.Subscriptions.FindAsync(id);
        if (existingSubscription == null)
            return null;

        existingSubscription.ClassesRemaining = subscription.ClassesRemaining;
        existingSubscription.ExpiryDate = subscription.ExpiryDate;
        existingSubscription.Status = subscription.Status;
        existingSubscription.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription == null)
            return false;

        var hasReservations = await _context.Reservations
            .AnyAsync(r => r.Student != null && r.Student.Subscriptions.Any(s => s.Id == id) && 
                          r.Status == "confirmed");

        if (hasReservations)
        {
            subscription.Status = "cancelled";
            subscription.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _context.Subscriptions.Remove(subscription);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Subscriptions.AnyAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Subscription>> GetFilteredAsync(SubscriptionFilterDto filter)
    {
        var query = _context.Subscriptions
            .Include(s => s.Student)
                .ThenInclude(st => st.User)
            .Include(s => s.Plan)
            .AsQueryable();

        if (filter.StudentId.HasValue)
            query = query.Where(s => s.StudentId == filter.StudentId.Value);

        if (filter.PlanId.HasValue)
            query = query.Where(s => s.PlanId == filter.PlanId.Value);

        if (!string.IsNullOrEmpty(filter.Status))
            query = query.Where(s => s.Status == filter.Status);

        if (filter.StartDate.HasValue)
            query = query.Where(s => s.StartDate >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(s => s.ExpiryDate <= filter.EndDate.Value);

        if (filter.ExpiringSoon == true)
        {
            var thresholdDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
            query = query.Where(s => s.Status == "active" && s.ExpiryDate <= thresholdDate);
        }

        if (filter.HasClassesRemaining == true)
            query = query.Where(s => s.ClassesRemaining > 0);

        return await query
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Subscription>> GetByStudentAsync(long studentId)
    {
        return await _context.Subscriptions
            .Include(s => s.Plan)
            .Where(s => s.StudentId == studentId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Subscription>> GetByPlanAsync(long planId)
    {
        return await _context.Subscriptions
            .Include(s => s.Student)
                .ThenInclude(st => st.User)
            .Where(s => s.PlanId == planId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<Subscription?> GetActiveSubscriptionByStudentAsync(long studentId)
    {
        return await _context.Subscriptions
            .Include(s => s.Plan)
            .Where(s => s.StudentId == studentId && 
                       s.Status == "active" && 
                       s.ExpiryDate >= DateOnly.FromDateTime(DateTime.Today) &&
                       s.ClassesRemaining > 0)
            .OrderByDescending(s => s.ExpiryDate)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync()
    {
        return await _context.Subscriptions
            .Include(s => s.Student)
                .ThenInclude(st => st.User)
            .Include(s => s.Plan)
            .Where(s => s.Status == "active" && s.ExpiryDate >= DateOnly.FromDateTime(DateTime.Today))
            .OrderBy(s => s.ExpiryDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Subscription>> GetExpiredSubscriptionsAsync()
    {
        return await _context.Subscriptions
            .Include(s => s.Student)
                .ThenInclude(st => st.User)
            .Include(s => s.Plan)
            .Where(s => s.Status == "active" && s.ExpiryDate < DateOnly.FromDateTime(DateTime.Today))
            .ToListAsync();
    }

    public async Task<IEnumerable<Subscription>> GetExpiringSoonAsync(int daysThreshold = 7)
    {
        var thresholdDate = DateOnly.FromDateTime(DateTime.Today.AddDays(daysThreshold));
        
        return await _context.Subscriptions
            .Include(s => s.Student)
                .ThenInclude(st => st.User)
            .Include(s => s.Plan)
            .Where(s => s.Status == "active" && 
                       s.ExpiryDate >= DateOnly.FromDateTime(DateTime.Today) &&
                       s.ExpiryDate <= thresholdDate)
            .OrderBy(s => s.ExpiryDate)
            .ToListAsync();
    }

    public async Task<bool> HasActiveSubscriptionAsync(long studentId)
    {
        return await _context.Subscriptions
            .AnyAsync(s => s.StudentId == studentId && 
                          s.Status == "active" && 
                          s.ExpiryDate >= DateOnly.FromDateTime(DateTime.Today) &&
                          s.ClassesRemaining > 0);
    }

    public async Task<bool> DecrementClassesAsync(long subscriptionId)
    {
        var subscription = await _context.Subscriptions.FindAsync(subscriptionId);
        if (subscription == null || subscription.ClassesRemaining <= 0)
            return false;

        subscription.ClassesRemaining--;
        subscription.UpdatedAt = DateTime.UtcNow;
        
        if (subscription.ClassesRemaining == 0)
        {
            subscription.Status = "expired";
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Subscription>> GetSubscriptionsToExpireAsync()
    {
        return await _context.Subscriptions
            .Where(s => s.Status == "active" && s.ExpiryDate < DateOnly.FromDateTime(DateTime.Today))
            .ToListAsync();
    }
}
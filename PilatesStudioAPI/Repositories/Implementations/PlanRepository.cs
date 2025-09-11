using Microsoft.EntityFrameworkCore;
using PilatesStudioAPI.Data.Context;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Repositories.Interfaces;

namespace PilatesStudioAPI.Repositories.Implementations;

public class PlanRepository : IPlanRepository
{
    private readonly PilatesStudioDbContext _context;

    public PlanRepository(PilatesStudioDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Plan>> GetAllAsync()
    {
        return await _context.Plans
            .Include(p => p.Subscriptions)
            .OrderBy(p => p.Price)
            .ToListAsync();
    }

    public async Task<Plan?> GetByIdAsync(long id)
    {
        return await _context.Plans
            .Include(p => p.Subscriptions)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Plan> CreateAsync(Plan plan)
    {
        plan.CreatedAt = DateTime.UtcNow;
        plan.UpdatedAt = DateTime.UtcNow;
        
        _context.Plans.Add(plan);
        await _context.SaveChangesAsync();
        return plan;
    }

    public async Task<Plan?> UpdateAsync(long id, Plan plan)
    {
        var existingPlan = await _context.Plans.FindAsync(id);
        if (existingPlan == null)
            return null;

        existingPlan.Title = plan.Title;
        existingPlan.Subtitle = plan.Subtitle;
        existingPlan.Description = plan.Description;
        existingPlan.Price = plan.Price;
        existingPlan.MonthlyClasses = plan.MonthlyClasses;
        existingPlan.ValidityDays = plan.ValidityDays;
        existingPlan.IsActive = plan.IsActive;
        existingPlan.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingPlan;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var plan = await _context.Plans.FindAsync(id);
        if (plan == null)
            return false;

        var hasActiveSubscriptions = await HasActiveSubscriptionsAsync(id);
        
        if (hasActiveSubscriptions)
        {
            plan.IsActive = false;
            plan.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _context.Plans.Remove(plan);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Plans.AnyAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Plan>> GetActivePlansAsync()
    {
        return await _context.Plans
            .Where(p => p.IsActive)
            .OrderBy(p => p.Price)
            .ToListAsync();
    }

    public async Task<Plan?> GetByTitleAsync(string title)
    {
        return await _context.Plans
            .FirstOrDefaultAsync(p => p.Title.ToLower() == title.ToLower());
    }

    public async Task<int> GetActiveSubscriptionsCountAsync(long planId)
    {
        return await _context.Subscriptions
            .CountAsync(s => s.PlanId == planId && s.Status == "active");
    }

    public async Task<bool> HasActiveSubscriptionsAsync(long planId)
    {
        return await _context.Subscriptions
            .AnyAsync(s => s.PlanId == planId && s.Status == "active");
    }
}
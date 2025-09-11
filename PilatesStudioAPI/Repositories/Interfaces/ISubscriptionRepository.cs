using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.Entities;

namespace PilatesStudioAPI.Repositories.Interfaces;

public interface ISubscriptionRepository
{
    Task<IEnumerable<Subscription>> GetAllAsync();
    Task<Subscription?> GetByIdAsync(long id);
    Task<Subscription> CreateAsync(Subscription subscription);
    Task<Subscription?> UpdateAsync(long id, Subscription subscription);
    Task<bool> DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
    Task<IEnumerable<Subscription>> GetFilteredAsync(SubscriptionFilterDto filter);
    Task<IEnumerable<Subscription>> GetByStudentAsync(long studentId);
    Task<IEnumerable<Subscription>> GetByPlanAsync(long planId);
    Task<Subscription?> GetActiveSubscriptionByStudentAsync(long studentId);
    Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync();
    Task<IEnumerable<Subscription>> GetExpiredSubscriptionsAsync();
    Task<IEnumerable<Subscription>> GetExpiringSoonAsync(int daysThreshold = 7);
    Task<bool> HasActiveSubscriptionAsync(long studentId);
    Task<bool> DecrementClassesAsync(long subscriptionId);
    Task<IEnumerable<Subscription>> GetSubscriptionsToExpireAsync();
}
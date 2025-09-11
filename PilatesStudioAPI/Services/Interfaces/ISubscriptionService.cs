using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Services.Interfaces;

public interface ISubscriptionService
{
    Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync();
    Task<SubscriptionDto?> GetSubscriptionByIdAsync(long id);
    Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto createSubscriptionDto);
    Task<SubscriptionDto?> UpdateSubscriptionAsync(long id, UpdateSubscriptionDto updateSubscriptionDto);
    Task<bool> DeleteSubscriptionAsync(long id);
    Task<IEnumerable<SubscriptionDto>> GetFilteredSubscriptionsAsync(SubscriptionFilterDto filter);
    Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByStudentAsync(long studentId);
    Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByPlanAsync(long planId);
    Task<SubscriptionDto?> GetActiveSubscriptionByStudentAsync(long studentId);
    Task<IEnumerable<SubscriptionDto>> GetActiveSubscriptionsAsync();
    Task<IEnumerable<SubscriptionDto>> GetExpiredSubscriptionsAsync();
    Task<IEnumerable<SubscriptionDto>> GetExpiringSoonAsync(int daysThreshold = 7);
    Task<SubscriptionDto> RenewSubscriptionAsync(long subscriptionId, RenewSubscriptionDto renewSubscriptionDto);
    Task<bool> SubscriptionExistsAsync(long id);
    Task<bool> HasActiveSubscriptionAsync(long studentId);
    Task<bool> UseClassFromSubscriptionAsync(long studentId);
    Task<int> ProcessExpiredSubscriptionsAsync();
    Task<bool> CanStudentReserveClassAsync(long studentId);
    Task<SubscriptionDto> ProcessPaymentSubscriptionAsync(long studentId, long planId);
}
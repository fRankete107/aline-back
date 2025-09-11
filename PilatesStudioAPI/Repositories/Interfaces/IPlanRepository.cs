using PilatesStudioAPI.Models.Entities;

namespace PilatesStudioAPI.Repositories.Interfaces;

public interface IPlanRepository
{
    Task<IEnumerable<Plan>> GetAllAsync();
    Task<Plan?> GetByIdAsync(long id);
    Task<Plan> CreateAsync(Plan plan);
    Task<Plan?> UpdateAsync(long id, Plan plan);
    Task<bool> DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
    Task<IEnumerable<Plan>> GetActivePlansAsync();
    Task<Plan?> GetByTitleAsync(string title);
    Task<int> GetActiveSubscriptionsCountAsync(long planId);
    Task<bool> HasActiveSubscriptionsAsync(long planId);
}
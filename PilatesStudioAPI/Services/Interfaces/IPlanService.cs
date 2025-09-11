using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Services.Interfaces;

public interface IPlanService
{
    Task<IEnumerable<PlanDto>> GetAllPlansAsync();
    Task<PlanDto?> GetPlanByIdAsync(long id);
    Task<PlanDto> CreatePlanAsync(CreatePlanDto createPlanDto);
    Task<PlanDto?> UpdatePlanAsync(long id, UpdatePlanDto updatePlanDto);
    Task<bool> DeletePlanAsync(long id);
    Task<IEnumerable<PlanDto>> GetActivePlansAsync();
    Task<bool> PlanExistsAsync(long id);
    Task<bool> PlanTitleExistsAsync(string title, long? excludeId = null);
    Task<bool> HasActiveSubscriptionsAsync(long planId);
}
using AutoMapper;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Repositories.Interfaces;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Services.Implementations;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _planRepository;
    private readonly IMapper _mapper;

    public PlanService(IPlanRepository planRepository, IMapper mapper)
    {
        _planRepository = planRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PlanDto>> GetAllPlansAsync()
    {
        var plans = await _planRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PlanDto>>(plans);
    }

    public async Task<PlanDto?> GetPlanByIdAsync(long id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        return plan == null ? null : _mapper.Map<PlanDto>(plan);
    }

    public async Task<PlanDto> CreatePlanAsync(CreatePlanDto createPlanDto)
    {
        if (await PlanTitleExistsAsync(createPlanDto.Title))
        {
            throw new InvalidOperationException($"A plan with the title '{createPlanDto.Title}' already exists.");
        }

        var plan = _mapper.Map<Plan>(createPlanDto);
        var createdPlan = await _planRepository.CreateAsync(plan);
        return _mapper.Map<PlanDto>(createdPlan);
    }

    public async Task<PlanDto?> UpdatePlanAsync(long id, UpdatePlanDto updatePlanDto)
    {
        var existingPlan = await _planRepository.GetByIdAsync(id);
        if (existingPlan == null)
            return null;

        if (!string.IsNullOrEmpty(updatePlanDto.Title) && 
            await PlanTitleExistsAsync(updatePlanDto.Title, id))
        {
            throw new InvalidOperationException($"A plan with the title '{updatePlanDto.Title}' already exists.");
        }

        _mapper.Map(updatePlanDto, existingPlan);
        var updatedPlan = await _planRepository.UpdateAsync(id, existingPlan);
        return updatedPlan == null ? null : _mapper.Map<PlanDto>(updatedPlan);
    }

    public async Task<bool> DeletePlanAsync(long id)
    {
        if (!await _planRepository.ExistsAsync(id))
            return false;

        return await _planRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<PlanDto>> GetActivePlansAsync()
    {
        var plans = await _planRepository.GetActivePlansAsync();
        return _mapper.Map<IEnumerable<PlanDto>>(plans);
    }

    public async Task<bool> PlanExistsAsync(long id)
    {
        return await _planRepository.ExistsAsync(id);
    }

    public async Task<bool> PlanTitleExistsAsync(string title, long? excludeId = null)
    {
        var existingPlan = await _planRepository.GetByTitleAsync(title);
        return existingPlan != null && (!excludeId.HasValue || existingPlan.Id != excludeId.Value);
    }

    public async Task<bool> HasActiveSubscriptionsAsync(long planId)
    {
        return await _planRepository.HasActiveSubscriptionsAsync(planId);
    }
}
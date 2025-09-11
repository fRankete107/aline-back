using AutoMapper;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Repositories.Interfaces;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Services.Implementations;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPlanRepository _planRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IMapper _mapper;

    public SubscriptionService(
        ISubscriptionRepository subscriptionRepository,
        IPlanRepository planRepository,
        IStudentRepository studentRepository,
        IMapper mapper)
    {
        _subscriptionRepository = subscriptionRepository;
        _planRepository = planRepository;
        _studentRepository = studentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync()
    {
        var subscriptions = await _subscriptionRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<SubscriptionDto>>(subscriptions);
    }

    public async Task<SubscriptionDto?> GetSubscriptionByIdAsync(long id)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        return subscription == null ? null : _mapper.Map<SubscriptionDto>(subscription);
    }

    public async Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto createSubscriptionDto)
    {
        await ValidateSubscriptionCreationAsync(createSubscriptionDto);

        // Cancelar suscripción activa anterior si existe
        var existingSubscription = await _subscriptionRepository.GetActiveSubscriptionByStudentAsync(createSubscriptionDto.StudentId);
        if (existingSubscription != null)
        {
            existingSubscription.Status = "cancelled";
            await _subscriptionRepository.UpdateAsync(existingSubscription.Id, existingSubscription);
        }

        var plan = await _planRepository.GetByIdAsync(createSubscriptionDto.PlanId);
        var subscription = _mapper.Map<Subscription>(createSubscriptionDto);
        
        // Configurar suscripción basada en el plan
        subscription.ClassesRemaining = plan!.MonthlyClasses;
        subscription.ExpiryDate = subscription.StartDate.AddDays(plan.ValidityDays);

        var createdSubscription = await _subscriptionRepository.CreateAsync(subscription);
        return _mapper.Map<SubscriptionDto>(createdSubscription);
    }

    public async Task<SubscriptionDto?> UpdateSubscriptionAsync(long id, UpdateSubscriptionDto updateSubscriptionDto)
    {
        var existingSubscription = await _subscriptionRepository.GetByIdAsync(id);
        if (existingSubscription == null)
            return null;

        _mapper.Map(updateSubscriptionDto, existingSubscription);
        var updatedSubscription = await _subscriptionRepository.UpdateAsync(id, existingSubscription);
        return updatedSubscription == null ? null : _mapper.Map<SubscriptionDto>(updatedSubscription);
    }

    public async Task<bool> DeleteSubscriptionAsync(long id)
    {
        if (!await _subscriptionRepository.ExistsAsync(id))
            return false;

        return await _subscriptionRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<SubscriptionDto>> GetFilteredSubscriptionsAsync(SubscriptionFilterDto filter)
    {
        var subscriptions = await _subscriptionRepository.GetFilteredAsync(filter);
        return _mapper.Map<IEnumerable<SubscriptionDto>>(subscriptions);
    }

    public async Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByStudentAsync(long studentId)
    {
        if (!await _studentRepository.ExistsAsync(studentId))
            throw new ArgumentException($"Student with ID {studentId} not found.");

        var subscriptions = await _subscriptionRepository.GetByStudentAsync(studentId);
        return _mapper.Map<IEnumerable<SubscriptionDto>>(subscriptions);
    }

    public async Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByPlanAsync(long planId)
    {
        if (!await _planRepository.ExistsAsync(planId))
            throw new ArgumentException($"Plan with ID {planId} not found.");

        var subscriptions = await _subscriptionRepository.GetByPlanAsync(planId);
        return _mapper.Map<IEnumerable<SubscriptionDto>>(subscriptions);
    }

    public async Task<SubscriptionDto?> GetActiveSubscriptionByStudentAsync(long studentId)
    {
        if (!await _studentRepository.ExistsAsync(studentId))
            throw new ArgumentException($"Student with ID {studentId} not found.");

        var subscription = await _subscriptionRepository.GetActiveSubscriptionByStudentAsync(studentId);
        return subscription == null ? null : _mapper.Map<SubscriptionDto>(subscription);
    }

    public async Task<IEnumerable<SubscriptionDto>> GetActiveSubscriptionsAsync()
    {
        var subscriptions = await _subscriptionRepository.GetActiveSubscriptionsAsync();
        return _mapper.Map<IEnumerable<SubscriptionDto>>(subscriptions);
    }

    public async Task<IEnumerable<SubscriptionDto>> GetExpiredSubscriptionsAsync()
    {
        var subscriptions = await _subscriptionRepository.GetExpiredSubscriptionsAsync();
        return _mapper.Map<IEnumerable<SubscriptionDto>>(subscriptions);
    }

    public async Task<IEnumerable<SubscriptionDto>> GetExpiringSoonAsync(int daysThreshold = 7)
    {
        var subscriptions = await _subscriptionRepository.GetExpiringSoonAsync(daysThreshold);
        return _mapper.Map<IEnumerable<SubscriptionDto>>(subscriptions);
    }

    public async Task<SubscriptionDto> RenewSubscriptionAsync(long subscriptionId, RenewSubscriptionDto renewSubscriptionDto)
    {
        var existingSubscription = await _subscriptionRepository.GetByIdAsync(subscriptionId);
        if (existingSubscription == null)
            throw new ArgumentException($"Subscription with ID {subscriptionId} not found.");

        if (!await _planRepository.ExistsAsync(renewSubscriptionDto.PlanId))
            throw new ArgumentException($"Plan with ID {renewSubscriptionDto.PlanId} not found.");

        var plan = await _planRepository.GetByIdAsync(renewSubscriptionDto.PlanId);

        // Marcar suscripción actual como cancelada
        existingSubscription.Status = "cancelled";
        await _subscriptionRepository.UpdateAsync(subscriptionId, existingSubscription);

        // Crear nueva suscripción
        var newSubscriptionDto = new CreateSubscriptionDto
        {
            StudentId = existingSubscription.StudentId,
            PlanId = renewSubscriptionDto.PlanId,
            StartDate = renewSubscriptionDto.StartDate ?? DateOnly.FromDateTime(DateTime.Today)
        };

        return await CreateSubscriptionAsync(newSubscriptionDto);
    }

    public async Task<bool> SubscriptionExistsAsync(long id)
    {
        return await _subscriptionRepository.ExistsAsync(id);
    }

    public async Task<bool> HasActiveSubscriptionAsync(long studentId)
    {
        return await _subscriptionRepository.HasActiveSubscriptionAsync(studentId);
    }

    public async Task<bool> UseClassFromSubscriptionAsync(long studentId)
    {
        var subscription = await _subscriptionRepository.GetActiveSubscriptionByStudentAsync(studentId);
        if (subscription == null || subscription.ClassesRemaining <= 0)
            return false;

        return await _subscriptionRepository.DecrementClassesAsync(subscription.Id);
    }

    public async Task<int> ProcessExpiredSubscriptionsAsync()
    {
        var expiredSubscriptions = await _subscriptionRepository.GetSubscriptionsToExpireAsync();
        int processedCount = 0;

        foreach (var subscription in expiredSubscriptions)
        {
            subscription.Status = "expired";
            await _subscriptionRepository.UpdateAsync(subscription.Id, subscription);
            processedCount++;
        }

        return processedCount;
    }

    public async Task<bool> CanStudentReserveClassAsync(long studentId)
    {
        var activeSubscription = await _subscriptionRepository.GetActiveSubscriptionByStudentAsync(studentId);
        return activeSubscription != null && activeSubscription.ClassesRemaining > 0;
    }

    public async Task<SubscriptionDto> ProcessPaymentSubscriptionAsync(long studentId, long planId)
    {
        var plan = await _planRepository.GetByIdAsync(planId);
        if (plan == null)
            throw new ArgumentException($"Plan with ID {planId} not found.");

        if (!await _studentRepository.ExistsAsync(studentId))
            throw new ArgumentException($"Student with ID {studentId} not found.");

        // Check if student already has an active subscription
        var existingSubscription = await _subscriptionRepository.GetActiveSubscriptionByStudentAsync(studentId);
        
        if (existingSubscription != null)
        {
            // If same plan, extend the subscription
            if (existingSubscription.PlanId == planId)
            {
                existingSubscription.ExpiryDate = existingSubscription.ExpiryDate.AddDays(30); // Extend by plan duration
                existingSubscription.ClassesRemaining += plan.MonthlyClasses;
                existingSubscription.UpdatedAt = DateTime.UtcNow;
                
                await _subscriptionRepository.UpdateAsync(existingSubscription.Id, existingSubscription);
                return _mapper.Map<SubscriptionDto>(existingSubscription);
            }
            else
            {
                // Different plan, cancel existing and create new
                existingSubscription.Status = "cancelled";
                await _subscriptionRepository.UpdateAsync(existingSubscription.Id, existingSubscription);
            }
        }

        // Create new subscription
        var subscription = new Subscription
        {
            StudentId = studentId,
            PlanId = planId,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30)), // 30 days from today
            ClassesRemaining = plan.MonthlyClasses,
            Status = "active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdSubscription = await _subscriptionRepository.CreateAsync(subscription);
        return _mapper.Map<SubscriptionDto>(createdSubscription);
    }

    private async Task ValidateSubscriptionCreationAsync(CreateSubscriptionDto createSubscriptionDto)
    {
        if (!await _studentRepository.ExistsAsync(createSubscriptionDto.StudentId))
            throw new ArgumentException($"Student with ID {createSubscriptionDto.StudentId} not found.");

        if (!await _planRepository.ExistsAsync(createSubscriptionDto.PlanId))
            throw new ArgumentException($"Plan with ID {createSubscriptionDto.PlanId} not found.");

        var plan = await _planRepository.GetByIdAsync(createSubscriptionDto.PlanId);
        if (plan != null && !plan.IsActive)
            throw new InvalidOperationException("Cannot create subscription for inactive plan.");
    }
}
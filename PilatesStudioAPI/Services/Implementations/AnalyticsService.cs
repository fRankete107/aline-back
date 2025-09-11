using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Repositories.Interfaces;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Services.Implementations;

public class AnalyticsService : IAnalyticsService
{
    private readonly IAnalyticsRepository _analyticsRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IZoneRepository _zoneRepository;
    private readonly IPlanRepository _planRepository;
    private readonly IStudentRepository _studentRepository;

    public AnalyticsService(
        IAnalyticsRepository analyticsRepository,
        IInstructorRepository instructorRepository,
        IZoneRepository zoneRepository,
        IPlanRepository planRepository,
        IStudentRepository studentRepository)
    {
        _analyticsRepository = analyticsRepository;
        _instructorRepository = instructorRepository;
        _zoneRepository = zoneRepository;
        _planRepository = planRepository;
        _studentRepository = studentRepository;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(AnalyticsFilterDto? filter = null)
    {
        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        return await _analyticsRepository.GetDashboardStatsAsync(filter);
    }

    public async Task<RealTimeKpiDto> GetRealTimeKpisAsync()
    {
        return await _analyticsRepository.GetRealTimeKpisAsync();
    }

    public async Task<StudentAnalyticsDto> GetStudentAnalyticsAsync(AnalyticsFilterDto? filter = null)
    {
        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        var analytics = await _analyticsRepository.GetStudentAnalyticsAsync(filter);
        
        // Add business intelligence logic
        if (analytics.TotalStudents > 0)
        {
            // Calculate additional metrics
            analytics.AverageReservationsPerStudent = Math.Round(analytics.AverageReservationsPerStudent, 2);
        }

        return analytics;
    }

    public async Task<List<TopStudentDto>> GetTopStudentsByReservationsAsync(int limit = 10)
    {
        if (limit <= 0 || limit > 100)
            throw new ArgumentException("Limit must be between 1 and 100");

        return await _analyticsRepository.GetTopStudentsByReservationsAsync(limit);
    }

    public async Task<List<TopStudentDto>> GetTopStudentsByPaymentsAsync(int limit = 10)
    {
        if (limit <= 0 || limit > 100)
            throw new ArgumentException("Limit must be between 1 and 100");

        return await _analyticsRepository.GetTopStudentsByPaymentsAsync(limit);
    }

    public async Task<ClassAnalyticsDto> GetClassAnalyticsAsync(AnalyticsFilterDto? filter = null)
    {
        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        var analytics = await _analyticsRepository.GetClassAnalyticsAsync(filter);
        
        // Add business logic for capacity analysis
        if (analytics.AverageCapacityUtilization < 50)
        {
            // Could add recommendations or warnings here
        }

        return analytics;
    }

    public async Task<List<PopularClassTimeDto>> GetPopularClassTimesAsync(int limit = 10)
    {
        if (limit <= 0 || limit > 24)
            throw new ArgumentException("Limit must be between 1 and 24");

        return await _analyticsRepository.GetPopularClassTimesAsync(limit);
    }

    public async Task<List<InstructorClassStatsDto>> GetInstructorStatsAsync(AnalyticsFilterDto? filter = null)
    {
        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        return await _analyticsRepository.GetInstructorStatsAsync(filter);
    }

    public async Task<ReservationAnalyticsDto> GetReservationAnalyticsAsync(AnalyticsFilterDto? filter = null)
    {
        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        var analytics = await _analyticsRepository.GetReservationAnalyticsAsync(filter);
        
        // Add business intelligence
        if (analytics.CancellationRate > 25)
        {
            // High cancellation rate - could trigger alerts
        }
        
        if (analytics.NoShowRate > 15)
        {
            // High no-show rate - could trigger policy review
        }

        return analytics;
    }

    public async Task<List<PeakHoursDto>> GetPeakHoursAnalysisAsync(AnalyticsFilterDto? filter = null)
    {
        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        return await _analyticsRepository.GetPeakHoursAnalysisAsync(filter);
    }

    public async Task<FinancialAnalyticsDto> GetFinancialAnalyticsAsync(AnalyticsFilterDto? filter = null)
    {
        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        var analytics = await _analyticsRepository.GetFinancialAnalyticsAsync(filter);
        
        // Enhanced financial analytics
        analytics.RevenueByPaymentMethod = await _analyticsRepository.GetRevenueByPaymentMethodAsync(filter);
        analytics.RevenueByPlan = await _analyticsRepository.GetRevenueByPlanAsync(filter);
        analytics.MonthlyRevenueGrowth = await _analyticsRepository.GetMonthlyRevenueGrowthAsync(12);
        analytics.RevenueByMonth = await GetRevenueByMonthAsync(12);
        analytics.RevenueByWeek = await GetRevenueByWeekAsync(12);

        return analytics;
    }

    public async Task<List<RevenueByPeriodDto>> GetRevenueByMonthAsync(int months = 12)
    {
        if (months <= 0 || months > 24)
            throw new ArgumentException("Months must be between 1 and 24");

        var revenueByMonth = await _analyticsRepository.GetRevenueByMonthAsync(months);
        
        // Calculate growth rates
        for (int i = 1; i < revenueByMonth.Count; i++)
        {
            var current = revenueByMonth[i];
            var previous = revenueByMonth[i - 1];
            
            if (previous.Revenue > 0)
            {
                current.GrowthRate = Math.Round((current.Revenue - previous.Revenue) / previous.Revenue * 100, 2);
            }
        }

        return revenueByMonth;
    }

    public async Task<List<RevenueByPeriodDto>> GetRevenueByWeekAsync(int weeks = 12)
    {
        if (weeks <= 0 || weeks > 52)
            throw new ArgumentException("Weeks must be between 1 and 52");

        var revenueByWeek = await _analyticsRepository.GetRevenueByWeekAsync(weeks);
        
        // Calculate growth rates
        for (int i = 1; i < revenueByWeek.Count; i++)
        {
            var current = revenueByWeek[i];
            var previous = revenueByWeek[i - 1];
            
            if (previous.Revenue > 0)
            {
                current.GrowthRate = Math.Round((current.Revenue - previous.Revenue) / previous.Revenue * 100, 2);
            }
        }

        return revenueByWeek;
    }

    public async Task<SubscriptionAnalyticsDto> GetSubscriptionAnalyticsAsync(AnalyticsFilterDto? filter = null)
    {
        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        var analytics = await _analyticsRepository.GetSubscriptionAnalyticsAsync(filter);
        
        // Enhanced subscription analytics  
        analytics.SubscriptionsByStatus = await _analyticsRepository.GetSubscriptionsByStatusAsync(filter);
        analytics.PlanPopularity = await GetPlanPopularityAsync(filter);
        analytics.SubscriptionTrends = await GetSubscriptionTrendsAsync(12);

        return analytics;
    }

    public async Task<List<PlanPopularityDto>> GetPlanPopularityAsync(AnalyticsFilterDto? filter = null)
    {
        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        return await _analyticsRepository.GetPlanPopularityAsync(filter);
    }

    public async Task<List<SubscriptionTrendDto>> GetSubscriptionTrendsAsync(int months = 12)
    {
        if (months <= 0 || months > 24)
            throw new ArgumentException("Months must be between 1 and 24");

        return await _analyticsRepository.GetSubscriptionTrendsAsync(months);
    }

    public async Task<PeriodComparisonDto> GetPeriodComparisonAsync(string period, AnalyticsFilterDto? filter = null)
    {
        if (string.IsNullOrEmpty(period))
            throw new ArgumentException("Period is required");

        var validPeriods = new[] { "daily", "weekly", "monthly", "yearly" };
        if (!validPeriods.Contains(period.ToLower()))
            throw new ArgumentException("Invalid period. Must be one of: daily, weekly, monthly, yearly");

        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        var comparison = await _analyticsRepository.GetPeriodComparisonAsync(period, filter);
        
        // Calculate comparison metrics
        var current = comparison.CurrentPeriod;
        var previous = comparison.PreviousPeriod;
        
        comparison.Comparison = new AnalyticsComparisonDto
        {
            RevenueGrowth = CalculateGrowthRate(previous.Revenue, current.Revenue),
            StudentGrowth = CalculateGrowthRate(previous.Students, current.Students),
            ClassGrowth = CalculateGrowthRate(previous.Classes, current.Classes),
            ReservationGrowth = CalculateGrowthRate(previous.Reservations, current.Reservations),
            PaymentGrowth = CalculateGrowthRate(previous.Payments, current.Payments),
            CapacityGrowth = CalculateGrowthRate(previous.CapacityUtilization, current.CapacityUtilization)
        };

        return comparison;
    }

    public async Task<decimal> GetAverageClassCapacityUtilizationAsync(AnalyticsFilterDto? filter = null)
    {
        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        return await _analyticsRepository.GetAverageClassCapacityUtilizationAsync(filter);
    }

    public async Task<decimal> GetCustomerRetentionRateAsync(int months = 12)
    {
        if (months <= 0 || months > 24)
            throw new ArgumentException("Months must be between 1 and 24");

        return await _analyticsRepository.GetCustomerRetentionRateAsync(months);
    }

    public async Task<decimal> GetSubscriptionRenewalRateAsync(int months = 12)
    {
        if (months <= 0 || months > 24)
            throw new ArgumentException("Months must be between 1 and 24");

        return await _analyticsRepository.GetSubscriptionRenewalRateAsync(months);
    }

    public async Task<decimal> GetAverageRevenuePerUserAsync(AnalyticsFilterDto? filter = null)
    {
        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        return await _analyticsRepository.GetAverageRevenuePerUserAsync(filter);
    }

    public async Task<bool> ValidateAnalyticsFilterAsync(AnalyticsFilterDto filter)
    {
        if (filter == null)
            return false;

        // Validate date range
        if (filter.StartDate.HasValue && filter.EndDate.HasValue)
        {
            if (filter.StartDate > filter.EndDate)
                return false;

            // Don't allow ranges more than 2 years
            if ((filter.EndDate.Value - filter.StartDate.Value).TotalDays > 730)
                return false;
        }

        // Validate entity IDs
        if (filter.InstructorId.HasValue && !await _instructorRepository.ExistsAsync(filter.InstructorId.Value))
            return false;

        if (filter.ZoneId.HasValue && !await _zoneRepository.ExistsAsync(filter.ZoneId.Value))
            return false;

        if (filter.PlanId.HasValue && !await _planRepository.ExistsAsync(filter.PlanId.Value))
            return false;

        // Validate period
        if (!string.IsNullOrEmpty(filter.Period))
        {
            var validPeriods = new[] { "daily", "weekly", "monthly", "yearly" };
            if (!validPeriods.Contains(filter.Period.ToLower()))
                return false;
        }

        // Validate payment method
        if (!string.IsNullOrEmpty(filter.PaymentMethod))
        {
            var validMethods = new[] { "cash", "credit_card", "debit_card", "bank_transfer", "digital_wallet" };
            if (!validMethods.Contains(filter.PaymentMethod.ToLower()))
                return false;
        }

        // Validate limit
        if (filter.Limit < 1 || filter.Limit > 1000)
            return false;

        return true;
    }

    public async Task<AnalyticsFilterDto> NormalizeAnalyticsFilterAsync(AnalyticsFilterDto filter)
    {
        filter ??= new AnalyticsFilterDto();

        // Set default date range if not provided (last 30 days)
        if (!filter.StartDate.HasValue && !filter.EndDate.HasValue)
        {
            filter.EndDate = DateTime.UtcNow.Date;
            filter.StartDate = filter.EndDate.Value.AddDays(-30);
        }
        else if (filter.StartDate.HasValue && !filter.EndDate.HasValue)
        {
            filter.EndDate = DateTime.UtcNow.Date;
        }
        else if (!filter.StartDate.HasValue && filter.EndDate.HasValue)
        {
            filter.StartDate = filter.EndDate.Value.AddDays(-30);
        }

        // Normalize strings
        filter.ClassType = filter.ClassType?.Trim().ToLowerInvariant();
        filter.PaymentMethod = filter.PaymentMethod?.Trim().ToLowerInvariant();
        filter.Period = filter.Period?.Trim().ToLowerInvariant();

        // Set default limit
        if (filter.Limit <= 0)
            filter.Limit = 10;

        return filter;
    }

    public async Task<Dictionary<string, object>> GetTrendAnalysisAsync(string metric, string period, int periods = 12)
    {
        if (string.IsNullOrEmpty(metric))
            throw new ArgumentException("Metric is required");

        if (string.IsNullOrEmpty(period))
            throw new ArgumentException("Period is required");

        if (periods <= 0 || periods > 24)
            throw new ArgumentException("Periods must be between 1 and 24");

        return await _analyticsRepository.GetTrendAnalysisAsync(metric, period, periods);
    }

    public async Task<Dictionary<string, decimal>> GetSeasonalityAnalysisAsync(string metric)
    {
        if (string.IsNullOrEmpty(metric))
            throw new ArgumentException("Metric is required");

        return await _analyticsRepository.GetSeasonalityAnalysisAsync(metric);
    }

    public async Task<List<object>> GetForecastAsync(string metric, int periods = 6)
    {
        if (string.IsNullOrEmpty(metric))
            throw new ArgumentException("Metric is required");

        if (periods <= 0 || periods > 12)
            throw new ArgumentException("Periods must be between 1 and 12");

        return await _analyticsRepository.GetForecastAsync(metric, periods);
    }

    public async Task<Dictionary<string, object>> GetInstructorPerformanceAsync(long instructorId, AnalyticsFilterDto? filter = null)
    {
        if (!await _instructorRepository.ExistsAsync(instructorId))
            throw new ArgumentException($"Instructor with ID {instructorId} not found");

        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        return await _analyticsRepository.GetInstructorPerformanceAsync(instructorId, filter);
    }

    public async Task<Dictionary<string, object>> GetZonePerformanceAsync(long zoneId, AnalyticsFilterDto? filter = null)
    {
        if (!await _zoneRepository.ExistsAsync(zoneId))
            throw new ArgumentException($"Zone with ID {zoneId} not found");

        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        return await _analyticsRepository.GetZonePerformanceAsync(zoneId, filter);
    }

    public async Task<Dictionary<string, object>> GetPlanPerformanceAsync(long planId, AnalyticsFilterDto? filter = null)
    {
        if (!await _planRepository.ExistsAsync(planId))
            throw new ArgumentException($"Plan with ID {planId} not found");

        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        return await _analyticsRepository.GetPlanPerformanceAsync(planId, filter);
    }

    public async Task<Dictionary<string, object>> GetStudentActivityAsync(long studentId, AnalyticsFilterDto? filter = null)
    {
        if (!await _studentRepository.ExistsAsync(studentId))
            throw new ArgumentException($"Student with ID {studentId} not found");

        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        return await _analyticsRepository.GetStudentActivityAsync(studentId, filter);
    }

    public async Task<byte[]> ExportAnalyticsAsync(string reportType, AnalyticsFilterDto? filter = null, string format = "excel")
    {
        if (string.IsNullOrEmpty(reportType))
            throw new ArgumentException("Report type is required");

        var validReportTypes = new[] { "dashboard", "students", "classes", "reservations", "financial", "subscriptions" };
        if (!validReportTypes.Contains(reportType.ToLower()))
            throw new ArgumentException("Invalid report type");

        var validFormats = new[] { "excel", "pdf", "csv" };
        if (!validFormats.Contains(format.ToLower()))
            throw new ArgumentException("Invalid format");

        filter = await NormalizeAnalyticsFilterAsync(filter ?? new AnalyticsFilterDto());
        
        if (!await ValidateAnalyticsFilterAsync(filter))
            throw new ArgumentException("Invalid analytics filter parameters");

        // For now, return empty array - would implement actual export logic
        return Array.Empty<byte>();
    }

    public async Task ClearAnalyticsCacheAsync()
    {
        // Implementation would clear cache
        await Task.CompletedTask;
    }

    public async Task<Dictionary<string, object>> GetCachedAnalyticsAsync(string cacheKey)
    {
        if (string.IsNullOrEmpty(cacheKey))
            throw new ArgumentException("Cache key is required");

        // Implementation would retrieve from cache
        return await Task.FromResult(new Dictionary<string, object>());
    }

    private static decimal CalculateGrowthRate(decimal previous, decimal current)
    {
        if (previous == 0)
            return current > 0 ? 100 : 0;
        
        return Math.Round((current - previous) / previous * 100, 2);
    }

    private static decimal CalculateGrowthRate(int previous, int current)
    {
        if (previous == 0)
            return current > 0 ? 100 : 0;
        
        return Math.Round((decimal)(current - previous) / previous * 100, 2);
    }
}
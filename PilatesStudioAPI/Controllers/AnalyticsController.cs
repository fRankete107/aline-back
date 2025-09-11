using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>
    /// Get dashboard statistics with optional filters
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats([FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var stats = await _analyticsService.GetDashboardStatsAsync(filter);
            return Ok(stats);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get real-time KPIs
    /// </summary>
    [HttpGet("real-time")]
    public async Task<ActionResult<RealTimeKpiDto>> GetRealTimeKpis()
    {
        var kpis = await _analyticsService.GetRealTimeKpisAsync();
        return Ok(kpis);
    }

    /// <summary>
    /// Get student analytics
    /// </summary>
    [HttpGet("students")]
    public async Task<ActionResult<StudentAnalyticsDto>> GetStudentAnalytics([FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var analytics = await _analyticsService.GetStudentAnalyticsAsync(filter);
            return Ok(analytics);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get top students by reservations
    /// </summary>
    [HttpGet("students/top-reservations")]
    public async Task<ActionResult<List<TopStudentDto>>> GetTopStudentsByReservations([FromQuery] int limit = 10)
    {
        try
        {
            var students = await _analyticsService.GetTopStudentsByReservationsAsync(limit);
            return Ok(students);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get top students by payments
    /// </summary>
    [HttpGet("students/top-payments")]
    public async Task<ActionResult<List<TopStudentDto>>> GetTopStudentsByPayments([FromQuery] int limit = 10)
    {
        try
        {
            var students = await _analyticsService.GetTopStudentsByPaymentsAsync(limit);
            return Ok(students);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get class analytics
    /// </summary>
    [HttpGet("classes")]
    public async Task<ActionResult<ClassAnalyticsDto>> GetClassAnalytics([FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var analytics = await _analyticsService.GetClassAnalyticsAsync(filter);
            return Ok(analytics);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get popular class times
    /// </summary>
    [HttpGet("classes/popular-times")]
    public async Task<ActionResult<List<PopularClassTimeDto>>> GetPopularClassTimes([FromQuery] int limit = 10)
    {
        try
        {
            var times = await _analyticsService.GetPopularClassTimesAsync(limit);
            return Ok(times);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get instructor statistics
    /// </summary>
    [HttpGet("instructors")]
    public async Task<ActionResult<List<InstructorClassStatsDto>>> GetInstructorStats([FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var stats = await _analyticsService.GetInstructorStatsAsync(filter);
            return Ok(stats);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get specific instructor performance
    /// </summary>
    [HttpGet("instructors/{instructorId}/performance")]
    public async Task<ActionResult<Dictionary<string, object>>> GetInstructorPerformance(long instructorId, [FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var performance = await _analyticsService.GetInstructorPerformanceAsync(instructorId, filter);
            return Ok(performance);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get reservation analytics
    /// </summary>
    [HttpGet("reservations")]
    public async Task<ActionResult<ReservationAnalyticsDto>> GetReservationAnalytics([FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var analytics = await _analyticsService.GetReservationAnalyticsAsync(filter);
            return Ok(analytics);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get peak hours analysis
    /// </summary>
    [HttpGet("reservations/peak-hours")]
    public async Task<ActionResult<List<PeakHoursDto>>> GetPeakHoursAnalysis([FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var peakHours = await _analyticsService.GetPeakHoursAnalysisAsync(filter);
            return Ok(peakHours);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get financial analytics
    /// </summary>
    [HttpGet("financial")]
    public async Task<ActionResult<FinancialAnalyticsDto>> GetFinancialAnalytics([FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var analytics = await _analyticsService.GetFinancialAnalyticsAsync(filter);
            return Ok(analytics);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get revenue by month
    /// </summary>
    [HttpGet("financial/revenue/monthly")]
    public async Task<ActionResult<List<RevenueByPeriodDto>>> GetRevenueByMonth([FromQuery] int months = 12)
    {
        try
        {
            var revenue = await _analyticsService.GetRevenueByMonthAsync(months);
            return Ok(revenue);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get revenue by week
    /// </summary>
    [HttpGet("financial/revenue/weekly")]
    public async Task<ActionResult<List<RevenueByPeriodDto>>> GetRevenueByWeek([FromQuery] int weeks = 12)
    {
        try
        {
            var revenue = await _analyticsService.GetRevenueByWeekAsync(weeks);
            return Ok(revenue);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get subscription analytics
    /// </summary>
    [HttpGet("subscriptions")]
    public async Task<ActionResult<SubscriptionAnalyticsDto>> GetSubscriptionAnalytics([FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var analytics = await _analyticsService.GetSubscriptionAnalyticsAsync(filter);
            return Ok(analytics);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get plan popularity
    /// </summary>
    [HttpGet("subscriptions/plan-popularity")]
    public async Task<ActionResult<List<PlanPopularityDto>>> GetPlanPopularity([FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var popularity = await _analyticsService.GetPlanPopularityAsync(filter);
            return Ok(popularity);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get subscription trends
    /// </summary>
    [HttpGet("subscriptions/trends")]
    public async Task<ActionResult<List<SubscriptionTrendDto>>> GetSubscriptionTrends([FromQuery] int months = 12)
    {
        try
        {
            var trends = await _analyticsService.GetSubscriptionTrendsAsync(months);
            return Ok(trends);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get period comparison
    /// </summary>
    [HttpGet("comparison/{period}")]
    public async Task<ActionResult<PeriodComparisonDto>> GetPeriodComparison(string period, [FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var comparison = await _analyticsService.GetPeriodComparisonAsync(period, filter);
            return Ok(comparison);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get average class capacity utilization
    /// </summary>
    [HttpGet("metrics/capacity-utilization")]
    public async Task<ActionResult<decimal>> GetAverageClassCapacityUtilization([FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var utilization = await _analyticsService.GetAverageClassCapacityUtilizationAsync(filter);
            return Ok(new { CapacityUtilization = utilization });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get customer retention rate
    /// </summary>
    [HttpGet("metrics/retention-rate")]
    public async Task<ActionResult<decimal>> GetCustomerRetentionRate([FromQuery] int months = 12)
    {
        try
        {
            var rate = await _analyticsService.GetCustomerRetentionRateAsync(months);
            return Ok(new { RetentionRate = rate });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get subscription renewal rate
    /// </summary>
    [HttpGet("metrics/renewal-rate")]
    public async Task<ActionResult<decimal>> GetSubscriptionRenewalRate([FromQuery] int months = 12)
    {
        try
        {
            var rate = await _analyticsService.GetSubscriptionRenewalRateAsync(months);
            return Ok(new { RenewalRate = rate });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get average revenue per user
    /// </summary>
    [HttpGet("metrics/arpu")]
    public async Task<ActionResult<decimal>> GetAverageRevenuePerUser([FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var arpu = await _analyticsService.GetAverageRevenuePerUserAsync(filter);
            return Ok(new { AverageRevenuePerUser = arpu });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get trend analysis
    /// </summary>
    [HttpGet("trends/{metric}")]
    public async Task<ActionResult<Dictionary<string, object>>> GetTrendAnalysis(
        string metric, 
        [FromQuery] string period, 
        [FromQuery] int periods = 12)
    {
        try
        {
            var trends = await _analyticsService.GetTrendAnalysisAsync(metric, period, periods);
            return Ok(trends);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get seasonality analysis
    /// </summary>
    [HttpGet("seasonality/{metric}")]
    public async Task<ActionResult<Dictionary<string, decimal>>> GetSeasonalityAnalysis(string metric)
    {
        try
        {
            var seasonality = await _analyticsService.GetSeasonalityAnalysisAsync(metric);
            return Ok(seasonality);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get forecast
    /// </summary>
    [HttpGet("forecast/{metric}")]
    public async Task<ActionResult<List<object>>> GetForecast(string metric, [FromQuery] int periods = 6)
    {
        try
        {
            var forecast = await _analyticsService.GetForecastAsync(metric, periods);
            return Ok(forecast);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get zone performance
    /// </summary>
    [HttpGet("zones/{zoneId}/performance")]
    public async Task<ActionResult<Dictionary<string, object>>> GetZonePerformance(long zoneId, [FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var performance = await _analyticsService.GetZonePerformanceAsync(zoneId, filter);
            return Ok(performance);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get plan performance
    /// </summary>
    [HttpGet("plans/{planId}/performance")]
    public async Task<ActionResult<Dictionary<string, object>>> GetPlanPerformance(long planId, [FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var performance = await _analyticsService.GetPlanPerformanceAsync(planId, filter);
            return Ok(performance);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get student activity
    /// </summary>
    [HttpGet("students/{studentId}/activity")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<Dictionary<string, object>>> GetStudentActivity(long studentId, [FromQuery] AnalyticsFilterDto? filter = null)
    {
        try
        {
            var activity = await _analyticsService.GetStudentActivityAsync(studentId, filter);
            return Ok(activity);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Export analytics data
    /// </summary>
    [HttpPost("export")]
    public async Task<ActionResult> ExportAnalytics([FromBody] ExportAnalyticsRequestDto request)
    {
        try
        {
            var data = await _analyticsService.ExportAnalyticsAsync(request.ReportType, request.Filter, request.Format);
            
            var fileName = $"analytics_{request.ReportType}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{request.Format}";
            var contentType = request.Format.ToLower() switch
            {
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "pdf" => "application/pdf",
                "csv" => "text/csv",
                _ => "application/octet-stream"
            };

            return File(data, contentType, fileName);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Clear analytics cache
    /// </summary>
    [HttpDelete("cache")]
    public async Task<ActionResult> ClearAnalyticsCache()
    {
        await _analyticsService.ClearAnalyticsCacheAsync();
        return Ok(new { Message = "Analytics cache cleared successfully" });
    }

    /// <summary>
    /// Get cached analytics
    /// </summary>
    [HttpGet("cache/{cacheKey}")]
    public async Task<ActionResult<Dictionary<string, object>>> GetCachedAnalytics(string cacheKey)
    {
        try
        {
            var data = await _analyticsService.GetCachedAnalyticsAsync(cacheKey);
            return Ok(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
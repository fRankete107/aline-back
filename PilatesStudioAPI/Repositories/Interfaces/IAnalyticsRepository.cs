using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Repositories.Interfaces;

public interface IAnalyticsRepository
{
    // Dashboard Principal
    Task<DashboardStatsDto> GetDashboardStatsAsync(AnalyticsFilterDto? filter = null);
    Task<RealTimeKpiDto> GetRealTimeKpisAsync();

    // Analytics de Estudiantes
    Task<StudentAnalyticsDto> GetStudentAnalyticsAsync(AnalyticsFilterDto? filter = null);
    Task<List<TopStudentDto>> GetTopStudentsByReservationsAsync(int limit = 10);
    Task<List<TopStudentDto>> GetTopStudentsByPaymentsAsync(int limit = 10);
    Task<Dictionary<string, int>> GetStudentsByAgeGroupAsync();
    Task<Dictionary<string, int>> GetStudentRegistrationsByMonthAsync(int months = 12);

    // Analytics de Clases
    Task<ClassAnalyticsDto> GetClassAnalyticsAsync(AnalyticsFilterDto? filter = null);
    Task<Dictionary<string, int>> GetClassesByTypeAsync(AnalyticsFilterDto? filter = null);
    Task<Dictionary<string, int>> GetClassesByLevelAsync(AnalyticsFilterDto? filter = null);
    Task<Dictionary<string, decimal>> GetCapacityUtilizationByZoneAsync(AnalyticsFilterDto? filter = null);
    Task<List<PopularClassTimeDto>> GetPopularClassTimesAsync(int limit = 10);
    Task<List<InstructorClassStatsDto>> GetInstructorStatsAsync(AnalyticsFilterDto? filter = null);

    // Analytics de Reservas
    Task<ReservationAnalyticsDto> GetReservationAnalyticsAsync(AnalyticsFilterDto? filter = null);
    Task<Dictionary<string, int>> GetReservationsByStatusAsync(AnalyticsFilterDto? filter = null);
    Task<Dictionary<string, int>> GetReservationsByMonthAsync(int months = 12);
    Task<Dictionary<string, int>> GetReservationsByDayOfWeekAsync(AnalyticsFilterDto? filter = null);
    Task<List<PeakHoursDto>> GetPeakHoursAnalysisAsync(AnalyticsFilterDto? filter = null);

    // Analytics Financieros
    Task<FinancialAnalyticsDto> GetFinancialAnalyticsAsync(AnalyticsFilterDto? filter = null);
    Task<Dictionary<string, decimal>> GetRevenueByPaymentMethodAsync(AnalyticsFilterDto? filter = null);
    Task<Dictionary<string, decimal>> GetRevenueByPlanAsync(AnalyticsFilterDto? filter = null);
    Task<List<RevenueByPeriodDto>> GetRevenueByMonthAsync(int months = 12);
    Task<List<RevenueByPeriodDto>> GetRevenueByWeekAsync(int weeks = 12);
    Task<Dictionary<string, decimal>> GetMonthlyRevenueGrowthAsync(int months = 12);

    // Analytics de Suscripciones
    Task<SubscriptionAnalyticsDto> GetSubscriptionAnalyticsAsync(AnalyticsFilterDto? filter = null);
    Task<List<PlanPopularityDto>> GetPlanPopularityAsync(AnalyticsFilterDto? filter = null);
    Task<List<SubscriptionTrendDto>> GetSubscriptionTrendsAsync(int months = 12);
    Task<Dictionary<string, int>> GetSubscriptionsByStatusAsync(AnalyticsFilterDto? filter = null);

    // Comparaciones de Períodos
    Task<PeriodComparisonDto> GetPeriodComparisonAsync(string period, AnalyticsFilterDto? filter = null);
    Task<AnalyticsPeriodDataDto> GetPeriodDataAsync(DateTime startDate, DateTime endDate, AnalyticsFilterDto? filter = null);

    // Métricas de Rendimiento
    Task<decimal> GetAverageClassCapacityUtilizationAsync(AnalyticsFilterDto? filter = null);
    Task<decimal> GetCustomerRetentionRateAsync(int months = 12);
    Task<decimal> GetSubscriptionRenewalRateAsync(int months = 12);
    Task<decimal> GetAverageRevenuePerUserAsync(AnalyticsFilterDto? filter = null);

    // Análisis de Tendencias
    Task<Dictionary<string, object>> GetTrendAnalysisAsync(string metric, string period, int periods = 12);
    Task<Dictionary<string, decimal>> GetSeasonalityAnalysisAsync(string metric);
    Task<List<object>> GetForecastAsync(string metric, int periods = 6);

    // Métricas Específicas por Entidad
    Task<Dictionary<string, object>> GetInstructorPerformanceAsync(long instructorId, AnalyticsFilterDto? filter = null);
    Task<Dictionary<string, object>> GetZonePerformanceAsync(long zoneId, AnalyticsFilterDto? filter = null);
    Task<Dictionary<string, object>> GetPlanPerformanceAsync(long planId, AnalyticsFilterDto? filter = null);
    Task<Dictionary<string, object>> GetStudentActivityAsync(long studentId, AnalyticsFilterDto? filter = null);
}
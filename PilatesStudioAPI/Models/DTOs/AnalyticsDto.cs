using System.ComponentModel.DataAnnotations;

namespace PilatesStudioAPI.Models.DTOs;

// Dashboard General
public class DashboardStatsDto
{
    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int TotalInstructors { get; set; }
    public int ActiveInstructors { get; set; }
    public int TotalClasses { get; set; }
    public int TotalReservations { get; set; }
    public int ActiveSubscriptions { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal AverageClassCapacity { get; set; }
    public int ClassesThisWeek { get; set; }
    public int ReservationsThisWeek { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

// Métricas de Estudiantes
public class StudentAnalyticsDto
{
    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int NewStudentsThisMonth { get; set; }
    public int StudentsWithActiveSubscriptions { get; set; }
    public decimal AverageReservationsPerStudent { get; set; }
    public Dictionary<string, int> StudentsByAgeGroup { get; set; } = new();
    public Dictionary<string, int> StudentRegistrationsByMonth { get; set; } = new();
    public List<TopStudentDto> TopStudentsByReservations { get; set; } = new();
    public List<TopStudentDto> TopStudentsByPayments { get; set; } = new();
}

public class TopStudentDto
{
    public long StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}

// Métricas de Clases
public class ClassAnalyticsDto
{
    public int TotalClasses { get; set; }
    public int ClassesThisMonth { get; set; }
    public decimal AverageCapacityUtilization { get; set; }
    public Dictionary<string, int> ClassesByType { get; set; } = new();
    public Dictionary<string, int> ClassesByLevel { get; set; } = new();
    public Dictionary<string, int> ClassesByZone { get; set; } = new();
    public Dictionary<string, decimal> CapacityUtilizationByZone { get; set; } = new();
    public List<PopularClassTimeDto> PopularClassTimes { get; set; } = new();
    public List<InstructorClassStatsDto> InstructorStats { get; set; } = new();
}

public class PopularClassTimeDto
{
    public TimeOnly Time { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public int ClassCount { get; set; }
    public decimal AverageCapacity { get; set; }
}

public class InstructorClassStatsDto
{
    public long InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public int TotalClasses { get; set; }
    public int TotalReservations { get; set; }
    public decimal AverageCapacityUtilization { get; set; }
    public decimal Rating { get; set; }
}

// Métricas de Reservas
public class ReservationAnalyticsDto
{
    public int TotalReservations { get; set; }
    public int ReservationsThisMonth { get; set; }
    public int CompletedReservations { get; set; }
    public int CancelledReservations { get; set; }
    public int NoShowReservations { get; set; }
    public decimal CancellationRate { get; set; }
    public decimal NoShowRate { get; set; }
    public decimal CompletionRate { get; set; }
    public Dictionary<string, int> ReservationsByStatus { get; set; } = new();
    public Dictionary<string, int> ReservationsByMonth { get; set; } = new();
    public Dictionary<string, int> ReservationsByDayOfWeek { get; set; } = new();
    public Dictionary<string, int> ReservationsByTimeSlot { get; set; } = new();
    public List<PeakHoursDto> PeakHours { get; set; } = new();
}

public class PeakHoursDto
{
    public int Hour { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public int ReservationCount { get; set; }
    public decimal AverageCapacityUsed { get; set; }
}

// Métricas Financieras
public class FinancialAnalyticsDto
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal YearlyRevenue { get; set; }
    public decimal AveragePaymentAmount { get; set; }
    public int TotalPayments { get; set; }
    public int PaymentsThisMonth { get; set; }
    public Dictionary<string, decimal> RevenueByPaymentMethod { get; set; } = new();
    public Dictionary<string, decimal> RevenueByPlan { get; set; } = new();
    public Dictionary<string, decimal> MonthlyRevenueGrowth { get; set; } = new();
    public List<RevenueByPeriodDto> RevenueByMonth { get; set; } = new();
    public List<RevenueByPeriodDto> RevenueByWeek { get; set; } = new();
    public decimal RefundRate { get; set; }
    public decimal TotalRefunds { get; set; }
}

public class RevenueByPeriodDto
{
    public string Period { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int PaymentCount { get; set; }
    public decimal GrowthRate { get; set; }
}

// Métricas de Suscripciones
public class SubscriptionAnalyticsDto
{
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int ExpiredSubscriptions { get; set; }
    public int ExpiringSoon { get; set; }
    public decimal AverageSubscriptionDuration { get; set; }
    public decimal SubscriptionRenewalRate { get; set; }
    public Dictionary<string, int> SubscriptionsByPlan { get; set; } = new();
    public Dictionary<string, int> SubscriptionsByStatus { get; set; } = new();
    public List<PlanPopularityDto> PlanPopularity { get; set; } = new();
    public List<SubscriptionTrendDto> SubscriptionTrends { get; set; } = new();
}

public class PlanPopularityDto
{
    public long PlanId { get; set; }
    public string PlanTitle { get; set; } = string.Empty;
    public decimal PlanPrice { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int TotalSubscriptions { get; set; }
    public decimal Revenue { get; set; }
}

public class SubscriptionTrendDto
{
    public string Month { get; set; } = string.Empty;
    public int NewSubscriptions { get; set; }
    public int RenewedSubscriptions { get; set; }
    public int ExpiredSubscriptions { get; set; }
    public int CancelledSubscriptions { get; set; }
}

// Filtros para Analytics
public class AnalyticsFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public long? InstructorId { get; set; }
    public long? ZoneId { get; set; }
    public long? PlanId { get; set; }
    public string? ClassType { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Period { get; set; } // daily, weekly, monthly, yearly
    public int Limit { get; set; } = 10;

    [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
    public int? Month { get; set; }

    [Range(2020, 2030, ErrorMessage = "Year must be between 2020 and 2030")]
    public int? Year { get; set; }
}

// Comparación de Períodos
public class PeriodComparisonDto
{
    public string Period { get; set; } = string.Empty;
    public AnalyticsPeriodDataDto CurrentPeriod { get; set; } = new();
    public AnalyticsPeriodDataDto PreviousPeriod { get; set; } = new();
    public AnalyticsComparisonDto Comparison { get; set; } = new();
}

public class AnalyticsPeriodDataDto
{
    public decimal Revenue { get; set; }
    public int Students { get; set; }
    public int Classes { get; set; }
    public int Reservations { get; set; }
    public int Payments { get; set; }
    public decimal CapacityUtilization { get; set; }
}

public class AnalyticsComparisonDto
{
    public decimal RevenueGrowth { get; set; }
    public decimal StudentGrowth { get; set; }
    public decimal ClassGrowth { get; set; }
    public decimal ReservationGrowth { get; set; }
    public decimal PaymentGrowth { get; set; }
    public decimal CapacityGrowth { get; set; }
}

// Reportes Exportables
public class ReportRequestDto
{
    [Required]
    public string ReportType { get; set; } = string.Empty; // revenue, students, classes, reservations

    [Required]
    public string Format { get; set; } = string.Empty; // pdf, excel, csv

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public long? InstructorId { get; set; }
    public long? ZoneId { get; set; }
    public string? GroupBy { get; set; } // day, week, month, year
    public bool IncludeCharts { get; set; } = false;
    public List<string> Metrics { get; set; } = new();
}

public class ReportResponseDto
{
    public string ReportId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

// KPIs en Tiempo Real
public class RealTimeKpiDto
{
    public int OngoingClasses { get; set; }
    public int TodayReservations { get; set; }
    public int TodayCompletedClasses { get; set; }
    public int TodayNoShows { get; set; }
    public decimal TodayRevenue { get; set; }
    public int ActiveInstructors { get; set; }
    public decimal CurrentCapacityUtilization { get; set; }
    public int StudentsOnline { get; set; } // If implementing real-time features
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

// Export Request DTO
public class ExportAnalyticsRequestDto
{
    public string ReportType { get; set; } = string.Empty;
    public string Format { get; set; } = "excel";
    public AnalyticsFilterDto? Filter { get; set; }
}
using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Services.Interfaces;

public interface IReportsService
{
    // Generate Reports
    Task<ReportResponseDto> GenerateReportAsync(ReportRequestDto request);
    Task<byte[]> GenerateAnalyticsReportAsync(string reportType, AnalyticsFilterDto? filter = null, string format = "pdf");
    
    // PDF Reports
    Task<byte[]> GenerateDashboardReportPdfAsync(DashboardStatsDto dashboardStats, AnalyticsFilterDto? filter = null);
    Task<byte[]> GenerateStudentReportPdfAsync(StudentAnalyticsDto studentAnalytics, AnalyticsFilterDto? filter = null);
    Task<byte[]> GenerateClassReportPdfAsync(ClassAnalyticsDto classAnalytics, AnalyticsFilterDto? filter = null);
    Task<byte[]> GenerateFinancialReportPdfAsync(FinancialAnalyticsDto financialAnalytics, AnalyticsFilterDto? filter = null);
    Task<byte[]> GenerateReservationReportPdfAsync(ReservationAnalyticsDto reservationAnalytics, AnalyticsFilterDto? filter = null);
    Task<byte[]> GenerateSubscriptionReportPdfAsync(SubscriptionAnalyticsDto subscriptionAnalytics, AnalyticsFilterDto? filter = null);

    // Excel Reports
    Task<byte[]> GenerateDashboardReportExcelAsync(DashboardStatsDto dashboardStats, AnalyticsFilterDto? filter = null);
    Task<byte[]> GenerateStudentReportExcelAsync(StudentAnalyticsDto studentAnalytics, AnalyticsFilterDto? filter = null);
    Task<byte[]> GenerateClassReportExcelAsync(ClassAnalyticsDto classAnalytics, AnalyticsFilterDto? filter = null);
    Task<byte[]> GenerateFinancialReportExcelAsync(FinancialAnalyticsDto financialAnalytics, AnalyticsFilterDto? filter = null);
    Task<byte[]> GenerateReservationReportExcelAsync(ReservationAnalyticsDto reservationAnalytics, AnalyticsFilterDto? filter = null);
    Task<byte[]> GenerateSubscriptionReportExcelAsync(SubscriptionAnalyticsDto subscriptionAnalytics, AnalyticsFilterDto? filter = null);

    // CSV Reports
    Task<byte[]> GenerateStudentReportCsvAsync(List<TopStudentDto> students);
    Task<byte[]> GenerateRevenueReportCsvAsync(List<RevenueByPeriodDto> revenue);
    Task<byte[]> GenerateReservationReportCsvAsync(ReservationAnalyticsDto reservationAnalytics);
    Task<byte[]> GenerateInstructorStatsReportCsvAsync(List<InstructorClassStatsDto> instructorStats);

    // Custom Reports
    Task<byte[]> GenerateCustomReportAsync(string reportTemplate, Dictionary<string, object> data, string format = "pdf");
    Task<byte[]> GeneratePeriodComparisonReportAsync(PeriodComparisonDto comparison, string format = "pdf");

    // Report Management
    Task<List<ReportResponseDto>> GetAvailableReportsAsync();
    Task<ReportResponseDto?> GetReportByIdAsync(string reportId);
    Task<bool> DeleteReportAsync(string reportId);
    Task<string> SaveReportAsync(byte[] reportData, string fileName, string format);

    // Report Templates
    Task<List<string>> GetAvailableTemplatesAsync();
    Task<byte[]> GetReportTemplateAsync(string templateName);
    Task<bool> SaveReportTemplateAsync(string templateName, byte[] templateData);

    // Utilities
    Task<string> GetContentTypeByFormatAsync(string format);
    Task<bool> ValidateReportRequestAsync(ReportRequestDto request);
}
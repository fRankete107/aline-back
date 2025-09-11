using System.Text;
using System.Text.Json;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Services.Implementations;

public class ReportsService : IReportsService
{
    private readonly IAnalyticsService _analyticsService;
    private readonly string _reportsPath;
    private readonly string _templatesPath;

    public ReportsService(IAnalyticsService analyticsService, IConfiguration configuration)
    {
        _analyticsService = analyticsService;
        _reportsPath = configuration["ReportsSettings:ReportsPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Reports");
        _templatesPath = configuration["ReportsSettings:TemplatesPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Templates");
        
        // Ensure directories exist
        Directory.CreateDirectory(_reportsPath);
        Directory.CreateDirectory(_templatesPath);
    }

    public async Task<ReportResponseDto> GenerateReportAsync(ReportRequestDto request)
    {
        if (!await ValidateReportRequestAsync(request))
            throw new ArgumentException("Invalid report request");

        var reportData = await GenerateAnalyticsReportAsync(request.ReportType, 
            new AnalyticsFilterDto 
            { 
                StartDate = request.StartDate, 
                EndDate = request.EndDate,
                InstructorId = request.InstructorId,
                ZoneId = request.ZoneId
            }, 
            request.Format);

        var fileName = $"{request.ReportType}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{request.Format}";
        var reportId = await SaveReportAsync(reportData, fileName, request.Format);

        return new ReportResponseDto
        {
            ReportId = reportId,
            FileName = fileName,
            Format = request.Format,
            FileSize = reportData.Length,
            GeneratedAt = DateTime.UtcNow,
            DownloadUrl = $"/api/reports/download/{reportId}",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
    }

    public async Task<byte[]> GenerateAnalyticsReportAsync(string reportType, AnalyticsFilterDto? filter = null, string format = "pdf")
    {
        return reportType.ToLower() switch
        {
            "dashboard" => await GenerateDashboardReport(filter, format),
            "students" => await GenerateStudentReport(filter, format),
            "classes" => await GenerateClassReport(filter, format),
            "financial" => await GenerateFinancialReport(filter, format),
            "reservations" => await GenerateReservationReport(filter, format),
            "subscriptions" => await GenerateSubscriptionReport(filter, format),
            _ => throw new ArgumentException($"Unknown report type: {reportType}")
        };
    }

    // Dashboard Reports
    private async Task<byte[]> GenerateDashboardReport(AnalyticsFilterDto? filter, string format)
    {
        var dashboardStats = await _analyticsService.GetDashboardStatsAsync(filter);
        
        return format.ToLower() switch
        {
            "pdf" => await GenerateDashboardReportPdfAsync(dashboardStats, filter),
            "excel" => await GenerateDashboardReportExcelAsync(dashboardStats, filter),
            _ => throw new ArgumentException($"Unsupported format for dashboard report: {format}")
        };
    }

    public async Task<byte[]> GenerateDashboardReportPdfAsync(DashboardStatsDto dashboardStats, AnalyticsFilterDto? filter = null)
    {
        var html = GenerateDashboardHtml(dashboardStats, filter);
        return await ConvertHtmlToPdfAsync(html);
    }

    public async Task<byte[]> GenerateDashboardReportExcelAsync(DashboardStatsDto dashboardStats, AnalyticsFilterDto? filter = null)
    {
        // For now, return CSV-like content as Excel implementation would require additional libraries
        var csv = GenerateDashboardCsv(dashboardStats);
        return Encoding.UTF8.GetBytes(csv);
    }

    // Student Reports
    private async Task<byte[]> GenerateStudentReport(AnalyticsFilterDto? filter, string format)
    {
        var studentAnalytics = await _analyticsService.GetStudentAnalyticsAsync(filter);
        
        return format.ToLower() switch
        {
            "pdf" => await GenerateStudentReportPdfAsync(studentAnalytics, filter),
            "excel" => await GenerateStudentReportExcelAsync(studentAnalytics, filter),
            "csv" => await GenerateStudentReportCsvAsync(studentAnalytics.TopStudentsByReservations),
            _ => throw new ArgumentException($"Unsupported format for student report: {format}")
        };
    }

    public async Task<byte[]> GenerateStudentReportPdfAsync(StudentAnalyticsDto studentAnalytics, AnalyticsFilterDto? filter = null)
    {
        var html = GenerateStudentAnalyticsHtml(studentAnalytics, filter);
        return await ConvertHtmlToPdfAsync(html);
    }

    public async Task<byte[]> GenerateStudentReportExcelAsync(StudentAnalyticsDto studentAnalytics, AnalyticsFilterDto? filter = null)
    {
        var csv = GenerateStudentAnalyticsCsv(studentAnalytics);
        return Encoding.UTF8.GetBytes(csv);
    }

    public async Task<byte[]> GenerateStudentReportCsvAsync(List<TopStudentDto> students)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Student Name,Email,Reservations,Total Amount");
        
        foreach (var student in students)
        {
            csv.AppendLine($"\"{student.StudentName}\",\"{student.Email}\",{student.Count},{student.TotalAmount:F2}");
        }
        
        return await Task.FromResult(Encoding.UTF8.GetBytes(csv.ToString()));
    }

    // Class Reports
    private async Task<byte[]> GenerateClassReport(AnalyticsFilterDto? filter, string format)
    {
        var classAnalytics = await _analyticsService.GetClassAnalyticsAsync(filter);
        
        return format.ToLower() switch
        {
            "pdf" => await GenerateClassReportPdfAsync(classAnalytics, filter),
            "excel" => await GenerateClassReportExcelAsync(classAnalytics, filter),
            _ => throw new ArgumentException($"Unsupported format for class report: {format}")
        };
    }

    public async Task<byte[]> GenerateClassReportPdfAsync(ClassAnalyticsDto classAnalytics, AnalyticsFilterDto? filter = null)
    {
        var html = GenerateClassAnalyticsHtml(classAnalytics, filter);
        return await ConvertHtmlToPdfAsync(html);
    }

    public async Task<byte[]> GenerateClassReportExcelAsync(ClassAnalyticsDto classAnalytics, AnalyticsFilterDto? filter = null)
    {
        var csv = GenerateClassAnalyticsCsv(classAnalytics);
        return Encoding.UTF8.GetBytes(csv);
    }

    // Financial Reports
    private async Task<byte[]> GenerateFinancialReport(AnalyticsFilterDto? filter, string format)
    {
        var financialAnalytics = await _analyticsService.GetFinancialAnalyticsAsync(filter);
        
        return format.ToLower() switch
        {
            "pdf" => await GenerateFinancialReportPdfAsync(financialAnalytics, filter),
            "excel" => await GenerateFinancialReportExcelAsync(financialAnalytics, filter),
            _ => throw new ArgumentException($"Unsupported format for financial report: {format}")
        };
    }

    public async Task<byte[]> GenerateFinancialReportPdfAsync(FinancialAnalyticsDto financialAnalytics, AnalyticsFilterDto? filter = null)
    {
        var html = GenerateFinancialAnalyticsHtml(financialAnalytics, filter);
        return await ConvertHtmlToPdfAsync(html);
    }

    public async Task<byte[]> GenerateFinancialReportExcelAsync(FinancialAnalyticsDto financialAnalytics, AnalyticsFilterDto? filter = null)
    {
        var csv = GenerateFinancialAnalyticsCsv(financialAnalytics);
        return Encoding.UTF8.GetBytes(csv);
    }

    // Reservation Reports
    private async Task<byte[]> GenerateReservationReport(AnalyticsFilterDto? filter, string format)
    {
        var reservationAnalytics = await _analyticsService.GetReservationAnalyticsAsync(filter);
        
        return format.ToLower() switch
        {
            "pdf" => await GenerateReservationReportPdfAsync(reservationAnalytics, filter),
            "excel" => await GenerateReservationReportExcelAsync(reservationAnalytics, filter),
            "csv" => await GenerateReservationReportCsvAsync(reservationAnalytics),
            _ => throw new ArgumentException($"Unsupported format for reservation report: {format}")
        };
    }

    public async Task<byte[]> GenerateReservationReportPdfAsync(ReservationAnalyticsDto reservationAnalytics, AnalyticsFilterDto? filter = null)
    {
        var html = GenerateReservationAnalyticsHtml(reservationAnalytics, filter);
        return await ConvertHtmlToPdfAsync(html);
    }

    public async Task<byte[]> GenerateReservationReportExcelAsync(ReservationAnalyticsDto reservationAnalytics, AnalyticsFilterDto? filter = null)
    {
        var csv = GenerateReservationAnalyticsCsv(reservationAnalytics);
        return Encoding.UTF8.GetBytes(csv);
    }

    public async Task<byte[]> GenerateReservationReportCsvAsync(ReservationAnalyticsDto reservationAnalytics)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Metric,Value");
        csv.AppendLine($"Total Reservations,{reservationAnalytics.TotalReservations}");
        csv.AppendLine($"Completed Reservations,{reservationAnalytics.CompletedReservations}");
        csv.AppendLine($"Cancelled Reservations,{reservationAnalytics.CancelledReservations}");
        csv.AppendLine($"No Show Reservations,{reservationAnalytics.NoShowReservations}");
        csv.AppendLine($"Cancellation Rate,{reservationAnalytics.CancellationRate:F2}%");
        csv.AppendLine($"No Show Rate,{reservationAnalytics.NoShowRate:F2}%");
        csv.AppendLine($"Completion Rate,{reservationAnalytics.CompletionRate:F2}%");
        
        return await Task.FromResult(Encoding.UTF8.GetBytes(csv.ToString()));
    }

    // Subscription Reports
    private async Task<byte[]> GenerateSubscriptionReport(AnalyticsFilterDto? filter, string format)
    {
        var subscriptionAnalytics = await _analyticsService.GetSubscriptionAnalyticsAsync(filter);
        
        return format.ToLower() switch
        {
            "pdf" => await GenerateSubscriptionReportPdfAsync(subscriptionAnalytics, filter),
            "excel" => await GenerateSubscriptionReportExcelAsync(subscriptionAnalytics, filter),
            _ => throw new ArgumentException($"Unsupported format for subscription report: {format}")
        };
    }

    public async Task<byte[]> GenerateSubscriptionReportPdfAsync(SubscriptionAnalyticsDto subscriptionAnalytics, AnalyticsFilterDto? filter = null)
    {
        var html = GenerateSubscriptionAnalyticsHtml(subscriptionAnalytics, filter);
        return await ConvertHtmlToPdfAsync(html);
    }

    public async Task<byte[]> GenerateSubscriptionReportExcelAsync(SubscriptionAnalyticsDto subscriptionAnalytics, AnalyticsFilterDto? filter = null)
    {
        var csv = GenerateSubscriptionAnalyticsCsv(subscriptionAnalytics);
        return Encoding.UTF8.GetBytes(csv);
    }

    // Other CSV Reports
    public async Task<byte[]> GenerateRevenueReportCsvAsync(List<RevenueByPeriodDto> revenue)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Period,Revenue,Payment Count,Growth Rate");
        
        foreach (var period in revenue)
        {
            csv.AppendLine($"\"{period.Period}\",{period.Revenue:F2},{period.PaymentCount},{period.GrowthRate:F2}%");
        }
        
        return await Task.FromResult(Encoding.UTF8.GetBytes(csv.ToString()));
    }

    public async Task<byte[]> GenerateInstructorStatsReportCsvAsync(List<InstructorClassStatsDto> instructorStats)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Instructor Name,Total Classes,Total Reservations,Capacity Utilization,Rating");
        
        foreach (var instructor in instructorStats)
        {
            csv.AppendLine($"\"{instructor.InstructorName}\",{instructor.TotalClasses},{instructor.TotalReservations},{instructor.AverageCapacityUtilization:F2}%,{instructor.Rating:F1}");
        }
        
        return await Task.FromResult(Encoding.UTF8.GetBytes(csv.ToString()));
    }

    // Custom and Period Comparison Reports
    public async Task<byte[]> GenerateCustomReportAsync(string reportTemplate, Dictionary<string, object> data, string format = "pdf")
    {
        // Implementation would use a template engine like Razor or Liquid
        var html = $"<html><body><h1>Custom Report: {reportTemplate}</h1><p>Data: {JsonSerializer.Serialize(data)}</p></body></html>";
        
        return format.ToLower() switch
        {
            "pdf" => await ConvertHtmlToPdfAsync(html),
            _ => Encoding.UTF8.GetBytes(html)
        };
    }

    public async Task<byte[]> GeneratePeriodComparisonReportAsync(PeriodComparisonDto comparison, string format = "pdf")
    {
        var html = GeneratePeriodComparisonHtml(comparison);
        
        return format.ToLower() switch
        {
            "pdf" => await ConvertHtmlToPdfAsync(html),
            _ => Encoding.UTF8.GetBytes(html)
        };
    }

    // Report Management
    public async Task<List<ReportResponseDto>> GetAvailableReportsAsync()
    {
        var reports = new List<ReportResponseDto>();
        
        if (Directory.Exists(_reportsPath))
        {
            var files = Directory.GetFiles(_reportsPath);
            
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var fileName = Path.GetFileNameWithoutExtension(file);
                var extension = Path.GetExtension(file).TrimStart('.');
                
                reports.Add(new ReportResponseDto
                {
                    ReportId = fileName,
                    FileName = fileInfo.Name,
                    Format = extension,
                    FileSize = fileInfo.Length,
                    GeneratedAt = fileInfo.CreationTime,
                    DownloadUrl = $"/api/reports/download/{fileName}",
                    ExpiresAt = fileInfo.CreationTime.AddDays(7)
                });
            }
        }
        
        return await Task.FromResult(reports.OrderByDescending(r => r.GeneratedAt).ToList());
    }

    public async Task<ReportResponseDto?> GetReportByIdAsync(string reportId)
    {
        var reports = await GetAvailableReportsAsync();
        return reports.FirstOrDefault(r => r.ReportId == reportId);
    }

    public async Task<bool> DeleteReportAsync(string reportId)
    {
        var files = Directory.GetFiles(_reportsPath, $"{reportId}.*");
        
        foreach (var file in files)
        {
            File.Delete(file);
        }
        
        return await Task.FromResult(files.Length > 0);
    }

    public async Task<string> SaveReportAsync(byte[] reportData, string fileName, string format)
    {
        var reportId = Path.GetFileNameWithoutExtension(fileName);
        var filePath = Path.Combine(_reportsPath, fileName);
        
        await File.WriteAllBytesAsync(filePath, reportData);
        
        return reportId;
    }

    // Templates
    public async Task<List<string>> GetAvailableTemplatesAsync()
    {
        var templates = new List<string>();
        
        if (Directory.Exists(_templatesPath))
        {
            var files = Directory.GetFiles(_templatesPath, "*.html");
            templates.AddRange(files.Select(Path.GetFileNameWithoutExtension));
        }
        
        return await Task.FromResult(templates);
    }

    public async Task<byte[]> GetReportTemplateAsync(string templateName)
    {
        var templatePath = Path.Combine(_templatesPath, $"{templateName}.html");
        
        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Template '{templateName}' not found");
        
        return await File.ReadAllBytesAsync(templatePath);
    }

    public async Task<bool> SaveReportTemplateAsync(string templateName, byte[] templateData)
    {
        var templatePath = Path.Combine(_templatesPath, $"{templateName}.html");
        
        await File.WriteAllBytesAsync(templatePath, templateData);
        
        return true;
    }

    // Utilities
    public async Task<string> GetContentTypeByFormatAsync(string format)
    {
        var contentType = format.ToLower() switch
        {
            "pdf" => "application/pdf",
            "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "csv" => "text/csv",
            "html" => "text/html",
            _ => "application/octet-stream"
        };
        
        return await Task.FromResult(contentType);
    }

    public async Task<bool> ValidateReportRequestAsync(ReportRequestDto request)
    {
        if (string.IsNullOrEmpty(request.ReportType))
            return false;
        
        if (string.IsNullOrEmpty(request.Format))
            return false;
        
        var validReportTypes = new[] { "dashboard", "students", "classes", "financial", "reservations", "subscriptions" };
        if (!validReportTypes.Contains(request.ReportType.ToLower()))
            return false;
        
        var validFormats = new[] { "pdf", "excel", "csv" };
        if (!validFormats.Contains(request.Format.ToLower()))
            return false;
        
        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            if (request.StartDate > request.EndDate)
                return false;
        }
        
        return await Task.FromResult(true);
    }

    // HTML Generation Methods (simplified implementations)
    private string GenerateDashboardHtml(DashboardStatsDto stats, AnalyticsFilterDto? filter)
    {
        return $@"
        <html>
        <head>
            <title>Dashboard Report</title>
            <style>
                body {{ font-family: Arial, sans-serif; margin: 20px; }}
                .header {{ text-align: center; margin-bottom: 30px; }}
                .stats-grid {{ display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 20px; }}
                .stat-card {{ border: 1px solid #ddd; padding: 15px; border-radius: 5px; }}
                .stat-title {{ font-weight: bold; color: #333; }}
                .stat-value {{ font-size: 24px; color: #007bff; margin-top: 5px; }}
            </style>
        </head>
        <body>
            <div class='header'>
                <h1>Dashboard Report</h1>
                <p>Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                {(filter?.StartDate.HasValue == true ? $"<p>Period: {filter.StartDate:yyyy-MM-dd} to {filter.EndDate:yyyy-MM-dd}</p>" : "")}
            </div>
            <div class='stats-grid'>
                <div class='stat-card'>
                    <div class='stat-title'>Total Students</div>
                    <div class='stat-value'>{stats.TotalStudents}</div>
                </div>
                <div class='stat-card'>
                    <div class='stat-title'>Active Students</div>
                    <div class='stat-value'>{stats.ActiveStudents}</div>
                </div>
                <div class='stat-card'>
                    <div class='stat-title'>Total Instructors</div>
                    <div class='stat-value'>{stats.TotalInstructors}</div>
                </div>
                <div class='stat-card'>
                    <div class='stat-title'>Total Classes</div>
                    <div class='stat-value'>{stats.TotalClasses}</div>
                </div>
                <div class='stat-card'>
                    <div class='stat-title'>Total Revenue</div>
                    <div class='stat-value'>${stats.TotalRevenue:F2}</div>
                </div>
                <div class='stat-card'>
                    <div class='stat-title'>Monthly Revenue</div>
                    <div class='stat-value'>${stats.MonthlyRevenue:F2}</div>
                </div>
                <div class='stat-card'>
                    <div class='stat-title'>Average Capacity</div>
                    <div class='stat-value'>{stats.AverageClassCapacity:F1}%</div>
                </div>
                <div class='stat-card'>
                    <div class='stat-title'>Active Subscriptions</div>
                    <div class='stat-value'>{stats.ActiveSubscriptions}</div>
                </div>
            </div>
        </body>
        </html>";
    }

    private string GenerateDashboardCsv(DashboardStatsDto stats)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Metric,Value");
        csv.AppendLine($"Total Students,{stats.TotalStudents}");
        csv.AppendLine($"Active Students,{stats.ActiveStudents}");
        csv.AppendLine($"Total Instructors,{stats.TotalInstructors}");
        csv.AppendLine($"Active Instructors,{stats.ActiveInstructors}");
        csv.AppendLine($"Total Classes,{stats.TotalClasses}");
        csv.AppendLine($"Total Reservations,{stats.TotalReservations}");
        csv.AppendLine($"Active Subscriptions,{stats.ActiveSubscriptions}");
        csv.AppendLine($"Total Revenue,{stats.TotalRevenue:F2}");
        csv.AppendLine($"Monthly Revenue,{stats.MonthlyRevenue:F2}");
        csv.AppendLine($"Average Class Capacity,{stats.AverageClassCapacity:F2}%");
        return csv.ToString();
    }

    // Simplified implementations for other HTML generators
    private string GenerateStudentAnalyticsHtml(StudentAnalyticsDto analytics, AnalyticsFilterDto? filter)
    {
        return $"<html><body><h1>Student Analytics Report</h1><p>Total Students: {analytics.TotalStudents}</p><p>Active Students: {analytics.ActiveStudents}</p></body></html>";
    }

    private string GenerateStudentAnalyticsCsv(StudentAnalyticsDto analytics)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Metric,Value");
        csv.AppendLine($"Total Students,{analytics.TotalStudents}");
        csv.AppendLine($"Active Students,{analytics.ActiveStudents}");
        csv.AppendLine($"New Students This Month,{analytics.NewStudentsThisMonth}");
        return csv.ToString();
    }

    private string GenerateClassAnalyticsHtml(ClassAnalyticsDto analytics, AnalyticsFilterDto? filter)
    {
        return $"<html><body><h1>Class Analytics Report</h1><p>Total Classes: {analytics.TotalClasses}</p><p>Average Capacity Utilization: {analytics.AverageCapacityUtilization:F2}%</p></body></html>";
    }

    private string GenerateClassAnalyticsCsv(ClassAnalyticsDto analytics)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Metric,Value");
        csv.AppendLine($"Total Classes,{analytics.TotalClasses}");
        csv.AppendLine($"Classes This Month,{analytics.ClassesThisMonth}");
        csv.AppendLine($"Average Capacity Utilization,{analytics.AverageCapacityUtilization:F2}%");
        return csv.ToString();
    }

    private string GenerateFinancialAnalyticsHtml(FinancialAnalyticsDto analytics, AnalyticsFilterDto? filter)
    {
        return $"<html><body><h1>Financial Analytics Report</h1><p>Total Revenue: ${analytics.TotalRevenue:F2}</p><p>Monthly Revenue: ${analytics.MonthlyRevenue:F2}</p></body></html>";
    }

    private string GenerateFinancialAnalyticsCsv(FinancialAnalyticsDto analytics)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Metric,Value");
        csv.AppendLine($"Total Revenue,{analytics.TotalRevenue:F2}");
        csv.AppendLine($"Monthly Revenue,{analytics.MonthlyRevenue:F2}");
        csv.AppendLine($"Yearly Revenue,{analytics.YearlyRevenue:F2}");
        csv.AppendLine($"Average Payment Amount,{analytics.AveragePaymentAmount:F2}");
        return csv.ToString();
    }

    private string GenerateReservationAnalyticsHtml(ReservationAnalyticsDto analytics, AnalyticsFilterDto? filter)
    {
        return $"<html><body><h1>Reservation Analytics Report</h1><p>Total Reservations: {analytics.TotalReservations}</p><p>Completion Rate: {analytics.CompletionRate:F2}%</p></body></html>";
    }

    private string GenerateReservationAnalyticsCsv(ReservationAnalyticsDto analytics)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Metric,Value");
        csv.AppendLine($"Total Reservations,{analytics.TotalReservations}");
        csv.AppendLine($"Completed Reservations,{analytics.CompletedReservations}");
        csv.AppendLine($"Cancelled Reservations,{analytics.CancelledReservations}");
        csv.AppendLine($"Completion Rate,{analytics.CompletionRate:F2}%");
        return csv.ToString();
    }

    private string GenerateSubscriptionAnalyticsHtml(SubscriptionAnalyticsDto analytics, AnalyticsFilterDto? filter)
    {
        return $"<html><body><h1>Subscription Analytics Report</h1><p>Total Subscriptions: {analytics.TotalSubscriptions}</p><p>Active Subscriptions: {analytics.ActiveSubscriptions}</p></body></html>";
    }

    private string GenerateSubscriptionAnalyticsCsv(SubscriptionAnalyticsDto analytics)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Metric,Value");
        csv.AppendLine($"Total Subscriptions,{analytics.TotalSubscriptions}");
        csv.AppendLine($"Active Subscriptions,{analytics.ActiveSubscriptions}");
        csv.AppendLine($"Expired Subscriptions,{analytics.ExpiredSubscriptions}");
        return csv.ToString();
    }

    private string GeneratePeriodComparisonHtml(PeriodComparisonDto comparison)
    {
        return $@"
        <html>
        <body>
            <h1>Period Comparison Report - {comparison.Period}</h1>
            <h2>Current Period vs Previous Period</h2>
            <table border='1'>
                <tr><th>Metric</th><th>Current</th><th>Previous</th><th>Growth</th></tr>
                <tr><td>Revenue</td><td>${comparison.CurrentPeriod.Revenue:F2}</td><td>${comparison.PreviousPeriod.Revenue:F2}</td><td>{comparison.Comparison.RevenueGrowth:F2}%</td></tr>
                <tr><td>Students</td><td>{comparison.CurrentPeriod.Students}</td><td>{comparison.PreviousPeriod.Students}</td><td>{comparison.Comparison.StudentGrowth:F2}%</td></tr>
            </table>
        </body>
        </html>";
    }

    private async Task<byte[]> ConvertHtmlToPdfAsync(string html)
    {
        // For this implementation, we'll return the HTML as bytes
        // In a real implementation, you would use a library like PuppeteerSharp or DinkToPdf
        return await Task.FromResult(Encoding.UTF8.GetBytes(html));
    }
}
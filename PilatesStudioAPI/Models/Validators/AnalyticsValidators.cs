using FluentValidation;
using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Models.Validators;

public class AnalyticsFilterValidator : AbstractValidator<AnalyticsFilterDto>
{
    public AnalyticsFilterValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("StartDate must be less than or equal to EndDate");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("EndDate must be greater than or equal to StartDate");

        // Don't allow date ranges more than 2 years apart
        RuleFor(x => x)
            .Must(HaveValidDateRange)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Date range cannot exceed 2 years");

        RuleFor(x => x.InstructorId)
            .GreaterThan(0)
            .When(x => x.InstructorId.HasValue)
            .WithMessage("InstructorId must be a positive number");

        RuleFor(x => x.ZoneId)
            .GreaterThan(0)
            .When(x => x.ZoneId.HasValue)
            .WithMessage("ZoneId must be a positive number");

        RuleFor(x => x.PlanId)
            .GreaterThan(0)
            .When(x => x.PlanId.HasValue)
            .WithMessage("PlanId must be a positive number");

        RuleFor(x => x.ClassType)
            .MaximumLength(50)
            .WithMessage("ClassType cannot exceed 50 characters");

        RuleFor(x => x.PaymentMethod)
            .Must(BeValidPaymentMethod)
            .When(x => !string.IsNullOrEmpty(x.PaymentMethod))
            .WithMessage("PaymentMethod must be one of: cash, credit_card, debit_card, bank_transfer, digital_wallet");

        RuleFor(x => x.Period)
            .Must(BeValidPeriod)
            .When(x => !string.IsNullOrEmpty(x.Period))
            .WithMessage("Period must be one of: daily, weekly, monthly, yearly");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 1000)
            .WithMessage("Limit must be between 1 and 1000");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .When(x => x.Month.HasValue)
            .WithMessage("Month must be between 1 and 12");

        RuleFor(x => x.Year)
            .InclusiveBetween(2020, 2030)
            .When(x => x.Year.HasValue)
            .WithMessage("Year must be between 2020 and 2030");
    }

    private static bool HaveValidDateRange(AnalyticsFilterDto filter)
    {
        if (!filter.StartDate.HasValue || !filter.EndDate.HasValue)
            return true;

        var daysDifference = (filter.EndDate.Value - filter.StartDate.Value).TotalDays;
        return daysDifference <= 730; // 2 years
    }

    private static bool BeValidPaymentMethod(string? paymentMethod)
    {
        if (string.IsNullOrEmpty(paymentMethod))
            return true;

        var validMethods = new[] { "cash", "credit_card", "debit_card", "bank_transfer", "digital_wallet" };
        return validMethods.Contains(paymentMethod.ToLowerInvariant());
    }

    private static bool BeValidPeriod(string? period)
    {
        if (string.IsNullOrEmpty(period))
            return true;

        var validPeriods = new[] { "daily", "weekly", "monthly", "yearly" };
        return validPeriods.Contains(period.ToLowerInvariant());
    }
}

public class ReportRequestValidator : AbstractValidator<ReportRequestDto>
{
    public ReportRequestValidator()
    {
        RuleFor(x => x.ReportType)
            .NotEmpty()
            .WithMessage("ReportType is required")
            .Must(BeValidReportType)
            .WithMessage("ReportType must be one of: revenue, students, classes, reservations, dashboard, subscriptions, financial");

        RuleFor(x => x.Format)
            .NotEmpty()
            .WithMessage("Format is required")
            .Must(BeValidFormat)
            .WithMessage("Format must be one of: pdf, excel, csv");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("StartDate must be less than or equal to EndDate");

        RuleFor(x => x.EndDate)
            .LessThanOrEqualTo(DateTime.UtcNow.Date)
            .When(x => x.EndDate.HasValue)
            .WithMessage("EndDate cannot be in the future");

        RuleFor(x => x)
            .Must(HaveValidDateRange)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Date range cannot exceed 2 years");

        RuleFor(x => x.InstructorId)
            .GreaterThan(0)
            .When(x => x.InstructorId.HasValue)
            .WithMessage("InstructorId must be a positive number");

        RuleFor(x => x.ZoneId)
            .GreaterThan(0)
            .When(x => x.ZoneId.HasValue)
            .WithMessage("ZoneId must be a positive number");

        RuleFor(x => x.GroupBy)
            .Must(BeValidGroupBy)
            .When(x => !string.IsNullOrEmpty(x.GroupBy))
            .WithMessage("GroupBy must be one of: day, week, month, year");

        RuleFor(x => x.Metrics)
            .Must(HaveValidMetrics)
            .When(x => x.Metrics != null && x.Metrics.Count > 0)
            .WithMessage("All metrics must be valid");
    }

    private static bool BeValidReportType(string reportType)
    {
        if (string.IsNullOrEmpty(reportType))
            return false;

        var validTypes = new[] { "revenue", "students", "classes", "reservations", "dashboard", "subscriptions", "financial" };
        return validTypes.Contains(reportType.ToLowerInvariant());
    }

    private static bool BeValidFormat(string format)
    {
        if (string.IsNullOrEmpty(format))
            return false;

        var validFormats = new[] { "pdf", "excel", "csv" };
        return validFormats.Contains(format.ToLowerInvariant());
    }

    private static bool HaveValidDateRange(ReportRequestDto request)
    {
        if (!request.StartDate.HasValue || !request.EndDate.HasValue)
            return true;

        var daysDifference = (request.EndDate.Value - request.StartDate.Value).TotalDays;
        return daysDifference <= 730; // 2 years
    }

    private static bool BeValidGroupBy(string? groupBy)
    {
        if (string.IsNullOrEmpty(groupBy))
            return true;

        var validGroupBy = new[] { "day", "week", "month", "year" };
        return validGroupBy.Contains(groupBy.ToLowerInvariant());
    }

    private static bool HaveValidMetrics(List<string> metrics)
    {
        if (metrics == null || metrics.Count == 0)
            return true;

        var validMetrics = new[] 
        { 
            "revenue", "students", "classes", "reservations", "subscriptions", 
            "capacity", "cancellation_rate", "no_show_rate", "completion_rate",
            "arpu", "retention_rate", "renewal_rate", "growth_rate" 
        };

        return metrics.All(m => validMetrics.Contains(m.ToLowerInvariant()));
    }
}

public class ExportAnalyticsRequestValidator : AbstractValidator<ExportAnalyticsRequestDto>
{
    public ExportAnalyticsRequestValidator()
    {
        RuleFor(x => x.ReportType)
            .NotEmpty()
            .WithMessage("ReportType is required")
            .Must(BeValidReportType)
            .WithMessage("ReportType must be one of: dashboard, students, classes, financial, reservations, subscriptions");

        RuleFor(x => x.Format)
            .NotEmpty()
            .WithMessage("Format is required")
            .Must(BeValidFormat)
            .WithMessage("Format must be one of: pdf, excel, csv");

        RuleFor(x => x.Filter)
            .SetValidator(new AnalyticsFilterValidator())
            .When(x => x.Filter != null);
    }

    private static bool BeValidReportType(string reportType)
    {
        if (string.IsNullOrEmpty(reportType))
            return false;

        var validTypes = new[] { "dashboard", "students", "classes", "financial", "reservations", "subscriptions" };
        return validTypes.Contains(reportType.ToLowerInvariant());
    }

    private static bool BeValidFormat(string format)
    {
        if (string.IsNullOrEmpty(format))
            return false;

        var validFormats = new[] { "pdf", "excel", "csv" };
        return validFormats.Contains(format.ToLowerInvariant());
    }
}
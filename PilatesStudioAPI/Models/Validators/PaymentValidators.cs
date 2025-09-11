using FluentValidation;
using PilatesStudioAPI.Models.DTOs;

namespace PilatesStudioAPI.Models.Validators;

public class CreatePaymentValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("StudentId must be a positive number");

        RuleFor(x => x.PlanId)
            .GreaterThan(0)
            .WithMessage("PlanId must be a positive number");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0")
            .LessThanOrEqualTo(100000)
            .WithMessage("Amount cannot exceed 100,000");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .WithMessage("PaymentMethod is required")
            .Must(BeValidPaymentMethod)
            .WithMessage("PaymentMethod must be one of: cash, credit_card, debit_card, bank_transfer, digital_wallet");

        RuleFor(x => x.PaymentReference)
            .MaximumLength(255)
            .WithMessage("PaymentReference cannot exceed 255 characters");

        RuleFor(x => x.ReceiptNumber)
            .MaximumLength(100)
            .WithMessage("ReceiptNumber cannot exceed 100 characters");

        RuleFor(x => x.PaymentDate)
            .Must(BeValidPaymentDate)
            .When(x => x.PaymentDate.HasValue)
            .WithMessage("PaymentDate cannot be in the future");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters");

        // Conditional validation for payment reference based on payment method
        RuleFor(x => x.PaymentReference)
            .NotEmpty()
            .When(x => x.PaymentMethod == "credit_card" || x.PaymentMethod == "debit_card" || x.PaymentMethod == "bank_transfer")
            .WithMessage("PaymentReference is required for card and bank transfer payments");
    }

    private static bool BeValidPaymentMethod(string paymentMethod)
    {
        var validMethods = new[] { "cash", "credit_card", "debit_card", "bank_transfer", "digital_wallet" };
        return validMethods.Contains(paymentMethod.ToLower());
    }

    private static bool BeValidPaymentDate(DateTime? paymentDate)
    {
        return paymentDate <= DateTime.UtcNow;
    }
}

public class UpdatePaymentValidator : AbstractValidator<UpdatePaymentDto>
{
    public UpdatePaymentValidator()
    {
        RuleFor(x => x.PaymentReference)
            .MaximumLength(255)
            .WithMessage("PaymentReference cannot exceed 255 characters");

        RuleFor(x => x.ReceiptNumber)
            .MaximumLength(100)
            .WithMessage("ReceiptNumber cannot exceed 100 characters");

        RuleFor(x => x.PaymentDate)
            .Must(BeValidPaymentDate)
            .When(x => x.PaymentDate.HasValue)
            .WithMessage("PaymentDate cannot be in the future");

        RuleFor(x => x.Status)
            .Must(BeValidStatus)
            .When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status must be one of: pending, completed, failed, refunded");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters");
    }

    private static bool BeValidPaymentDate(DateTime? paymentDate)
    {
        return paymentDate <= DateTime.UtcNow;
    }

    private static bool BeValidStatus(string? status)
    {
        if (string.IsNullOrEmpty(status)) return true;
        var validStatuses = new[] { "pending", "completed", "failed", "refunded" };
        return validStatuses.Contains(status.ToLower());
    }
}

public class PaymentFilterValidator : AbstractValidator<PaymentFilterDto>
{
    public PaymentFilterValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .When(x => x.StudentId.HasValue)
            .WithMessage("StudentId must be a positive number");

        RuleFor(x => x.PlanId)
            .GreaterThan(0)
            .When(x => x.PlanId.HasValue)
            .WithMessage("PlanId must be a positive number");

        RuleFor(x => x.PaymentMethod)
            .Must(BeValidPaymentMethod)
            .When(x => !string.IsNullOrEmpty(x.PaymentMethod))
            .WithMessage("PaymentMethod must be one of: cash, credit_card, debit_card, bank_transfer, digital_wallet");

        RuleFor(x => x.Status)
            .Must(BeValidStatus)
            .When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status must be one of: pending, completed, failed, refunded");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("StartDate must be less than or equal to EndDate");

        RuleFor(x => x.MinAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinAmount.HasValue)
            .WithMessage("MinAmount must be greater than or equal to 0");

        RuleFor(x => x.MaxAmount)
            .GreaterThan(x => x.MinAmount)
            .When(x => x.MinAmount.HasValue && x.MaxAmount.HasValue)
            .WithMessage("MaxAmount must be greater than MinAmount");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100");
    }

    private static bool BeValidPaymentMethod(string? paymentMethod)
    {
        if (string.IsNullOrEmpty(paymentMethod)) return true;
        var validMethods = new[] { "cash", "credit_card", "debit_card", "bank_transfer", "digital_wallet" };
        return validMethods.Contains(paymentMethod.ToLower());
    }

    private static bool BeValidStatus(string? status)
    {
        if (string.IsNullOrEmpty(status)) return true;
        var validStatuses = new[] { "pending", "completed", "failed", "refunded" };
        return validStatuses.Contains(status.ToLower());
    }
}

public class RefundPaymentValidator : AbstractValidator<RefundPaymentDto>
{
    public RefundPaymentValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Refund reason is required")
            .MaximumLength(1000)
            .WithMessage("Reason cannot exceed 1000 characters");

        RuleFor(x => x.RefundAmount)
            .GreaterThan(0)
            .When(x => x.RefundAmount.HasValue)
            .WithMessage("RefundAmount must be greater than 0")
            .LessThanOrEqualTo(100000)
            .When(x => x.RefundAmount.HasValue)
            .WithMessage("RefundAmount cannot exceed 100,000");
    }
}
using System.Text.RegularExpressions;
using FluentValidation;
using WealthOS.Application.Income.DTOs.Requests;
using WealthOS.Application.Income.Queries;

namespace WealthOS.Application.Income.Validators;

internal static class PeriodRules
{
    private static readonly Regex PeriodRegex = new(@"^\d{4}-(0[1-9]|1[0-2])$", RegexOptions.Compiled);

    public static bool IsValidPeriod(string? period) =>
        !string.IsNullOrWhiteSpace(period) && PeriodRegex.IsMatch(period.Trim());
}

public sealed class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
{
    public CreateClientRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Engagement).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.MonthlyRevenue).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
        RuleFor(x => x.ContactEmail).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));
        RuleFor(x => x.ContactPhone).MaximumLength(32);
        RuleFor(x => x.Notes).MaximumLength(4000);
    }
}

public sealed class UpdateClientRequestValidator : AbstractValidator<UpdateClientRequest>
{
    public UpdateClientRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Engagement).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.MonthlyRevenue).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
        RuleFor(x => x.ContactEmail).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));
        RuleFor(x => x.ContactPhone).MaximumLength(32);
        RuleFor(x => x.Notes).MaximumLength(4000);
    }
}

public sealed class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.MonthlyRevenue).GreaterThanOrEqualTo(0).When(x => x.MonthlyRevenue.HasValue);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate.HasValue);
    }
}

public sealed class AssignDeveloperRequestValidator : AbstractValidator<AssignDeveloperRequest>
{
    public AssignDeveloperRequestValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.DeveloperId).NotEmpty();
        RuleFor(x => x.RoleOnProject).MaximumLength(200);
    }
}

public sealed class CreateInvoiceRequestValidator : AbstractValidator<CreateInvoiceRequest>
{
    public CreateInvoiceRequestValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.InvoiceNumber).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
        RuleFor(x => x.DueDate).GreaterThanOrEqualTo(x => x.IssueDate);
        RuleFor(x => x.Notes).MaximumLength(4000);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Description).NotEmpty().MaximumLength(500);
            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}

public sealed class RecordInvoicePaymentRequestValidator : AbstractValidator<RecordInvoicePaymentRequest>
{
    public RecordInvoicePaymentRequestValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).IsInEnum();
        RuleFor(x => x.Reference).MaximumLength(128);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}

public sealed class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequest>
{
    public CreateExpenseRequestValidator()
    {
        RuleFor(x => x.Vendor).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
        RuleFor(x => x.Notes).MaximumLength(2000);
        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .MaximumLength(100)
            .When(x => !x.CategoryId.HasValue);
    }
}

public sealed class RecordSalaryRequestValidator : AbstractValidator<RecordSalaryRequest>
{
    public RecordSalaryRequestValidator()
    {
        RuleFor(x => x.MemberName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Employer).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Role).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MonthlyAmount).GreaterThan(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
        RuleFor(x => x.Period)
            .Must(PeriodRules.IsValidPeriod)
            .WithMessage("Period must be in yyyy-MM format.");
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}

public sealed class CreateDeveloperRequestValidator : AbstractValidator<CreateDeveloperRequest>
{
    public CreateDeveloperRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Role).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MonthlySalary).GreaterThan(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}

public sealed class CreatePayrollRequestValidator : AbstractValidator<CreatePayrollRequest>
{
    public CreatePayrollRequestValidator()
    {
        RuleFor(x => x.DeveloperId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Period)
            .Must(PeriodRules.IsValidPeriod)
            .WithMessage("Period must be in yyyy-MM format.");
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}

public sealed class GetIncomeDashboardQueryValidator : AbstractValidator<GetIncomeDashboardQuery>
{
    public GetIncomeDashboardQueryValidator()
    {
        RuleFor(x => x.Period)
            .Must(period => string.IsNullOrWhiteSpace(period) || PeriodRules.IsValidPeriod(period))
            .WithMessage("Period must be in yyyy-MM format.");
    }
}

public sealed class GetCashFlowQueryValidator : AbstractValidator<GetCashFlowQuery>
{
    public GetCashFlowQueryValidator()
    {
        RuleFor(x => x.Period)
            .Must(period => string.IsNullOrWhiteSpace(period) || PeriodRules.IsValidPeriod(period))
            .WithMessage("Period must be in yyyy-MM format.");
    }
}

public sealed class GetProfitLossQueryValidator : AbstractValidator<GetProfitLossQuery>
{
    public GetProfitLossQueryValidator()
    {
        RuleFor(x => x.Period)
            .Must(period => string.IsNullOrWhiteSpace(period) || PeriodRules.IsValidPeriod(period))
            .WithMessage("Period must be in yyyy-MM format.");
    }
}

public sealed class GetMonthlyIncomeQueryValidator : AbstractValidator<GetMonthlyIncomeQuery>
{
    public GetMonthlyIncomeQueryValidator()
    {
        RuleFor(x => x.Months).InclusiveBetween(1, 36);
    }
}

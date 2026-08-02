using FluentValidation;
using WealthOS.Application.Reports.Commands;
using WealthOS.Application.Reports.DTOs.Requests;
using WealthOS.Application.Reports.Queries;

namespace WealthOS.Application.Reports.Validators;

public sealed class ReportFilterRequestValidator : AbstractValidator<ReportFilterRequest>
{
    public ReportFilterRequestValidator()
    {
        RuleFor(request => request.Category).MaximumLength(100);
        RuleFor(request => request.Owner).MaximumLength(200);
        RuleFor(request => request.PeriodLabel).MaximumLength(32);
        RuleFor(request => request.Period)
            .IsInEnum()
            .When(request => request.Period.HasValue);

        RuleFor(request => request)
            .Must(request => !request.FromDate.HasValue || !request.ToDate.HasValue || request.FromDate <= request.ToDate)
            .WithMessage("FromDate must be on or before ToDate.")
            .WithName("FromDate");

        RuleFor(request => request.PropertyId)
            .NotEmpty()
            .When(request => request.PropertyId.HasValue);
        RuleFor(request => request.InvestmentAccountId)
            .NotEmpty()
            .When(request => request.InvestmentAccountId.HasValue);
        RuleFor(request => request.BusinessClientId)
            .NotEmpty()
            .When(request => request.BusinessClientId.HasValue);
        RuleFor(request => request.GoalId)
            .NotEmpty()
            .When(request => request.GoalId.HasValue);
        RuleFor(request => request.LoanId)
            .NotEmpty()
            .When(request => request.LoanId.HasValue);
    }
}

public sealed class GenerateSnapshotRequestValidator : AbstractValidator<GenerateSnapshotRequest>
{
    public GenerateSnapshotRequestValidator()
    {
        RuleFor(request => request.ReportType).IsInEnum();
        RuleFor(request => request.Title).MaximumLength(200);
        RuleFor(request => request.Filters!)
            .SetValidator(new ReportFilterRequestValidator())
            .When(request => request.Filters is not null);
    }
}

public sealed class ExportReportRequestValidator : AbstractValidator<ExportReportRequest>
{
    public ExportReportRequestValidator()
    {
        RuleFor(request => request.ReportType).IsInEnum();
        RuleFor(request => request.Format).IsInEnum();
        RuleFor(request => request.SnapshotId)
            .NotEmpty()
            .When(request => request.SnapshotId.HasValue);
        RuleFor(request => request.Filters!)
            .SetValidator(new ReportFilterRequestValidator())
            .When(request => request.Filters is not null);
    }
}

public sealed class GetNetWorthReportQueryValidator : AbstractValidator<GetNetWorthReportQuery>
{
    public GetNetWorthReportQueryValidator()
    {
        RuleFor(query => query.Filters!)
            .SetValidator(new ReportFilterRequestValidator())
            .When(query => query.Filters is not null);
    }
}

public sealed class GetCashFlowReportQueryValidator : AbstractValidator<GetCashFlowReportQuery>
{
    public GetCashFlowReportQueryValidator()
    {
        RuleFor(query => query.Filters!)
            .SetValidator(new ReportFilterRequestValidator())
            .When(query => query.Filters is not null);
    }
}

public sealed class GetInvestmentReportQueryValidator : AbstractValidator<GetInvestmentReportQuery>
{
    public GetInvestmentReportQueryValidator()
    {
        RuleFor(query => query.Filters!)
            .SetValidator(new ReportFilterRequestValidator())
            .When(query => query.Filters is not null);
    }
}

public sealed class GetLoanReportQueryValidator : AbstractValidator<GetLoanReportQuery>
{
    public GetLoanReportQueryValidator()
    {
        RuleFor(query => query.Filters!)
            .SetValidator(new ReportFilterRequestValidator())
            .When(query => query.Filters is not null);
    }
}

public sealed class GetBusinessReportQueryValidator : AbstractValidator<GetBusinessReportQuery>
{
    public GetBusinessReportQueryValidator()
    {
        RuleFor(query => query.Filters!)
            .SetValidator(new ReportFilterRequestValidator())
            .When(query => query.Filters is not null);
    }
}

public sealed class GetGoalReportQueryValidator : AbstractValidator<GetGoalReportQuery>
{
    public GetGoalReportQueryValidator()
    {
        RuleFor(query => query.Filters!)
            .SetValidator(new ReportFilterRequestValidator())
            .When(query => query.Filters is not null);
    }
}

public sealed class GetPropertyReportQueryValidator : AbstractValidator<GetPropertyReportQuery>
{
    public GetPropertyReportQueryValidator()
    {
        RuleFor(query => query.Filters!)
            .SetValidator(new ReportFilterRequestValidator())
            .When(query => query.Filters is not null);
    }
}

public sealed class GetDocumentReportQueryValidator : AbstractValidator<GetDocumentReportQuery>
{
    public GetDocumentReportQueryValidator()
    {
        RuleFor(query => query.Filters!)
            .SetValidator(new ReportFilterRequestValidator())
            .When(query => query.Filters is not null);
    }
}

public sealed class GetFinancialHealthQueryValidator : AbstractValidator<GetFinancialHealthQuery>
{
    public GetFinancialHealthQueryValidator()
    {
        RuleFor(query => query.Filters!)
            .SetValidator(new ReportFilterRequestValidator())
            .When(query => query.Filters is not null);
    }
}

public sealed class GetAnalyticsSummaryQueryValidator : AbstractValidator<GetAnalyticsSummaryQuery>
{
    public GetAnalyticsSummaryQueryValidator()
    {
        RuleFor(query => query.Filters!)
            .SetValidator(new ReportFilterRequestValidator())
            .When(query => query.Filters is not null);
    }
}

public sealed class GenerateSnapshotCommandValidator : AbstractValidator<GenerateSnapshotCommand>
{
    public GenerateSnapshotCommandValidator()
    {
        RuleFor(command => command.Request).NotNull().SetValidator(new GenerateSnapshotRequestValidator());
    }
}

public sealed class ExportReportCommandValidator : AbstractValidator<ExportReportCommand>
{
    public ExportReportCommandValidator()
    {
        RuleFor(command => command.Request).NotNull().SetValidator(new ExportReportRequestValidator());
    }
}

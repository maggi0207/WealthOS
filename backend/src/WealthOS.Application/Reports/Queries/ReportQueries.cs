using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Reports.DTOs.Requests;

namespace WealthOS.Application.Reports.Queries;

public sealed class GetNetWorthReportQuery : IQuery
{
    public ReportFilterRequest? Filters { get; init; }
}

public sealed class GetCashFlowReportQuery : IQuery
{
    public ReportFilterRequest? Filters { get; init; }
}

public sealed class GetInvestmentReportQuery : IQuery
{
    public ReportFilterRequest? Filters { get; init; }
}

public sealed class GetLoanReportQuery : IQuery
{
    public ReportFilterRequest? Filters { get; init; }
}

public sealed class GetBusinessReportQuery : IQuery
{
    public ReportFilterRequest? Filters { get; init; }
}

public sealed class GetGoalReportQuery : IQuery
{
    public ReportFilterRequest? Filters { get; init; }
}

public sealed class GetPropertyReportQuery : IQuery
{
    public ReportFilterRequest? Filters { get; init; }
}

public sealed class GetDocumentReportQuery : IQuery
{
    public ReportFilterRequest? Filters { get; init; }
}

public sealed class GetFinancialHealthQuery : IQuery
{
    public ReportFilterRequest? Filters { get; init; }
}

public sealed class GetAnalyticsSummaryQuery : IQuery
{
    public ReportFilterRequest? Filters { get; init; }
}

using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Reports.DTOs.Responses;
using WealthOS.Application.Reports.Interfaces;
using WealthOS.Application.Reports.Queries;

namespace WealthOS.Application.Reports.Queries.Handlers;

public sealed class GetNetWorthReportQueryHandler
    : IQueryHandler<GetNetWorthReportQuery, NetWorthReportResponse>
{
    private readonly IReportService _reportService;

    public GetNetWorthReportQueryHandler(IReportService reportService) =>
        _reportService = reportService;

    public Task<Result<NetWorthReportResponse>> HandleAsync(
        GetNetWorthReportQuery query,
        CancellationToken cancellationToken = default) =>
        _reportService.GetNetWorthReportAsync(query.Filters, cancellationToken);
}

public sealed class GetCashFlowReportQueryHandler
    : IQueryHandler<GetCashFlowReportQuery, CashFlowReportResponse>
{
    private readonly IReportService _reportService;

    public GetCashFlowReportQueryHandler(IReportService reportService) =>
        _reportService = reportService;

    public Task<Result<CashFlowReportResponse>> HandleAsync(
        GetCashFlowReportQuery query,
        CancellationToken cancellationToken = default) =>
        _reportService.GetCashFlowReportAsync(query.Filters, cancellationToken);
}

public sealed class GetInvestmentReportQueryHandler
    : IQueryHandler<GetInvestmentReportQuery, InvestmentReportResponse>
{
    private readonly IReportService _reportService;

    public GetInvestmentReportQueryHandler(IReportService reportService) =>
        _reportService = reportService;

    public Task<Result<InvestmentReportResponse>> HandleAsync(
        GetInvestmentReportQuery query,
        CancellationToken cancellationToken = default) =>
        _reportService.GetInvestmentReportAsync(query.Filters, cancellationToken);
}

public sealed class GetLoanReportQueryHandler
    : IQueryHandler<GetLoanReportQuery, LoanReportResponse>
{
    private readonly IReportService _reportService;

    public GetLoanReportQueryHandler(IReportService reportService) =>
        _reportService = reportService;

    public Task<Result<LoanReportResponse>> HandleAsync(
        GetLoanReportQuery query,
        CancellationToken cancellationToken = default) =>
        _reportService.GetLoanReportAsync(query.Filters, cancellationToken);
}

public sealed class GetBusinessReportQueryHandler
    : IQueryHandler<GetBusinessReportQuery, BusinessReportResponse>
{
    private readonly IReportService _reportService;

    public GetBusinessReportQueryHandler(IReportService reportService) =>
        _reportService = reportService;

    public Task<Result<BusinessReportResponse>> HandleAsync(
        GetBusinessReportQuery query,
        CancellationToken cancellationToken = default) =>
        _reportService.GetBusinessReportAsync(query.Filters, cancellationToken);
}

public sealed class GetGoalReportQueryHandler
    : IQueryHandler<GetGoalReportQuery, GoalReportResponse>
{
    private readonly IReportService _reportService;

    public GetGoalReportQueryHandler(IReportService reportService) =>
        _reportService = reportService;

    public Task<Result<GoalReportResponse>> HandleAsync(
        GetGoalReportQuery query,
        CancellationToken cancellationToken = default) =>
        _reportService.GetGoalReportAsync(query.Filters, cancellationToken);
}

public sealed class GetPropertyReportQueryHandler
    : IQueryHandler<GetPropertyReportQuery, PropertyReportResponse>
{
    private readonly IReportService _reportService;

    public GetPropertyReportQueryHandler(IReportService reportService) =>
        _reportService = reportService;

    public Task<Result<PropertyReportResponse>> HandleAsync(
        GetPropertyReportQuery query,
        CancellationToken cancellationToken = default) =>
        _reportService.GetPropertyReportAsync(query.Filters, cancellationToken);
}

public sealed class GetDocumentReportQueryHandler
    : IQueryHandler<GetDocumentReportQuery, DocumentReportResponse>
{
    private readonly IReportService _reportService;

    public GetDocumentReportQueryHandler(IReportService reportService) =>
        _reportService = reportService;

    public Task<Result<DocumentReportResponse>> HandleAsync(
        GetDocumentReportQuery query,
        CancellationToken cancellationToken = default) =>
        _reportService.GetDocumentReportAsync(query.Filters, cancellationToken);
}

public sealed class GetFinancialHealthQueryHandler
    : IQueryHandler<GetFinancialHealthQuery, FinancialHealthResponse>
{
    private readonly IFinancialHealthService _financialHealthService;

    public GetFinancialHealthQueryHandler(IFinancialHealthService financialHealthService) =>
        _financialHealthService = financialHealthService;

    public Task<Result<FinancialHealthResponse>> HandleAsync(
        GetFinancialHealthQuery query,
        CancellationToken cancellationToken = default) =>
        _financialHealthService.GetFinancialHealthAsync(query.Filters, cancellationToken);
}

public sealed class GetAnalyticsSummaryQueryHandler
    : IQueryHandler<GetAnalyticsSummaryQuery, AnalyticsSummaryResponse>
{
    private readonly IAnalyticsService _analyticsService;

    public GetAnalyticsSummaryQueryHandler(IAnalyticsService analyticsService) =>
        _analyticsService = analyticsService;

    public Task<Result<AnalyticsSummaryResponse>> HandleAsync(
        GetAnalyticsSummaryQuery query,
        CancellationToken cancellationToken = default) =>
        _analyticsService.GetSummaryAsync(query.Filters, cancellationToken);
}

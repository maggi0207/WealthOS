using WealthOS.Application.Common.Models;
using WealthOS.Application.Reports.DTOs.Requests;
using WealthOS.Application.Reports.DTOs.Responses;
using WealthOS.Domain.Reports.Enums;

namespace WealthOS.Application.Reports.Interfaces;

/// <summary>
/// Aggregates module data into typed reports through application interfaces only.
/// </summary>
public interface IReportService
{
    Task<Result<NetWorthReportResponse>> GetNetWorthReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default);

    Task<Result<CashFlowReportResponse>> GetCashFlowReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default);

    Task<Result<InvestmentReportResponse>> GetInvestmentReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default);

    Task<Result<LoanReportResponse>> GetLoanReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default);

    Task<Result<PropertyReportResponse>> GetPropertyReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default);

    Task<Result<BusinessReportResponse>> GetBusinessReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default);

    Task<Result<GoalReportResponse>> GetGoalReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default);

    Task<Result<DocumentReportResponse>> GetDocumentReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default);

    Task<Result<ReportSnapshotResponse>> GenerateSnapshotAsync(
        GenerateSnapshotRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Cross-module analytics KPIs and trends.
/// </summary>
public interface IAnalyticsService
{
    Task<Result<AnalyticsSummaryResponse>> GetSummaryAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Composite financial health scoring from module summaries.
/// </summary>
public interface IFinancialHealthService
{
    Task<Result<FinancialHealthResponse>> GetFinancialHealthAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Export architecture stub — returns NotImplemented metadata only.
/// </summary>
public interface IExportService
{
    Task<Result<ReportExportResponse>> ExportAsync(
        ExportReportRequest request,
        CancellationToken cancellationToken = default);

    IReadOnlyList<ReportExportFormat> GetSupportedFormats();
}

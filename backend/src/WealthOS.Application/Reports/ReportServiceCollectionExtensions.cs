using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Reports.Commands;
using WealthOS.Application.Reports.Commands.Handlers;
using WealthOS.Application.Reports.DTOs.Responses;
using WealthOS.Application.Reports.Interfaces;
using WealthOS.Application.Reports.Queries;
using WealthOS.Application.Reports.Queries.Handlers;
using WealthOS.Application.Reports.Services;

namespace WealthOS.Application.Reports;

/// <summary>
/// Registers Reports &amp; Analytics application services and CQRS handlers.
/// </summary>
public static class ReportServiceCollectionExtensions
{
    public static IServiceCollection AddReportsApplication(this IServiceCollection services)
    {
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<IFinancialHealthService, FinancialHealthService>();
        services.AddScoped<IExportService, ExportService>();

        services.AddScoped<
            IQueryHandler<GetNetWorthReportQuery, NetWorthReportResponse>,
            GetNetWorthReportQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetCashFlowReportQuery, CashFlowReportResponse>,
            GetCashFlowReportQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetInvestmentReportQuery, InvestmentReportResponse>,
            GetInvestmentReportQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetLoanReportQuery, LoanReportResponse>,
            GetLoanReportQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetBusinessReportQuery, BusinessReportResponse>,
            GetBusinessReportQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetGoalReportQuery, GoalReportResponse>,
            GetGoalReportQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetPropertyReportQuery, PropertyReportResponse>,
            GetPropertyReportQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetDocumentReportQuery, DocumentReportResponse>,
            GetDocumentReportQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetFinancialHealthQuery, FinancialHealthResponse>,
            GetFinancialHealthQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetAnalyticsSummaryQuery, AnalyticsSummaryResponse>,
            GetAnalyticsSummaryQueryHandler>();

        services.AddScoped<
            ICommandHandler<GenerateSnapshotCommand, ReportSnapshotResponse>,
            GenerateSnapshotCommandHandler>();
        services.AddScoped<
            ICommandHandler<ExportReportCommand, ReportExportResponse>,
            ExportReportCommandHandler>();

        return services;
    }
}

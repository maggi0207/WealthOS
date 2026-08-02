using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WealthOS.Domain.Reports.Entities;
using WealthOS.Domain.Reports.Enums;
using WealthOS.Domain.Reports.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Reports.Repositories;

namespace WealthOS.Infrastructure.Reports;

/// <summary>
/// Registers Reports &amp; Analytics infrastructure: repositories and seed hooks.
/// </summary>
public static class ReportsInfrastructureExtensions
{
    public static IServiceCollection AddReportsInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IReportDefinitionRepository, ReportDefinitionRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IReportExecutionRepository, ReportExecutionRepository>();
        services.AddScoped<IReportSnapshotRepository, ReportSnapshotRepository>();
        services.AddScoped<IReportExportRepository, ReportExportRepository>();

        return services;
    }
}

/// <summary>
/// Seeds system report definitions (catalog placeholders).
/// </summary>
public static class ReportsDataSeeder
{
    public static readonly Guid NetWorthDefinitionId =
        Guid.Parse("c1200001-0000-4000-8000-000000000001");

    public static readonly Guid CashFlowDefinitionId =
        Guid.Parse("c1200001-0000-4000-8000-000000000002");

    public static readonly Guid InvestmentDefinitionId =
        Guid.Parse("c1200001-0000-4000-8000-000000000003");

    public static readonly Guid LoanDefinitionId =
        Guid.Parse("c1200001-0000-4000-8000-000000000004");

    public static readonly Guid PropertyDefinitionId =
        Guid.Parse("c1200001-0000-4000-8000-000000000005");

    public static readonly Guid BusinessDefinitionId =
        Guid.Parse("c1200001-0000-4000-8000-000000000006");

    public static readonly Guid GoalDefinitionId =
        Guid.Parse("c1200001-0000-4000-8000-000000000007");

    public static readonly Guid DocumentDefinitionId =
        Guid.Parse("c1200001-0000-4000-8000-000000000008");

    public static readonly Guid FinancialHealthDefinitionId =
        Guid.Parse("c1200001-0000-4000-8000-000000000009");

    public static readonly Guid AnalyticsSummaryDefinitionId =
        Guid.Parse("c1200001-0000-4000-8000-00000000000a");

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(ReportsDataSeeder));

        if (!dbContext.ReportDefinitions.Any())
        {
            dbContext.ReportDefinitions.AddRange(
                CreateDefinition(NetWorthDefinitionId, "net-worth", "Net Worth", ReportType.NetWorth, 1),
                CreateDefinition(CashFlowDefinitionId, "cash-flow", "Cash Flow", ReportType.CashFlow, 2),
                CreateDefinition(InvestmentDefinitionId, "investments", "Investment Performance", ReportType.InvestmentPerformance, 3),
                CreateDefinition(LoanDefinitionId, "loans", "Loan Analysis", ReportType.LoanAnalysis, 4),
                CreateDefinition(PropertyDefinitionId, "properties", "Property Appreciation", ReportType.PropertyAppreciation, 5),
                CreateDefinition(BusinessDefinitionId, "business", "Business P&L", ReportType.BusinessProfitAndLoss, 6),
                CreateDefinition(GoalDefinitionId, "goals", "Goal Progress", ReportType.GoalProgress, 7),
                CreateDefinition(DocumentDefinitionId, "documents", "Document Summary", ReportType.DocumentSummary, 8),
                CreateDefinition(FinancialHealthDefinitionId, "financial-health", "Financial Health Score", ReportType.FinancialHealthScore, 9),
                CreateDefinition(AnalyticsSummaryDefinitionId, "summary", "Analytics Summary", ReportType.AnalyticsSummary, 10));

            logger.LogInformation("Seeded {Count} report definitions", 10);
            await dbContext.SaveChangesAsync();
        }
    }

    private static ReportDefinition CreateDefinition(
        Guid id,
        string code,
        string name,
        ReportType reportType,
        int sortOrder) =>
        new(id)
        {
            Code = code,
            Name = name,
            Description = $"{name} report aggregated via Application interfaces.",
            ReportType = reportType,
            Status = ReportDefinitionStatus.Active,
            SortOrder = sortOrder,
            IsSystem = true,
        };
}

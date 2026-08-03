using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Dashboard.Providers;
using WealthOS.Infrastructure.Assets.Providers;
using WealthOS.Infrastructure.Documents.Providers;
using WealthOS.Infrastructure.Income.Providers;
using WealthOS.Infrastructure.Investments.Providers;
using WealthOS.Infrastructure.Loans.Providers;
using WealthOS.Infrastructure.Properties.Providers;

namespace WealthOS.Infrastructure.Dashboard;

/// <summary>
/// Registers Dashboard infrastructure adapters that read live module totals.
/// </summary>
public static class DashboardInfrastructureExtensions
{
    public static IServiceCollection AddDashboardInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IPropertySummaryProvider, PropertySummaryProvider>();
        services.AddScoped<ILoanSummaryProvider, LoanSummaryProvider>();
        services.AddScoped<IInvestmentSummaryProvider, InvestmentSummaryProvider>();
        services.AddScoped<IIncomeSummaryProvider, IncomeSummaryProvider>();
        services.AddScoped<IDocumentSummaryProvider, DocumentSummaryProvider>();
        services.AddScoped<IManualAssetSummaryProvider, ManualAssetSummaryProvider>();

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Dashboard.Providers;
using WealthOS.Infrastructure.Dashboard.Providers;

namespace WealthOS.Infrastructure.Dashboard;

/// <summary>
/// Registers Dashboard infrastructure adapters (mock module providers for Phase 3).
/// </summary>
public static class DashboardInfrastructureExtensions
{
    public static IServiceCollection AddDashboardInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IPropertySummaryProvider, MockPropertySummaryProvider>();
        services.AddScoped<ILoanSummaryProvider, MockLoanSummaryProvider>();
        services.AddScoped<IInvestmentSummaryProvider, MockInvestmentSummaryProvider>();
        services.AddScoped<IIncomeSummaryProvider, MockIncomeSummaryProvider>();
        services.AddScoped<IDocumentSummaryProvider, MockDocumentSummaryProvider>();

        return services;
    }
}

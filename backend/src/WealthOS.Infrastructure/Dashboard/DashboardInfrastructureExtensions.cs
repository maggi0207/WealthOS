using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Dashboard.Providers;
using WealthOS.Infrastructure.Dashboard.Providers;
using WealthOS.Infrastructure.Loans.Providers;

namespace WealthOS.Infrastructure.Dashboard;

/// <summary>
/// Registers Dashboard infrastructure adapters (module providers; mocks until modules land).
/// </summary>
public static class DashboardInfrastructureExtensions
{
    public static IServiceCollection AddDashboardInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IPropertySummaryProvider, MockPropertySummaryProvider>();
        services.AddScoped<ILoanSummaryProvider, LoanSummaryProvider>();
        services.AddScoped<IInvestmentSummaryProvider, MockInvestmentSummaryProvider>();
        services.AddScoped<IIncomeSummaryProvider, MockIncomeSummaryProvider>();
        services.AddScoped<IDocumentSummaryProvider, MockDocumentSummaryProvider>();

        return services;
    }
}

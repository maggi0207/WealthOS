using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Dashboard.DTOs.Responses;
using WealthOS.Application.Dashboard.Interfaces;
using WealthOS.Application.Dashboard.Queries;
using WealthOS.Application.Dashboard.Queries.Handlers;
using WealthOS.Application.Dashboard.Services;

namespace WealthOS.Application.Dashboard;

/// <summary>
/// Registers Dashboard application services and CQRS query handlers.
/// </summary>
public static class DashboardServiceCollectionExtensions
{
    public static IServiceCollection AddDashboardApplication(this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardService>();

        services.AddScoped<
            IQueryHandler<GetDashboardSummaryQuery, DashboardResponse>,
            GetDashboardSummaryQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetNetWorthQuery, NetWorthResponse>,
            GetNetWorthQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetRecentActivitiesQuery, IReadOnlyList<RecentActivityResponse>>,
            GetRecentActivitiesQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetDashboardHealthQuery, DashboardHealthResponse>,
            GetDashboardHealthQueryHandler>();

        return services;
    }
}

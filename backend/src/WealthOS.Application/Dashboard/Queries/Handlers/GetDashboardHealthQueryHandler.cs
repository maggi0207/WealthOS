using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.DTOs.Responses;
using WealthOS.Application.Dashboard.Interfaces;
using WealthOS.Application.Dashboard.Queries;

namespace WealthOS.Application.Dashboard.Queries.Handlers;

/// <summary>
/// Handles <see cref="GetDashboardHealthQuery"/>.
/// </summary>
public sealed class GetDashboardHealthQueryHandler
    : IQueryHandler<GetDashboardHealthQuery, DashboardHealthResponse>
{
    private readonly IDashboardService _dashboardService;

    public GetDashboardHealthQueryHandler(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public Task<Result<DashboardHealthResponse>> HandleAsync(
        GetDashboardHealthQuery query,
        CancellationToken cancellationToken = default) =>
        _dashboardService.GetHealthAsync(cancellationToken);
}

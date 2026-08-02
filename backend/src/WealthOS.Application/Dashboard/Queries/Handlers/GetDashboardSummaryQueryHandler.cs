using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.DTOs.Responses;
using WealthOS.Application.Dashboard.Interfaces;
using WealthOS.Application.Dashboard.Queries;

namespace WealthOS.Application.Dashboard.Queries.Handlers;

/// <summary>
/// Handles <see cref="GetDashboardSummaryQuery"/>.
/// </summary>
public sealed class GetDashboardSummaryQueryHandler
    : IQueryHandler<GetDashboardSummaryQuery, DashboardResponse>
{
    private readonly IDashboardService _dashboardService;

    public GetDashboardSummaryQueryHandler(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public Task<Result<DashboardResponse>> HandleAsync(
        GetDashboardSummaryQuery query,
        CancellationToken cancellationToken = default) =>
        _dashboardService.GetSummaryAsync(cancellationToken);
}

using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.DTOs.Responses;
using WealthOS.Application.Dashboard.Interfaces;
using WealthOS.Application.Dashboard.Queries;

namespace WealthOS.Application.Dashboard.Queries.Handlers;

/// <summary>
/// Handles <see cref="GetNetWorthQuery"/>.
/// </summary>
public sealed class GetNetWorthQueryHandler : IQueryHandler<GetNetWorthQuery, NetWorthResponse>
{
    private readonly IDashboardService _dashboardService;

    public GetNetWorthQueryHandler(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public Task<Result<NetWorthResponse>> HandleAsync(
        GetNetWorthQuery query,
        CancellationToken cancellationToken = default) =>
        _dashboardService.GetNetWorthAsync(cancellationToken);
}

using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.DTOs.Responses;
using WealthOS.Domain.Dashboard.Models;

namespace WealthOS.Application.Dashboard.Interfaces;

/// <summary>
/// Orchestrates dashboard aggregation from module summary providers.
/// </summary>
public interface IDashboardService
{
    Task<Result<DashboardResponse>> GetSummaryAsync(CancellationToken cancellationToken = default);

    Task<Result<NetWorthResponse>> GetNetWorthAsync(CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<RecentActivityResponse>>> GetRecentActivitiesAsync(
        int limit,
        CancellationToken cancellationToken = default);

    Task<Result<DashboardHealthResponse>> GetHealthAsync(CancellationToken cancellationToken = default);

    Task<Result<DashboardSnapshot>> CreateSnapshotAsync(CancellationToken cancellationToken = default);
}

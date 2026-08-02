using WealthOS.Application.Common.Abstractions;

namespace WealthOS.Application.Dashboard.Queries;

/// <summary>
/// Query for recent dashboard activities.
/// </summary>
public sealed class GetRecentActivitiesQuery : IQuery
{
    public int Limit { get; init; } = 10;
}

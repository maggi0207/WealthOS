using WealthOS.Application.Common.Abstractions;
using WealthOS.Domain.Properties.Enums;

namespace WealthOS.Application.Properties.Queries;

/// <summary>
/// Loads a single property by id for the authenticated user.
/// </summary>
public sealed class GetPropertyByIdQuery : IQuery
{
    public Guid PropertyId { get; init; }
}

/// <summary>
/// Lists properties for the authenticated user with optional filters and paging.
/// </summary>
public sealed class GetAllPropertiesQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? Search { get; init; }

    public PropertyStatus? Status { get; init; }

    public PropertyType? Type { get; init; }
}

/// <summary>
/// Portfolio-level property totals for the authenticated user.
/// </summary>
public sealed class GetPropertySummaryQuery : IQuery;

/// <summary>
/// Per-property dashboard snapshot for the authenticated user.
/// </summary>
public sealed class GetPropertyDashboardQuery : IQuery
{
    public Guid PropertyId { get; init; }
}

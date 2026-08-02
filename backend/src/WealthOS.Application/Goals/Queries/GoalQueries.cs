using WealthOS.Application.Common.Abstractions;
using WealthOS.Domain.Goals.Enums;

namespace WealthOS.Application.Goals.Queries;

/// <summary>
/// Lists goals for the authenticated user.
/// </summary>
public sealed class GetGoalsQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? Search { get; init; }

    public GoalStatus? Status { get; init; }

    public GoalCategory? Category { get; init; }

    public GoalPriority? Priority { get; init; }
}

/// <summary>
/// Gets a single goal by identifier.
/// </summary>
public sealed class GetGoalByIdQuery : IQuery
{
    public Guid GoalId { get; init; }
}

/// <summary>
/// Gets computed progress for a goal.
/// </summary>
public sealed class GetGoalProgressQuery : IQuery
{
    public Guid GoalId { get; init; }
}

/// <summary>
/// Gets goals module dashboard summary.
/// </summary>
public sealed class GetGoalDashboardQuery : IQuery
{
}

/// <summary>
/// Gets a contribution projection for a goal.
/// </summary>
public sealed class GetGoalProjectionQuery : IQuery
{
    public Guid GoalId { get; init; }
}

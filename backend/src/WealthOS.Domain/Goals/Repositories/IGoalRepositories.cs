using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Goals.Entities;
using WealthOS.Domain.Goals.Enums;
using WealthOS.Domain.Goals.Models;

namespace WealthOS.Domain.Goals.Repositories;

/// <summary>
/// Persistence abstraction for the FinancialGoal aggregate.
/// </summary>
public interface IFinancialGoalRepository : IRepository<FinancialGoal>
{
    Task<FinancialGoal?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<FinancialGoal?> GetByIdWithDetailsAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<FinancialGoal> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        GoalStatus? status,
        GoalCategory? category,
        GoalPriority? priority,
        CancellationToken cancellationToken = default);

    Task<GoalDashboardSummary> GetDashboardSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FinancialGoal>> ListActiveForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Persistence abstraction for goal contributions (queries nested under goals).
/// </summary>
public interface IGoalContributionRepository : IRepository<GoalContribution>
{
    Task<(IReadOnlyList<GoalContribution> Items, int TotalCount)> ListForGoalAsync(
        Guid goalId,
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Persistence abstraction for goal milestones.
/// </summary>
public interface IGoalMilestoneRepository : IRepository<GoalMilestone>
{
    Task<GoalMilestone?> GetByIdForUserAsync(
        Guid milestoneId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GoalMilestone>> ListUpcomingForUserAsync(
        Guid userId,
        int take,
        CancellationToken cancellationToken = default);
}

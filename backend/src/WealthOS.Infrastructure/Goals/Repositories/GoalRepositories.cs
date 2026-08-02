using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Goals.Entities;
using WealthOS.Domain.Goals.Enums;
using WealthOS.Domain.Goals.Models;
using WealthOS.Domain.Goals.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Repositories;

namespace WealthOS.Infrastructure.Goals.Repositories;

/// <summary>
/// EF Core repository for the FinancialGoal aggregate.
/// </summary>
public sealed class FinancialGoalRepository : Repository<FinancialGoal>, IFinancialGoalRepository
{
    public FinancialGoalRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<FinancialGoal?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            goal => goal.Id == id && goal.UserId == userId,
            cancellationToken);

    public async Task<FinancialGoal?> GetByIdWithDetailsAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsSplitQuery()
            .Include(goal => goal.Contributions)
            .Include(goal => goal.Milestones)
            .FirstOrDefaultAsync(
                goal => goal.Id == id && goal.UserId == userId,
                cancellationToken);

    public async Task<(IReadOnlyList<FinancialGoal> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        GoalStatus? status,
        GoalCategory? category,
        GoalPriority? priority,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(goal => goal.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(goal =>
                goal.Name.ToLower().Contains(term)
                || (goal.Description != null && goal.Description.ToLower().Contains(term)));
        }

        if (status.HasValue)
        {
            query = query.Where(goal => goal.Status == status.Value);
        }

        if (category.HasValue)
        {
            query = query.Where(goal => goal.Category == category.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(goal => goal.Priority == priority.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(goal => goal.Priority)
            .ThenBy(goal => goal.TargetDate)
            .ThenBy(goal => goal.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<GoalDashboardSummary> GetDashboardSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var goals = await DbSet
            .AsNoTracking()
            .Where(goal => goal.UserId == userId)
            .Select(goal => new
            {
                goal.Id,
                goal.Name,
                goal.Status,
                goal.TargetAmount,
                goal.CurrentAmount,
                goal.MonthlyContribution,
                goal.CurrencyCode,
            })
            .ToListAsync(cancellationToken);

        var totalGoalValue = goals.Sum(goal => goal.TargetAmount);
        var totalSaved = goals.Sum(goal => goal.CurrentAmount);
        var overall = totalGoalValue <= 0m
            ? 0m
            : Math.Round(Math.Min(100m, totalSaved / totalGoalValue * 100m), 2, MidpointRounding.AwayFromZero);

        var upcoming = await Context.Set<GoalMilestone>()
            .AsNoTracking()
            .Where(milestone =>
                !milestone.IsCompleted
                && milestone.Goal.UserId == userId
                && !milestone.Goal.IsDeleted
                && milestone.Goal.Status == GoalStatus.Active)
            .OrderBy(milestone => milestone.TargetPercent)
            .ThenBy(milestone => milestone.SortOrder)
            .Take(8)
            .Select(milestone => new GoalUpcomingMilestone
            {
                MilestoneId = milestone.Id,
                GoalId = milestone.GoalId,
                GoalName = milestone.Goal.Name,
                Label = milestone.Label,
                TargetPercent = milestone.TargetPercent,
                GoalCompletionPercent = milestone.Goal.TargetAmount <= 0m
                    ? 0m
                    : Math.Round(
                        Math.Min(100m, milestone.Goal.CurrentAmount / milestone.Goal.TargetAmount * 100m),
                        2,
                        MidpointRounding.AwayFromZero),
            })
            .ToListAsync(cancellationToken);

        return new GoalDashboardSummary
        {
            ActiveGoals = goals.Count(goal => goal.Status == GoalStatus.Active),
            CompletedGoals = goals.Count(goal => goal.Status == GoalStatus.Completed),
            PausedGoals = goals.Count(goal => goal.Status == GoalStatus.Paused),
            TotalGoalValue = Math.Round(totalGoalValue, 2, MidpointRounding.AwayFromZero),
            TotalSaved = Math.Round(totalSaved, 2, MidpointRounding.AwayFromZero),
            OverallProgressPercent = overall,
            MonthlyCommitted = Math.Round(
                goals.Where(goal => goal.Status == GoalStatus.Active).Sum(goal => goal.MonthlyContribution),
                2,
                MidpointRounding.AwayFromZero),
            UpcomingMilestones = upcoming,
            CurrencyCode = goals.FirstOrDefault()?.CurrencyCode ?? "INR",
        };
    }

    public async Task<IReadOnlyList<FinancialGoal>> ListActiveForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Where(goal => goal.UserId == userId && goal.Status == GoalStatus.Active)
            .OrderByDescending(goal => goal.Priority)
            .ThenBy(goal => goal.TargetDate)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.AnyAsync(goal => goal.Id == id && goal.UserId == userId, cancellationToken);
}

/// <summary>
/// EF Core repository for goal contributions.
/// </summary>
public sealed class GoalContributionRepository : Repository<GoalContribution>, IGoalContributionRepository
{
    public GoalContributionRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<(IReadOnlyList<GoalContribution> Items, int TotalCount)> ListForGoalAsync(
        Guid goalId,
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(contribution =>
                contribution.GoalId == goalId
                && contribution.Goal.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(contribution => contribution.ContributedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}

/// <summary>
/// EF Core repository for goal milestones.
/// </summary>
public sealed class GoalMilestoneRepository : Repository<GoalMilestone>, IGoalMilestoneRepository
{
    public GoalMilestoneRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<GoalMilestone?> GetByIdForUserAsync(
        Guid milestoneId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(milestone => milestone.Goal)
            .FirstOrDefaultAsync(
                milestone => milestone.Id == milestoneId && milestone.Goal.UserId == userId,
                cancellationToken);

    public async Task<IReadOnlyList<GoalMilestone>> ListUpcomingForUserAsync(
        Guid userId,
        int take,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Where(milestone =>
                !milestone.IsCompleted
                && milestone.Goal.UserId == userId
                && milestone.Goal.Status == GoalStatus.Active)
            .OrderBy(milestone => milestone.TargetPercent)
            .ThenBy(milestone => milestone.SortOrder)
            .Take(take)
            .ToListAsync(cancellationToken);
}

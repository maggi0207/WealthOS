using WealthOS.Domain.Goals.Enums;

namespace WealthOS.Application.Goals.DTOs.Requests;

/// <summary>
/// Creates a new financial goal.
/// </summary>
public sealed class CreateGoalRequest
{
    public string Name { get; set; } = string.Empty;

    public GoalCategory Category { get; set; } = GoalCategory.Custom;

    public decimal TargetAmount { get; set; }

    public decimal CurrentAmount { get; set; }

    public DateOnly TargetDate { get; set; }

    public DateOnly StartedOn { get; set; }

    public decimal MonthlyContribution { get; set; }

    public GoalPriority Priority { get; set; } = GoalPriority.Medium;

    public GoalStatus Status { get; set; } = GoalStatus.Active;

    public string? Description { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public Guid? LinkedPropertyId { get; set; }

    public Guid? LinkedInvestmentId { get; set; }

    public Guid? LinkedLoanId { get; set; }

    public Guid? LinkedIncomeSourceId { get; set; }

    public IReadOnlyList<CreateGoalMilestoneRequest>? Milestones { get; set; }
}

/// <summary>
/// Milestone definition supplied when creating or updating a goal.
/// </summary>
public sealed class CreateGoalMilestoneRequest
{
    public string Label { get; set; } = string.Empty;

    public decimal TargetPercent { get; set; }

    public decimal? TargetAmount { get; set; }

    public DateOnly? ReachedOn { get; set; }

    public int SortOrder { get; set; }
}

/// <summary>
/// Updates an existing financial goal.
/// </summary>
public sealed class UpdateGoalRequest
{
    public string Name { get; set; } = string.Empty;

    public GoalCategory Category { get; set; } = GoalCategory.Custom;

    public decimal TargetAmount { get; set; }

    public decimal CurrentAmount { get; set; }

    public DateOnly TargetDate { get; set; }

    public DateOnly StartedOn { get; set; }

    public decimal MonthlyContribution { get; set; }

    public GoalPriority Priority { get; set; } = GoalPriority.Medium;

    public GoalStatus Status { get; set; } = GoalStatus.Active;

    public string? Description { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public Guid? LinkedPropertyId { get; set; }

    public Guid? LinkedInvestmentId { get; set; }

    public Guid? LinkedLoanId { get; set; }

    public Guid? LinkedIncomeSourceId { get; set; }
}

/// <summary>
/// Records a contribution against a goal.
/// </summary>
public sealed class RecordGoalContributionRequest
{
    public decimal Amount { get; set; }

    public DateOnly ContributedOn { get; set; }

    public string? Notes { get; set; }

    public string? Source { get; set; }
}

/// <summary>
/// Marks a milestone as completed.
/// </summary>
public sealed class CompleteMilestoneRequest
{
    public DateOnly? ReachedOn { get; set; }
}

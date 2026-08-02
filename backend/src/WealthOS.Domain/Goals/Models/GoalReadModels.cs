using WealthOS.Domain.Goals.Enums;

namespace WealthOS.Domain.Goals.Models;

/// <summary>
/// Computed progress snapshot for a single goal (not persisted).
/// </summary>
public sealed class GoalProgress
{
    public Guid GoalId { get; init; }

    public string GoalName { get; init; } = string.Empty;

    public decimal TargetAmount { get; init; }

    public decimal CurrentAmount { get; init; }

    public decimal RemainingAmount { get; init; }

    public decimal CompletionPercent { get; init; }

    public decimal MonthlyContribution { get; init; }

    public decimal MonthlyRequiredContribution { get; init; }

    public DateOnly? EstimatedCompletionDate { get; init; }

    public DateOnly TargetDate { get; init; }

    public ProgressTrend Trend { get; init; }

    public int MonthsRemaining { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

/// <summary>
/// Forward-looking projection for a goal at the planned monthly contribution (not persisted).
/// </summary>
public sealed class GoalProjection
{
    public Guid GoalId { get; init; }

    public string GoalName { get; init; } = string.Empty;

    public decimal TargetAmount { get; init; }

    public decimal CurrentAmount { get; init; }

    public decimal MonthlyContribution { get; init; }

    public DateOnly TargetDate { get; init; }

    public DateOnly? EstimatedCompletionDate { get; init; }

    public decimal MonthlyRequiredContribution { get; init; }

    public decimal ProjectedAmountAtTargetDate { get; init; }

    public decimal ShortfallAtTargetDate { get; init; }

    public bool IsOnTrack { get; init; }

    public IReadOnlyList<GoalProjectionPoint> Points { get; init; } = Array.Empty<GoalProjectionPoint>();

    public string CurrencyCode { get; init; } = "INR";
}

/// <summary>
/// Single projection chart point.
/// </summary>
public sealed class GoalProjectionPoint
{
    public DateOnly AsOf { get; init; }

    public string Label { get; init; } = string.Empty;

    public decimal ProjectedAmount { get; init; }
}

/// <summary>
/// Goals module dashboard summary KPIs (not persisted).
/// </summary>
public sealed class GoalDashboardSummary
{
    public int ActiveGoals { get; init; }

    public int CompletedGoals { get; init; }

    public int PausedGoals { get; init; }

    public decimal TotalGoalValue { get; init; }

    public decimal TotalSaved { get; init; }

    public decimal OverallProgressPercent { get; init; }

    public decimal MonthlyCommitted { get; init; }

    public IReadOnlyList<GoalUpcomingMilestone> UpcomingMilestones { get; init; } =
        Array.Empty<GoalUpcomingMilestone>();

    public string CurrencyCode { get; init; } = "INR";
}

/// <summary>
/// Compact upcoming milestone row for dashboard.
/// </summary>
public sealed class GoalUpcomingMilestone
{
    public Guid MilestoneId { get; init; }

    public Guid GoalId { get; init; }

    public string GoalName { get; init; } = string.Empty;

    public string Label { get; init; } = string.Empty;

    public decimal TargetPercent { get; init; }

    public decimal GoalCompletionPercent { get; init; }
}

/// <summary>
/// Placeholder recommendation structure (Phase 8 returns empty / static stubs — no AI).
/// Extension point for future AI / rule-based goal insights.
/// </summary>
public sealed class GoalRecommendation
{
    public Guid Id { get; init; }

    public Guid? GoalId { get; init; }

    public string Tag { get; init; } = string.Empty;

    public string Tone { get; init; } = "neutral";

    public string Title { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public string Impact { get; init; } = string.Empty;

    public string Action { get; init; } = string.Empty;
}

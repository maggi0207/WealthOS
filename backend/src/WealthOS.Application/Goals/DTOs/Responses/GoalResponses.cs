using WealthOS.Domain.Goals.Enums;

namespace WealthOS.Application.Goals.DTOs.Responses;

/// <summary>
/// Full goal detail response.
/// </summary>
public sealed class GoalResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public GoalCategory Category { get; set; }

    public decimal TargetAmount { get; set; }

    public decimal CurrentAmount { get; set; }

    public decimal RemainingAmount { get; set; }

    public decimal CompletionPercent { get; set; }

    public DateOnly TargetDate { get; set; }

    public DateOnly StartedOn { get; set; }

    public decimal MonthlyContribution { get; set; }

    public decimal MonthlyRequiredContribution { get; set; }

    public DateOnly? EstimatedCompletionDate { get; set; }

    public ProgressTrend Trend { get; set; }

    public GoalPriority Priority { get; set; }

    public GoalStatus Status { get; set; }

    public string? Description { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public Guid? LinkedPropertyId { get; set; }

    public Guid? LinkedInvestmentId { get; set; }

    public Guid? LinkedLoanId { get; set; }

    public Guid? LinkedIncomeSourceId { get; set; }

    public IReadOnlyList<GoalContributionResponse> Contributions { get; set; } =
        Array.Empty<GoalContributionResponse>();

    public IReadOnlyList<GoalMilestoneResponse> Milestones { get; set; } =
        Array.Empty<GoalMilestoneResponse>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Compact goal row for list endpoints.
/// </summary>
public sealed class GoalListItemResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public GoalCategory Category { get; set; }

    public decimal TargetAmount { get; set; }

    public decimal CurrentAmount { get; set; }

    public decimal CompletionPercent { get; set; }

    public DateOnly TargetDate { get; set; }

    public decimal MonthlyContribution { get; set; }

    public GoalPriority Priority { get; set; }

    public GoalStatus Status { get; set; }

    public ProgressTrend Trend { get; set; }

    public string CurrencyCode { get; set; } = "INR";
}

/// <summary>
/// Paginated goal list.
/// </summary>
public sealed class GoalListResponse
{
    public IReadOnlyList<GoalListItemResponse> Items { get; set; } = Array.Empty<GoalListItemResponse>();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}

/// <summary>
/// Contribution response.
/// </summary>
public sealed class GoalContributionResponse
{
    public Guid Id { get; set; }

    public Guid GoalId { get; set; }

    public decimal Amount { get; set; }

    public DateOnly ContributedOn { get; set; }

    public string? Notes { get; set; }

    public string? Source { get; set; }

    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Milestone response.
/// </summary>
public sealed class GoalMilestoneResponse
{
    public Guid Id { get; set; }

    public Guid GoalId { get; set; }

    public string Label { get; set; } = string.Empty;

    public decimal TargetPercent { get; set; }

    public decimal? TargetAmount { get; set; }

    public DateOnly? ReachedOn { get; set; }

    public bool IsCompleted { get; set; }

    public int SortOrder { get; set; }
}

/// <summary>
/// Progress response envelope.
/// </summary>
public sealed class GoalProgressResponse
{
    public Guid GoalId { get; set; }

    public string GoalName { get; set; } = string.Empty;

    public decimal TargetAmount { get; set; }

    public decimal CurrentAmount { get; set; }

    public decimal RemainingAmount { get; set; }

    public decimal CompletionPercent { get; set; }

    public decimal MonthlyContribution { get; set; }

    public decimal MonthlyRequiredContribution { get; set; }

    public DateOnly? EstimatedCompletionDate { get; set; }

    public DateOnly TargetDate { get; set; }

    public ProgressTrend Trend { get; set; }

    public int MonthsRemaining { get; set; }

    public string CurrencyCode { get; set; } = "INR";
}

/// <summary>
/// Projection response envelope.
/// </summary>
public sealed class GoalProjectionResponse
{
    public Guid GoalId { get; set; }

    public string GoalName { get; set; } = string.Empty;

    public decimal TargetAmount { get; set; }

    public decimal CurrentAmount { get; set; }

    public decimal MonthlyContribution { get; set; }

    public DateOnly TargetDate { get; set; }

    public DateOnly? EstimatedCompletionDate { get; set; }

    public decimal MonthlyRequiredContribution { get; set; }

    public decimal ProjectedAmountAtTargetDate { get; set; }

    public decimal ShortfallAtTargetDate { get; set; }

    public bool IsOnTrack { get; set; }

    public IReadOnlyList<GoalProjectionPointResponse> Points { get; set; } =
        Array.Empty<GoalProjectionPointResponse>();

    public string CurrencyCode { get; set; } = "INR";
}

/// <summary>
/// Projection chart point response.
/// </summary>
public sealed class GoalProjectionPointResponse
{
    public DateOnly AsOf { get; set; }

    public string Label { get; set; } = string.Empty;

    public decimal ProjectedAmount { get; set; }
}

/// <summary>
/// Goals dashboard summary response.
/// </summary>
public sealed class GoalDashboardResponse
{
    public int ActiveGoals { get; set; }

    public int CompletedGoals { get; set; }

    public int PausedGoals { get; set; }

    public decimal TotalGoalValue { get; set; }

    public decimal TotalSaved { get; set; }

    public decimal OverallProgressPercent { get; set; }

    public decimal MonthlyCommitted { get; set; }

    public IReadOnlyList<GoalUpcomingMilestoneResponse> UpcomingMilestones { get; set; } =
        Array.Empty<GoalUpcomingMilestoneResponse>();

    /// <summary>
    /// Placeholder recommendations (empty in Phase 8 — extension point for AI insights).
    /// </summary>
    public IReadOnlyList<GoalRecommendationResponse> Recommendations { get; set; } =
        Array.Empty<GoalRecommendationResponse>();

    public string CurrencyCode { get; set; } = "INR";
}

/// <summary>
/// Upcoming milestone row.
/// </summary>
public sealed class GoalUpcomingMilestoneResponse
{
    public Guid MilestoneId { get; set; }

    public Guid GoalId { get; set; }

    public string GoalName { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public decimal TargetPercent { get; set; }

    public decimal GoalCompletionPercent { get; set; }
}

/// <summary>
/// Placeholder recommendation DTO (no AI in Phase 8).
/// </summary>
public sealed class GoalRecommendationResponse
{
    public Guid Id { get; set; }

    public Guid? GoalId { get; set; }

    public string Tag { get; set; } = string.Empty;

    public string Tone { get; set; } = "neutral";

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public string Impact { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;
}

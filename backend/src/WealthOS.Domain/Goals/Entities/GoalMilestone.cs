using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Goals.Entities;

/// <summary>
/// A progress checkpoint toward completing a <see cref="FinancialGoal"/>.
/// </summary>
public sealed class GoalMilestone : AuditableEntity
{
    public GoalMilestone()
    {
    }

    public GoalMilestone(Guid id)
        : base(id)
    {
    }

    public Guid GoalId { get; set; }

    public FinancialGoal Goal { get; set; } = null!;

    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Target completion percentage (0–100) at which this milestone is considered reached.
    /// </summary>
    public decimal TargetPercent { get; set; }

    /// <summary>
    /// Optional absolute amount checkpoint (mirrors TargetPercent when set at create time).
    /// </summary>
    public decimal? TargetAmount { get; set; }

    public DateOnly? ReachedOn { get; set; }

    public bool IsCompleted { get; set; }

    public int SortOrder { get; set; }
}

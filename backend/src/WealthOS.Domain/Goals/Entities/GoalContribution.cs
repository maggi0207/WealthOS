using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Goals.Entities;

/// <summary>
/// A manual contribution recorded against a <see cref="FinancialGoal"/>.
/// Automatic contributions are intentionally not implemented in Phase 8.
/// </summary>
public sealed class GoalContribution : AuditableEntity
{
    public GoalContribution()
    {
    }

    public GoalContribution(Guid id)
        : base(id)
    {
    }

    public Guid GoalId { get; set; }

    public FinancialGoal Goal { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateOnly ContributedOn { get; set; }

    public string? Notes { get; set; }

    /// <summary>
    /// Free-text contribution source label (e.g. "Salary surplus", "Bonus").
    /// </summary>
    public string? Source { get; set; }
}

using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Goals.Enums;

namespace WealthOS.Domain.Goals.Entities;

/// <summary>
/// Aggregate root for a user financial goal.
/// Optional cross-module links are GUID-only (no ownership of Properties / Investments / Loans / Income).
/// </summary>
public sealed class FinancialGoal : AuditableEntity
{
    public FinancialGoal()
    {
    }

    public FinancialGoal(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

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

    /// <summary>
    /// Optional soft reference to a Property.Id — no EF navigation / cascade ownership.
    /// </summary>
    public Guid? LinkedPropertyId { get; set; }

    /// <summary>
    /// Optional soft reference to an investment holding or account Id — no EF navigation / cascade ownership.
    /// </summary>
    public Guid? LinkedInvestmentId { get; set; }

    /// <summary>
    /// Optional soft reference to a Loan.Id — no EF navigation / cascade ownership.
    /// </summary>
    public Guid? LinkedLoanId { get; set; }

    /// <summary>
    /// Optional soft reference to an IncomeSource.Id — no EF navigation / cascade ownership.
    /// </summary>
    public Guid? LinkedIncomeSourceId { get; set; }

    public ICollection<GoalContribution> Contributions { get; set; } = new List<GoalContribution>();

    public ICollection<GoalMilestone> Milestones { get; set; } = new List<GoalMilestone>();
}

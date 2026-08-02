using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Income.Enums;

namespace WealthOS.Domain.Income.Entities;

/// <summary>
/// Monthly payroll record for a developer.
/// </summary>
public sealed class DeveloperPayroll : AuditableEntity
{
    public DeveloperPayroll()
    {
    }

    public DeveloperPayroll(Guid id)
        : base(id)
    {
    }

    public Guid DeveloperId { get; set; }

    public Developer? Developer { get; set; }

    public Guid UserId { get; set; }

    public decimal Amount { get; set; }

    /// <summary>
    /// Period key in <c>yyyy-MM</c> form.
    /// </summary>
    public string Period { get; set; } = string.Empty;

    public PayrollStatus Status { get; set; } = PayrollStatus.Pending;

    public DateOnly? PaidOn { get; set; }

    public DateOnly? ScheduledOn { get; set; }

    public string? Notes { get; set; }
}

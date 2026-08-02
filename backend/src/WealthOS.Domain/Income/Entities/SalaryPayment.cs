using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Income.Entities;

/// <summary>
/// A credited salary payment for a given period (yyyy-MM).
/// </summary>
public sealed class SalaryPayment : AuditableEntity
{
    public SalaryPayment()
    {
    }

    public SalaryPayment(Guid id)
        : base(id)
    {
    }

    public Guid SalaryId { get; set; }

    public Salary? Salary { get; set; }

    public Guid UserId { get; set; }

    public decimal Amount { get; set; }

    public DateOnly PaidOn { get; set; }

    /// <summary>
    /// Period key in <c>yyyy-MM</c> form (e.g. 2026-07).
    /// </summary>
    public string Period { get; set; } = string.Empty;

    public string? Notes { get; set; }
}

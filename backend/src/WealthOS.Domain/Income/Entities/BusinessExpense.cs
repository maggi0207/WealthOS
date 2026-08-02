using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Income.Entities;

/// <summary>
/// Business operating expense.
/// </summary>
public sealed class BusinessExpense : AuditableEntity
{
    public BusinessExpense()
    {
    }

    public BusinessExpense(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }

    public ExpenseCategory? Category { get; set; }

    public string Vendor { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public DateOnly PaidOn { get; set; }

    public bool IsRecurring { get; set; }

    /// <summary>
    /// Optional period key in <c>yyyy-MM</c> form for reporting.
    /// </summary>
    public string? Period { get; set; }

    public string? Notes { get; set; }
}

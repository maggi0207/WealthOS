using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Income.Enums;

namespace WealthOS.Domain.Income.Entities;

/// <summary>
/// Lightweight catalog of income streams (salary / business / other).
/// Useful for future ERP-style source tracking; not required for every transaction.
/// </summary>
public sealed class IncomeSource : AuditableEntity
{
    public IncomeSource()
    {
    }

    public IncomeSource(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public IncomeSourceType Type { get; set; }

    /// <summary>
    /// Optional link to Salary.Id or BusinessClient.Id depending on <see cref="Type"/>.
    /// </summary>
    public Guid? LinkedEntityId { get; set; }

    public decimal MonthlyEstimate { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public bool IsActive { get; set; } = true;

    public string? Notes { get; set; }
}

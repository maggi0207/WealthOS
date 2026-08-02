using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Loans.Enums;

namespace WealthOS.Domain.Loans.Entities;

/// <summary>
/// Interest rate history entry. Supports future floating-rate and refinance tracking.
/// </summary>
public sealed class LoanInterestRate : AuditableEntity
{
    public Guid LoanId { get; set; }

    public Loan Loan { get; set; } = null!;

    /// <summary>
    /// Annual rate as a percentage.
    /// </summary>
    public decimal RatePercent { get; set; }

    public InterestType InterestType { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public string? Reason { get; set; }

    public string? Notes { get; set; }
}

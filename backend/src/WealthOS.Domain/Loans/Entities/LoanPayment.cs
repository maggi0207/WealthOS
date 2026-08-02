using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Loans.Enums;

namespace WealthOS.Domain.Loans.Entities;

/// <summary>
/// A payment recorded against a loan (EMI, partial, or prepayment).
/// </summary>
public sealed class LoanPayment : AuditableEntity
{
    public Guid LoanId { get; set; }

    public Loan Loan { get; set; } = null!;

    public DateOnly PaidOn { get; set; }

    public decimal Amount { get; set; }

    public decimal PrincipalComponent { get; set; }

    public decimal InterestComponent { get; set; }

    public LoanPaymentStatus Status { get; set; } = LoanPaymentStatus.Paid;

    public string? PaymentMode { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }

    /// <summary>
    /// True when this payment is a lump-sum prepayment (extension point for refinancing calculators).
    /// </summary>
    public bool IsPrepayment { get; set; }
}

namespace WealthOS.Domain.Loans.Enums;

/// <summary>
/// Outcome of a recorded loan payment.
/// </summary>
public enum LoanPaymentStatus
{
    Paid = 0,
    Pending = 1,
    Failed = 2,
    Partial = 3,
}

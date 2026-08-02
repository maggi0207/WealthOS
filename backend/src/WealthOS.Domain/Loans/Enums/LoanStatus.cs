namespace WealthOS.Domain.Loans.Enums;

/// <summary>
/// Lifecycle status of a loan account.
/// </summary>
public enum LoanStatus
{
    Active = 0,
    Closed = 1,
    Overdue = 2,
    Refinanced = 3,
    WrittenOff = 4,
    Pending = 5,
}

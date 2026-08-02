namespace WealthOS.Domain.Loans.Models;

/// <summary>
/// Read-model shape for portfolio-level loan totals (not persisted).
/// </summary>
public sealed class LoanSummary
{
    public int LoanCount { get; init; }

    public decimal TotalLoanAmount { get; init; }

    public decimal OutstandingBalance { get; init; }

    public decimal MonthlyEmi { get; init; }

    public decimal UpcomingEmi { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public int ActiveCount { get; init; }

    public int ClosedCount { get; init; }
}

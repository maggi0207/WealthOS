namespace WealthOS.Application.Loans.Calculations;

/// <summary>
/// Contract for reusable loan math. Swap implementations for amortisation / refinance engines later.
/// </summary>
public interface ILoanCalculationService
{
    /// <summary>
    /// Principal repaid so far: principal − outstanding (floored at zero).
    /// </summary>
    decimal CalculateTotalPrincipalPaid(decimal principal, decimal outstandingBalance);

    /// <summary>
    /// Sum of interest components from recorded payments.
    /// </summary>
    decimal CalculateTotalInterestPaid(IEnumerable<decimal> interestComponents);

    /// <summary>
    /// Progress through principal as a percentage 0–100.
    /// </summary>
    decimal CalculateLoanProgressPercent(decimal principal, decimal outstandingBalance);

    /// <summary>
    /// EMI progress through original tenure as a percentage 0–100.
    /// </summary>
    decimal CalculateEmiProgressPercent(int tenureMonths, int remainingTenureMonths);

    /// <summary>
    /// Remaining tenure in months (clamped ≥ 0). Prefer stored value when present.
    /// </summary>
    int CalculateRemainingTenureMonths(int tenureMonths, int remainingTenureMonths);

    /// <summary>
    /// Outstanding after applying a principal reduction (e.g. payment / prepayment).
    /// </summary>
    decimal CalculateOutstandingAfterPrincipalPayment(decimal outstandingBalance, decimal principalComponent);

    /// <summary>
    /// Simple prepayment estimate. Extension point — not a full amortisation engine.
    /// </summary>
    LoanPrepaymentEstimate EstimatePrepayment(
        decimal outstandingBalance,
        decimal emiAmount,
        decimal annualRatePercent,
        int remainingTenureMonths,
        decimal lumpSum);
}

/// <summary>
/// Result of a simple prepayment estimate.
/// </summary>
public sealed class LoanPrepaymentEstimate
{
    public decimal NewOutstanding { get; init; }

    public int EstimatedRemainingMonths { get; init; }

    public int MonthsSaved { get; init; }

    public decimal EstimatedInterestSaved { get; init; }

    public string CalculatorKey { get; init; } = "simple-emi";
}

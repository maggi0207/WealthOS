namespace WealthOS.Application.Loans.Calculations;

/// <summary>
/// Simple loan formulas. Complex amortisation schedules are intentionally not implemented.
/// </summary>
public sealed class LoanCalculationService : ILoanCalculationService
{
    public decimal CalculateTotalPrincipalPaid(decimal principal, decimal outstandingBalance)
    {
        var paid = principal - outstandingBalance;
        return paid < 0 ? 0m : paid;
    }

    public decimal CalculateTotalInterestPaid(IEnumerable<decimal> interestComponents) =>
        interestComponents.Sum();

    public decimal CalculateLoanProgressPercent(decimal principal, decimal outstandingBalance)
    {
        if (principal <= 0)
        {
            return 0m;
        }

        var paid = CalculateTotalPrincipalPaid(principal, outstandingBalance);
        var percent = paid / principal * 100m;
        return Math.Clamp(Math.Round(percent, 2), 0m, 100m);
    }

    public decimal CalculateEmiProgressPercent(int tenureMonths, int remainingTenureMonths)
    {
        if (tenureMonths <= 0)
        {
            return 0m;
        }

        var elapsed = tenureMonths - Math.Max(0, remainingTenureMonths);
        var percent = (decimal)elapsed / tenureMonths * 100m;
        return Math.Clamp(Math.Round(percent, 2), 0m, 100m);
    }

    public int CalculateRemainingTenureMonths(int tenureMonths, int remainingTenureMonths) =>
        Math.Clamp(remainingTenureMonths, 0, Math.Max(0, tenureMonths));

    public decimal CalculateOutstandingAfterPrincipalPayment(
        decimal outstandingBalance,
        decimal principalComponent)
    {
        var next = outstandingBalance - principalComponent;
        return next < 0 ? 0m : next;
    }

    /// <remarks>
    /// Rough months-remaining via closed-form EMI annuity when rate &gt; 0;
    /// falls back to balance / EMI when rate is zero. Not a full schedule engine.
    /// </remarks>
    public LoanPrepaymentEstimate EstimatePrepayment(
        decimal outstandingBalance,
        decimal emiAmount,
        decimal annualRatePercent,
        int remainingTenureMonths,
        decimal lumpSum)
    {
        var newOutstanding = CalculateOutstandingAfterPrincipalPayment(outstandingBalance, lumpSum);

        if (newOutstanding <= 0 || emiAmount <= 0)
        {
            return new LoanPrepaymentEstimate
            {
                NewOutstanding = 0m,
                EstimatedRemainingMonths = 0,
                MonthsSaved = Math.Max(0, remainingTenureMonths),
                EstimatedInterestSaved = Math.Max(0, remainingTenureMonths * emiAmount - lumpSum),
                CalculatorKey = "simple-emi",
            };
        }

        var beforeMonths = EstimateMonthsRemaining(outstandingBalance, emiAmount, annualRatePercent, remainingTenureMonths);
        var afterMonths = EstimateMonthsRemaining(newOutstanding, emiAmount, annualRatePercent, remainingTenureMonths);
        var monthsSaved = Math.Max(0, beforeMonths - afterMonths);
        var interestSaved = Math.Max(0, beforeMonths * emiAmount - lumpSum - afterMonths * emiAmount);

        return new LoanPrepaymentEstimate
        {
            NewOutstanding = newOutstanding,
            EstimatedRemainingMonths = afterMonths,
            MonthsSaved = monthsSaved,
            EstimatedInterestSaved = Math.Round(interestSaved, 2),
            CalculatorKey = "simple-emi",
        };
    }

    private static int EstimateMonthsRemaining(
        decimal balance,
        decimal emi,
        decimal annualRatePercent,
        int fallbackMonths)
    {
        if (balance <= 0)
        {
            return 0;
        }

        if (emi <= 0)
        {
            return fallbackMonths;
        }

        var monthlyRate = annualRatePercent / 100m / 12m;
        if (monthlyRate <= 0)
        {
            return (int)Math.Ceiling(balance / emi);
        }

        // months = -ln(1 - B*r/EMI) / ln(1+r)
        var ratio = 1m - balance * monthlyRate / emi;
        if (ratio <= 0)
        {
            return fallbackMonths;
        }

        var months = (int)Math.Ceiling(
            -(double)Math.Log((double)ratio) / Math.Log(1d + (double)monthlyRate));

        return Math.Max(0, months);
    }
}

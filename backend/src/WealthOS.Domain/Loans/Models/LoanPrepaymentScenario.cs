namespace WealthOS.Domain.Loans.Models;

/// <summary>
/// Prepayment impact estimate. Complex amortisation is NOT required;
/// this DTO is an extension point for future calculators / refinancing.
/// </summary>
public sealed class LoanPrepaymentScenario
{
    public Guid LoanId { get; init; }

    public decimal LumpSum { get; init; }

    public decimal CurrentOutstanding { get; init; }

    public decimal NewOutstanding { get; init; }

    public int CurrentRemainingMonths { get; init; }

    public int EstimatedRemainingMonths { get; init; }

    public int MonthsSaved { get; init; }

    public decimal EstimatedInterestSaved { get; init; }

    public string CalculatorKey { get; init; } = "simple-emi";

    public string? Notes { get; init; }
}

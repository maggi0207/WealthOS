namespace WealthOS.Domain.Dashboard.Models;

/// <summary>
/// Aggregated monetary totals for a user portfolio.
/// Phase 3 read model / value object — not mapped to EF.
/// </summary>
public sealed class FinancialSummary
{
    public decimal NetWorth { get; init; }

    public decimal AssetValue { get; init; }

    public decimal LiabilityValue { get; init; }

    public decimal MonthlyIncome { get; init; }

    public decimal MonthlyExpense { get; init; }

    public decimal InvestmentValue { get; init; }

    public decimal PropertyValue { get; init; }

    public decimal LoanBalance { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public decimal ChangePercent { get; init; }
}

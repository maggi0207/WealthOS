namespace WealthOS.Application.Dashboard.Providers;

/// <summary>
/// Cross-module loan totals. Implemented by the Loans module.
/// </summary>
public interface ILoanSummaryProvider
{
    Task<LoanModuleSummary> GetSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Loan module summary shape consumed by Dashboard.
/// </summary>
public sealed class LoanModuleSummary
{
    public decimal TotalBalance { get; init; }

    public int LoanCount { get; init; }

    public string CurrencyCode { get; init; } = "USD";
}

namespace WealthOS.Application.Dashboard.Providers;

/// <summary>
/// Cross-module investment totals. Implemented by Investments module later; mock in Phase 3.
/// </summary>
public interface IInvestmentSummaryProvider
{
    Task<InvestmentModuleSummary> GetSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Investment module summary shape consumed by Dashboard.
/// </summary>
public sealed class InvestmentModuleSummary
{
    public decimal TotalValue { get; init; }

    public int HoldingCount { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

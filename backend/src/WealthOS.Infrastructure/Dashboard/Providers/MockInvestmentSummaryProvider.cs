using WealthOS.Application.Dashboard.Providers;

namespace WealthOS.Infrastructure.Dashboard.Providers;

/// <summary>
/// Placeholder investment totals aligned with frontend dashboard demo data.
/// </summary>
public sealed class MockInvestmentSummaryProvider : IInvestmentSummaryProvider
{
    public Task<InvestmentModuleSummary> GetSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new InvestmentModuleSummary
        {
            TotalValue = 1_697_000m,
            HoldingCount = 12,
            CurrencyCode = "USD",
        });
    }
}

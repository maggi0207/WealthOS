using WealthOS.Application.Dashboard.Providers;

namespace WealthOS.Infrastructure.Dashboard.Providers;

/// <summary>
/// Placeholder loan totals aligned with frontend dashboard demo data.
/// </summary>
public sealed class MockLoanSummaryProvider : ILoanSummaryProvider
{
    public Task<LoanModuleSummary> GetSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new LoanModuleSummary
        {
            TotalBalance = 655_600m,
            LoanCount = 4,
            CurrencyCode = "USD",
        });
    }
}

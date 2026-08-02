using WealthOS.Application.Dashboard.Providers;

namespace WealthOS.Infrastructure.Dashboard.Providers;

/// <summary>
/// Placeholder property totals aligned with frontend dashboard demo data.
/// </summary>
public sealed class MockPropertySummaryProvider : IPropertySummaryProvider
{
    public Task<PropertyModuleSummary> GetSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new PropertyModuleSummary
        {
            TotalValue = 1_068_000m,
            PropertyCount = 2,
            CurrencyCode = "USD",
        });
    }
}

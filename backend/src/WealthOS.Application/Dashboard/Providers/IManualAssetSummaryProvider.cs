namespace WealthOS.Application.Dashboard.Providers;

/// <summary>
/// Cross-module manual asset totals for dashboard net-worth aggregation.
/// </summary>
public interface IManualAssetSummaryProvider
{
    Task<ManualAssetModuleSummary> GetSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Manual asset module summary shape consumed by Dashboard.
/// </summary>
public sealed class ManualAssetModuleSummary
{
    public decimal TotalValue { get; init; }

    public int AssetCount { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

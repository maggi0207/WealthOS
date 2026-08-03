using WealthOS.Application.Dashboard.Providers;
using WealthOS.Domain.Assets.Repositories;

namespace WealthOS.Infrastructure.Assets.Providers;

/// <summary>
/// Dashboard totals for manual assets.
/// </summary>
public sealed class ManualAssetSummaryProvider : IManualAssetSummaryProvider
{
    private readonly IManualAssetRepository _repository;

    public ManualAssetSummaryProvider(IManualAssetRepository repository)
    {
        _repository = repository;
    }

    public async Task<ManualAssetModuleSummary> GetSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var assets = await _repository.ListAllForUserAsync(userId, cancellationToken);
        var total = assets.Sum(asset => asset.CurrentValue);
        var currency = assets.FirstOrDefault()?.CurrencyCode ?? "INR";

        return new ManualAssetModuleSummary
        {
            TotalValue = total,
            AssetCount = assets.Count,
            CurrencyCode = string.IsNullOrWhiteSpace(currency) ? "INR" : currency,
        };
    }
}

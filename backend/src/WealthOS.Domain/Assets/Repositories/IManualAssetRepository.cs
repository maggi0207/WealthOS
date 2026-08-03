using WealthOS.Domain.Assets.Entities;
using WealthOS.Domain.Assets.Enums;
using WealthOS.Domain.Common.Abstractions.Repositories;

namespace WealthOS.Domain.Assets.Repositories;

/// <summary>
/// Persistence abstraction for manual assets.
/// </summary>
public interface IManualAssetRepository : IRepository<ManualAsset>
{
    Task<ManualAsset?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ManualAsset> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        ManualAssetType? type,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ManualAsset>> ListAllForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalCurrentValueAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);
}

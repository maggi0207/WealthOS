using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Assets.Entities;
using WealthOS.Domain.Assets.Enums;
using WealthOS.Domain.Assets.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Repositories;

namespace WealthOS.Infrastructure.Assets.Repositories;

/// <summary>
/// EF Core repository for manual assets.
/// </summary>
public sealed class ManualAssetRepository : Repository<ManualAsset>, IManualAssetRepository
{
    public ManualAssetRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<ManualAsset?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            asset => asset.Id == id && asset.UserId == userId,
            cancellationToken);

    public async Task<(IReadOnlyList<ManualAsset> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        ManualAssetType? type,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(asset => asset.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(asset =>
                asset.Name.ToLower().Contains(term)
                || (asset.Institution != null && asset.Institution.ToLower().Contains(term))
                || (asset.Notes != null && asset.Notes.ToLower().Contains(term)));
        }

        if (type.HasValue)
        {
            query = query.Where(asset => asset.Type == type.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(asset => asset.UpdatedAt ?? asset.CreatedAt)
            .ThenBy(asset => asset.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<ManualAsset>> ListAllForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Where(asset => asset.UserId == userId)
            .OrderByDescending(asset => asset.UpdatedAt ?? asset.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<decimal> GetTotalCurrentValueAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Where(asset => asset.UserId == userId)
            .SumAsync(asset => asset.CurrentValue, cancellationToken);

    public async Task<bool> ExistsForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.AnyAsync(
            asset => asset.Id == id && asset.UserId == userId,
            cancellationToken);
}

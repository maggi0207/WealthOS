using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Domain.Properties.Enums;
using WealthOS.Domain.Properties.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Repositories;

namespace WealthOS.Infrastructure.Properties.Repositories;

/// <summary>
/// EF Core repository for the Property aggregate.
/// </summary>
public sealed class PropertyRepository : Repository<Property>, IPropertyRepository
{
    public PropertyRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Property?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            property => property.Id == id && property.UserId == userId,
            cancellationToken);

    public async Task<Property?> GetByIdWithDetailsAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsSplitQuery()
            .Include(property => property.Address)
            .Include(property => property.Owners)
            .Include(property => property.Valuations)
            .Include(property => property.LoanLinks)
            .Include(property => property.DocumentLinks)
            .Include(property => property.Images)
            .Include(property => property.PropertyNotes)
            .FirstOrDefaultAsync(
                property => property.Id == id && property.UserId == userId,
                cancellationToken);

    public async Task<(IReadOnlyList<Property> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        PropertyStatus? status,
        PropertyType? type,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(property => property.Address)
            .Include(property => property.Owners)
            .Where(property => property.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(property =>
                property.Name.ToLower().Contains(term)
                || (property.Address != null && property.Address.City != null
                    && property.Address.City.ToLower().Contains(term))
                || (property.Address != null && property.Address.Locality != null
                    && property.Address.Locality.ToLower().Contains(term))
                || (property.Address != null && property.Address.FullAddress != null
                    && property.Address.FullAddress.ToLower().Contains(term)));
        }

        if (status.HasValue)
        {
            query = query.Where(property => property.Status == status.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(property => property.Type == type.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(property => property.UpdatedAt ?? property.CreatedAt)
            .ThenBy(property => property.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<PropertyPortfolioTotals> GetPortfolioTotalsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var properties = await DbSet
            .AsNoTracking()
            .Where(property => property.UserId == userId)
            .Select(property => new
            {
                property.PurchasePrice,
                property.CurrentMarketValue,
                property.CurrencyCode,
                property.Status,
            })
            .ToListAsync(cancellationToken);

        if (properties.Count == 0)
        {
            return new PropertyPortfolioTotals();
        }

        return new PropertyPortfolioTotals
        {
            PropertyCount = properties.Count,
            TotalPurchasePrice = properties.Sum(item => item.PurchasePrice),
            TotalMarketValue = properties.Sum(item => item.CurrentMarketValue),
            CurrencyCode = properties
                .GroupBy(item => item.CurrencyCode)
                .OrderByDescending(group => group.Count())
                .First()
                .Key,
            ActiveCount = properties.Count(item => item.Status == PropertyStatus.Active),
            RentedCount = properties.Count(item => item.Status == PropertyStatus.Rented),
        };
    }

    public Task<bool> ExistsForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        DbSet.AnyAsync(property => property.Id == id && property.UserId == userId, cancellationToken);
}

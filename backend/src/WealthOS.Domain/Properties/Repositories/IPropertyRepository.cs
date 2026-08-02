using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Domain.Properties.Enums;

namespace WealthOS.Domain.Properties.Repositories;

/// <summary>
/// Persistence abstraction for the Property aggregate.
/// </summary>
public interface IPropertyRepository : IRepository<Property>
{
    Task<Property?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Property?> GetByIdWithDetailsAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Property> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        PropertyStatus? status,
        PropertyType? type,
        CancellationToken cancellationToken = default);

    Task<PropertyPortfolioTotals> GetPortfolioTotalsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Aggregated portfolio totals for summary/dashboard queries.
/// </summary>
public sealed class PropertyPortfolioTotals
{
    public int PropertyCount { get; init; }

    public decimal TotalPurchasePrice { get; init; }

    public decimal TotalMarketValue { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public int ActiveCount { get; init; }

    public int RentedCount { get; init; }
}

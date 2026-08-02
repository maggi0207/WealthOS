using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Investments.Enums;

namespace WealthOS.Domain.Investments.Entities;

/// <summary>
/// User watchlist entry (symbols to track; no live prices in Phase 7).
/// </summary>
public sealed class WatchlistItem : AuditableEntity
{
    public WatchlistItem()
    {
    }

    public WatchlistItem(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public InvestmentCategory Category { get; set; } = InvestmentCategory.Other;

    public decimal? TargetPrice { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public string? Notes { get; set; }
}

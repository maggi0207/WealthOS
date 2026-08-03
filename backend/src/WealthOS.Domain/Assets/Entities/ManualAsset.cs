using WealthOS.Domain.Assets.Enums;
using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Assets.Entities;

/// <summary>
/// User-owned manual asset (cash, gold, vehicle, etc.).
/// Does not duplicate Properties or Investments records.
/// </summary>
public sealed class ManualAsset : AuditableEntity
{
    public ManualAsset()
    {
    }

    public ManualAsset(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public ManualAssetType Type { get; set; } = ManualAssetType.Other;

    public string Name { get; set; } = string.Empty;

    public decimal PurchaseValue { get; set; }

    public decimal CurrentValue { get; set; }

    public decimal? Quantity { get; set; }

    public string? Institution { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public string? Notes { get; set; }

    public string CurrencyCode { get; set; } = "INR";
}

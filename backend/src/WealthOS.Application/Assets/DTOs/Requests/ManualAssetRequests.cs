using WealthOS.Domain.Assets.Enums;

namespace WealthOS.Application.Assets.DTOs.Requests;

/// <summary>
/// Creates a manual asset owned by the authenticated user.
/// </summary>
public sealed class CreateManualAssetRequest
{
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

/// <summary>
/// Updates an existing manual asset.
/// </summary>
public sealed class UpdateManualAssetRequest
{
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

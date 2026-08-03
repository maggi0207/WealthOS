using WealthOS.Domain.Assets.Enums;

namespace WealthOS.Application.Assets.DTOs.Responses;

/// <summary>
/// Full manual asset response.
/// </summary>
public sealed class ManualAssetResponse
{
    public Guid Id { get; set; }

    public ManualAssetType Type { get; set; }

    public string TypeLabel { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal PurchaseValue { get; set; }

    public decimal CurrentValue { get; set; }

    public decimal GainLoss { get; set; }

    public decimal? GainLossPercent { get; set; }

    public decimal? Quantity { get; set; }

    public string? Institution { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public string? Notes { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Paginated list of manual assets.
/// </summary>
public sealed class ManualAssetListResponse
{
    public IReadOnlyList<ManualAssetResponse> Items { get; set; } = Array.Empty<ManualAssetResponse>();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public decimal TotalCurrentValue { get; set; }

    public string CurrencyCode { get; set; } = "INR";
}

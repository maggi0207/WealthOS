using WealthOS.Domain.Investments.Enums;

namespace WealthOS.Application.Investments.DTOs.Responses;

public sealed class InvestmentProviderResponse
{
    public Guid Id { get; init; }

    public ProviderKind Kind { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public bool IsEnabled { get; init; }

    public bool SupportsSync { get; init; }
}

public sealed class InvestmentProviderListResponse
{
    public IReadOnlyList<InvestmentProviderResponse> Items { get; init; } = Array.Empty<InvestmentProviderResponse>();

    public int TotalCount { get; init; }
}

public sealed class InvestmentAccountResponse
{
    public Guid Id { get; init; }

    public Guid ProviderId { get; init; }

    public string ProviderName { get; init; } = string.Empty;

    public ProviderKind ProviderKind { get; init; }

    public string Name { get; init; } = string.Empty;

    public string OwnerName { get; init; } = string.Empty;

    public string KindLabel { get; init; } = string.Empty;

    public InvestmentAccountStatus Status { get; init; }

    public DateTime? LastSyncedAt { get; init; }

    public decimal CurrentValue { get; init; }

    public decimal InvestedAmount { get; init; }

    public decimal DayChange { get; init; }

    public decimal DayChangePercent { get; init; }

    public int HoldingsCount { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public string? Notes { get; init; }
}

public sealed class InvestmentAccountListResponse
{
    public IReadOnlyList<InvestmentAccountResponse> Items { get; init; } = Array.Empty<InvestmentAccountResponse>();

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }
}

public sealed class HoldingResponse
{
    public Guid Id { get; init; }

    public Guid AccountId { get; init; }

    public string AccountName { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Symbol { get; init; } = string.Empty;

    public InvestmentCategory Category { get; init; }

    public InvestmentType InvestmentType { get; init; }

    public decimal Quantity { get; init; }

    public decimal AverageCost { get; init; }

    public decimal InvestedAmount { get; init; }

    public decimal CurrentPrice { get; init; }

    public decimal CurrentValue { get; init; }

    public decimal DayChange { get; init; }

    public decimal DayChangePercent { get; init; }

    public decimal OverallGain { get; init; }

    public decimal AbsoluteReturnPercent { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public string? Notes { get; init; }
}

public sealed class HoldingListResponse
{
    public IReadOnlyList<HoldingResponse> Items { get; init; } = Array.Empty<HoldingResponse>();

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }
}

public sealed class InvestmentTransactionResponse
{
    public Guid Id { get; init; }

    public Guid AccountId { get; init; }

    public Guid? HoldingId { get; init; }

    public string? HoldingName { get; init; }

    public InvestmentTransactionType TransactionType { get; init; }

    public decimal Quantity { get; init; }

    public decimal Price { get; init; }

    public decimal Amount { get; init; }

    public decimal Fees { get; init; }

    public DateOnly TransactionDate { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public string? Notes { get; init; }

    public string? ExternalReference { get; init; }
}

public sealed class InvestmentTransactionListResponse
{
    public IReadOnlyList<InvestmentTransactionResponse> Items { get; init; } = Array.Empty<InvestmentTransactionResponse>();

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }
}

public sealed class PortfolioResponse
{
    public decimal InvestedAmount { get; init; }

    public decimal CurrentValue { get; init; }

    public decimal TodaysGain { get; init; }

    public decimal TodaysGainPercent { get; init; }

    public decimal OverallGain { get; init; }

    public decimal AbsoluteReturnPercent { get; init; }

    /// <summary>XIRR placeholder — not calculated in Phase 7.</summary>
    public decimal? XirrPercent { get; init; }

    public int AccountCount { get; init; }

    public int HoldingCount { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

public sealed class PortfolioSummaryResponse
{
    public decimal PortfolioValue { get; init; }

    public decimal InvestedAmount { get; init; }

    public decimal TodaysGain { get; init; }

    public decimal TodaysGainPercent { get; init; }

    public decimal TotalReturn { get; init; }

    public decimal AbsoluteReturnPercent { get; init; }

    public decimal XirrPlaceholderPercent { get; init; }

    public int AccountCount { get; init; }

    public int HoldingCount { get; init; }

    public string? LargestHoldingName { get; init; }

    public decimal? LargestHoldingValue { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

public sealed class AssetAllocationSliceResponse
{
    public InvestmentCategory Category { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public decimal Value { get; init; }

    public decimal WeightPercent { get; init; }
}

public sealed class AssetAllocationResponse
{
    public decimal TotalValue { get; init; }

    public IReadOnlyList<AssetAllocationSliceResponse> Slices { get; init; } = Array.Empty<AssetAllocationSliceResponse>();

    public string CurrencyCode { get; init; } = "INR";
}

public sealed class PerformancePointResponse
{
    public string Label { get; init; } = string.Empty;

    public decimal Value { get; init; }
}

public sealed class InvestmentPerformanceResponse
{
    public PerformanceRange Range { get; init; }

    public IReadOnlyList<PerformancePointResponse> Points { get; init; } = Array.Empty<PerformancePointResponse>();

    public decimal AbsoluteReturnPercent { get; init; }

    /// <summary>XIRR placeholder — null until real IRR is implemented.</summary>
    public decimal? XirrPercent { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

public sealed class InvestmentDashboardResponse
{
    public decimal PortfolioValue { get; init; }

    public decimal TodaysGain { get; init; }

    public decimal TodaysGainPercent { get; init; }

    public decimal TotalReturn { get; init; }

    public decimal AbsoluteReturnPercent { get; init; }

    public int AccountCount { get; init; }

    public int HoldingCount { get; init; }

    public string? LargestHoldingName { get; init; }

    public decimal? LargestHoldingValue { get; init; }

    public AssetAllocationResponse Allocation { get; init; } = new();

    public string CurrencyCode { get; init; } = "INR";
}

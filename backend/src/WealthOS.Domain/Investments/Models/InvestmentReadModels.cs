using WealthOS.Domain.Investments.Enums;

namespace WealthOS.Domain.Investments.Models;

/// <summary>
/// Computed portfolio view across one or all accounts (not persisted).
/// </summary>
public sealed class Portfolio
{
    public decimal InvestedAmount { get; init; }

    public decimal CurrentValue { get; init; }

    public decimal TodaysGain { get; init; }

    public decimal TodaysGainPercent { get; init; }

    public decimal OverallGain { get; init; }

    public decimal AbsoluteReturnPercent { get; init; }

    /// <summary>
    /// XIRR placeholder — not calculated in Phase 7.
    /// </summary>
    public decimal? XirrPercent { get; init; }

    public int AccountCount { get; init; }

    public int HoldingCount { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

/// <summary>
/// Compact portfolio summary KPIs (not persisted).
/// </summary>
public sealed class PortfolioSummary
{
    public decimal PortfolioValue { get; init; }

    public decimal InvestedAmount { get; init; }

    public decimal TodaysGain { get; init; }

    public decimal TodaysGainPercent { get; init; }

    public decimal TotalReturn { get; init; }

    public decimal AbsoluteReturnPercent { get; init; }

    /// <summary>
    /// XIRR placeholder value for UI (not a real IRR).
    /// </summary>
    public decimal XirrPlaceholderPercent { get; init; }

    public int AccountCount { get; init; }

    public int HoldingCount { get; init; }

    public string? LargestHoldingName { get; init; }

    public decimal? LargestHoldingValue { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

/// <summary>
/// Asset allocation breakdown (not persisted — derived from holdings).
/// </summary>
public sealed class AssetAllocation
{
    public decimal TotalValue { get; init; }

    public IReadOnlyList<AssetAllocationSlice> Slices { get; init; } = Array.Empty<AssetAllocationSlice>();

    public string CurrencyCode { get; init; } = "INR";
}

/// <summary>
/// Single allocation slice by category.
/// </summary>
public sealed class AssetAllocationSlice
{
    public InvestmentCategory Category { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public decimal Value { get; init; }

    public decimal WeightPercent { get; init; }
}

/// <summary>
/// Performance series for charts (not persisted — derived from snapshots / seed).
/// </summary>
public sealed class InvestmentPerformance
{
    public PerformanceRange Range { get; init; }

    public IReadOnlyList<PerformancePoint> Points { get; init; } = Array.Empty<PerformancePoint>();

    public decimal AbsoluteReturnPercent { get; init; }

    /// <summary>
    /// XIRR placeholder — always null / documented stub until cash-flow IRR is implemented.
    /// </summary>
    public decimal? XirrPercent { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

/// <summary>
/// Single performance chart point (values in lakhs or absolute currency units).
/// </summary>
public sealed class PerformancePoint
{
    public string Label { get; init; } = string.Empty;

    public decimal Value { get; init; }
}

/// <summary>
/// Investments module dashboard summary (not persisted).
/// </summary>
public sealed class InvestmentDashboardSummary
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

    public AssetAllocation Allocation { get; init; } = new();

    public string CurrencyCode { get; init; } = "INR";
}

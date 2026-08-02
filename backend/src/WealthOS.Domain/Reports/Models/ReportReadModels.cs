using WealthOS.Domain.Reports.Enums;

namespace WealthOS.Domain.Reports.Models;

/// <summary>
/// Cross-cutting filter criteria applied when aggregating module data for reports.
/// </summary>
public sealed class ReportFilter
{
    public DateTime? FromDate { get; init; }

    public DateTime? ToDate { get; init; }

    public string? Category { get; init; }

    public string? Owner { get; init; }

    public Guid? PropertyId { get; init; }

    public Guid? InvestmentAccountId { get; init; }

    public Guid? BusinessClientId { get; init; }

    public Guid? GoalId { get; init; }

    public Guid? LoanId { get; init; }

    public AnalyticsPeriod? Period { get; init; }

    public string? PeriodLabel { get; init; }
}

/// <summary>
/// Named parameter supplied to a report execution.
/// </summary>
public sealed class ReportParameter
{
    public string Name { get; init; } = string.Empty;

    public string? Value { get; init; }
}

/// <summary>
/// Generic report result envelope used by domain services before DTO mapping.
/// </summary>
public sealed class ReportResult
{
    public ReportType ReportType { get; init; }

    public string Title { get; init; } = string.Empty;

    public DateTime GeneratedAt { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public ReportFilter? Filters { get; init; }

    public string PayloadJson { get; init; } = "{}";

    public IReadOnlyList<string> DataSources { get; init; } = Array.Empty<string>();
}

/// <summary>
/// Cross-module analytics KPIs (trends, ratios, growth).
/// </summary>
public sealed class AnalyticsSummary
{
    public DateTime GeneratedAt { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public decimal NetWorth { get; init; }

    public decimal NetWorthGrowthPercent { get; init; }

    public decimal SavingsRatePercent { get; init; }

    public decimal InvestmentReturnPercent { get; init; }

    public decimal BusinessProfit { get; init; }

    public decimal DebtRatioPercent { get; init; }

    public IReadOnlyList<TrendPoint> NetWorthTrend { get; init; } = Array.Empty<TrendPoint>();

    public IReadOnlyList<TrendPoint> CashFlowTrend { get; init; } = Array.Empty<TrendPoint>();

    public IReadOnlyList<TrendPoint> MonthlyIncomeTrend { get; init; } = Array.Empty<TrendPoint>();
}

/// <summary>
/// Composite financial health score derived from module summaries.
/// </summary>
public sealed class FinancialHealth
{
    public int Score { get; init; }

    public FinancialHealthGrade Grade { get; init; }

    public string Summary { get; init; } = string.Empty;

    public DateTime CalculatedAt { get; init; }

    public decimal DebtToAssetRatioPercent { get; init; }

    public decimal SavingsRatePercent { get; init; }

    public decimal GoalProgressPercent { get; init; }

    public decimal LiquidityScore { get; init; }

    public IReadOnlyList<FinancialHealthFactor> Factors { get; init; } = Array.Empty<FinancialHealthFactor>();
}

/// <summary>
/// Individual factor contributing to <see cref="FinancialHealth"/>.
/// </summary>
public sealed class FinancialHealthFactor
{
    public string Code { get; init; } = string.Empty;

    public string Label { get; init; } = string.Empty;

    public int Score { get; init; }

    public string Weight { get; init; } = string.Empty;

    public string? Detail { get; init; }
}

/// <summary>
/// Time-series point for analytics trends.
/// </summary>
public sealed class TrendPoint
{
    public string Label { get; init; } = string.Empty;

    public decimal Value { get; init; }

    public DateTime? AsOf { get; init; }
}

/// <summary>
/// Lightweight catalog summary for report definitions.
/// </summary>
public sealed class ReportDefinitionSummary
{
    public Guid Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public ReportType ReportType { get; init; }

    public ReportDefinitionStatus Status { get; init; }
}

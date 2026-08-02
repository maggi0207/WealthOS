using WealthOS.Domain.Reports.Enums;

namespace WealthOS.Application.Reports.DTOs.Responses;

/// <summary>Echo of applied report filters.</summary>
public sealed class ReportFilterResponse
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

/// <summary>Common report metadata envelope.</summary>
public abstract class ReportResponseBase
{
    public ReportType ReportType { get; init; }

    public string Title { get; init; } = string.Empty;

    public DateTime GeneratedAt { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public ReportFilterResponse? Filters { get; init; }

    public IReadOnlyList<string> DataSources { get; init; } = Array.Empty<string>();
}

/// <summary>Net worth report aggregating assets and liabilities.</summary>
public sealed class NetWorthReportResponse : ReportResponseBase
{
    public decimal NetWorth { get; init; }

    public decimal AssetValue { get; init; }

    public decimal LiabilityValue { get; init; }

    public decimal PropertyValue { get; init; }

    public decimal InvestmentValue { get; init; }

    public decimal LoanBalance { get; init; }

    public decimal ChangePercent { get; init; }
}

/// <summary>Cash flow report (income vs outflows).</summary>
public sealed class CashFlowReportResponse : ReportResponseBase
{
    public string Period { get; init; } = string.Empty;

    public decimal SalaryIncome { get; init; }

    public decimal BusinessRevenue { get; init; }

    public decimal TotalInflow { get; init; }

    public decimal DeveloperPayroll { get; init; }

    public decimal BusinessExpenses { get; init; }

    public decimal PersonalOutflow { get; init; }

    public decimal TotalOutflow { get; init; }

    public decimal NetCashFlow { get; init; }

    public decimal SavingsRatePercent { get; init; }
}

/// <summary>Investment performance and allocation report.</summary>
public sealed class InvestmentReportResponse : ReportResponseBase
{
    public decimal PortfolioValue { get; init; }

    public decimal InvestedAmount { get; init; }

    public decimal TotalReturn { get; init; }

    public decimal AbsoluteReturnPercent { get; init; }

    public decimal? XirrPercent { get; init; }

    public int AccountCount { get; init; }

    public int HoldingCount { get; init; }

    public IReadOnlyList<ReportAllocationSliceResponse> Allocation { get; init; } =
        Array.Empty<ReportAllocationSliceResponse>();

    public IReadOnlyList<ReportTrendPointResponse> PerformancePoints { get; init; } =
        Array.Empty<ReportTrendPointResponse>();
}

/// <summary>Asset allocation slice.</summary>
public sealed class ReportAllocationSliceResponse
{
    public string Category { get; init; } = string.Empty;

    public decimal Value { get; init; }

    public decimal Percent { get; init; }
}

/// <summary>Loan analysis report.</summary>
public sealed class LoanReportResponse : ReportResponseBase
{
    public int LoanCount { get; init; }

    public decimal TotalLoanAmount { get; init; }

    public decimal OutstandingBalance { get; init; }

    public decimal MonthlyEmi { get; init; }

    public decimal UpcomingEmi { get; init; }

    public decimal DebtRatioPercent { get; init; }

    public int ActiveCount { get; init; }

    public int ClosedCount { get; init; }
}

/// <summary>Property appreciation report.</summary>
public sealed class PropertyReportResponse : ReportResponseBase
{
    public int PropertyCount { get; init; }

    public decimal TotalPurchasePrice { get; init; }

    public decimal TotalMarketValue { get; init; }

    public decimal TotalAppreciation { get; init; }

    public decimal? TotalAppreciationPercent { get; init; }

    public int ActiveCount { get; init; }

    public int RentedCount { get; init; }
}

/// <summary>Business P&amp;L report.</summary>
public sealed class BusinessReportResponse : ReportResponseBase
{
    public string Period { get; init; } = string.Empty;

    public decimal BusinessRevenue { get; init; }

    public decimal DeveloperCost { get; init; }

    public decimal BusinessExpenses { get; init; }

    public decimal GrossProfit { get; init; }

    public decimal NetProfit { get; init; }

    public decimal SalaryIncome { get; init; }

    public decimal TotalIncome { get; init; }

    public decimal SavingsRatePercent { get; init; }
}

/// <summary>Goal progress report.</summary>
public sealed class GoalReportResponse : ReportResponseBase
{
    public int ActiveGoals { get; init; }

    public int CompletedGoals { get; init; }

    public int PausedGoals { get; init; }

    public decimal TotalGoalValue { get; init; }

    public decimal TotalSaved { get; init; }

    public decimal OverallProgressPercent { get; init; }

    public decimal MonthlyCommitted { get; init; }
}

/// <summary>Document summary report.</summary>
public sealed class DocumentReportResponse : ReportResponseBase
{
    public int DocumentCount { get; init; }

    public int PendingReviewCount { get; init; }

    public int RecentCount { get; init; }

    public int ExpiredCount { get; init; }

    public int UnreadNotifications { get; init; }
}

/// <summary>Financial health score report.</summary>
public sealed class FinancialHealthResponse : ReportResponseBase
{
    public int Score { get; init; }

    public FinancialHealthGrade Grade { get; init; }

    public string GradeLabel { get; init; } = string.Empty;

    public string Summary { get; init; } = string.Empty;

    public decimal DebtToAssetRatioPercent { get; init; }

    public decimal SavingsRatePercent { get; init; }

    public decimal GoalProgressPercent { get; init; }

    public decimal LiquidityScore { get; init; }

    public IReadOnlyList<FinancialHealthFactorResponse> Factors { get; init; } =
        Array.Empty<FinancialHealthFactorResponse>();
}

/// <summary>Financial health factor DTO.</summary>
public sealed class FinancialHealthFactorResponse
{
    public string Code { get; init; } = string.Empty;

    public string Label { get; init; } = string.Empty;

    public int Score { get; init; }

    public string Weight { get; init; } = string.Empty;

    public string? Detail { get; init; }
}

/// <summary>Cross-module analytics summary.</summary>
public sealed class AnalyticsSummaryResponse : ReportResponseBase
{
    public decimal NetWorth { get; init; }

    public decimal NetWorthGrowthPercent { get; init; }

    public decimal SavingsRatePercent { get; init; }

    public decimal InvestmentReturnPercent { get; init; }

    public decimal BusinessProfit { get; init; }

    public decimal DebtRatioPercent { get; init; }

    public IReadOnlyList<ReportTrendPointResponse> NetWorthTrend { get; init; } =
        Array.Empty<ReportTrendPointResponse>();

    public IReadOnlyList<ReportTrendPointResponse> CashFlowTrend { get; init; } =
        Array.Empty<ReportTrendPointResponse>();

    public IReadOnlyList<ReportTrendPointResponse> MonthlyIncomeTrend { get; init; } =
        Array.Empty<ReportTrendPointResponse>();
}

/// <summary>Trend point DTO.</summary>
public sealed class ReportTrendPointResponse
{
    public string Label { get; init; } = string.Empty;

    public decimal Value { get; init; }

    public DateTime? AsOf { get; init; }
}

/// <summary>Persisted snapshot metadata + payload.</summary>
public sealed class ReportSnapshotResponse
{
    public Guid Id { get; init; }

    public ReportType ReportType { get; init; }

    public string Title { get; init; } = string.Empty;

    public DateTime CapturedAt { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public string PayloadJson { get; init; } = "{}";

    public ReportFilterResponse? Filters { get; init; }
}

/// <summary>
/// Export placeholder response — documents that generation is not implemented.
/// </summary>
public sealed class ReportExportResponse
{
    public Guid Id { get; init; }

    public ReportType ReportType { get; init; }

    public ReportExportFormat Format { get; init; }

    public ReportExportStatus Status { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? FileName { get; init; }

    public string? ContentType { get; init; }

    public DateTime RequestedAt { get; init; }

    public IReadOnlyList<string> SupportedFormats { get; init; } =
        new[] { "Csv", "Excel", "Pdf", "Json" };
}

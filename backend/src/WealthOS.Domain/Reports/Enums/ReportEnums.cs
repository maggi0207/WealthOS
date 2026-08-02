namespace WealthOS.Domain.Reports.Enums;

/// <summary>Catalog of supported WealthOS report types.</summary>
public enum ReportType
{
    NetWorth = 0,
    CashFlow = 1,
    Income = 2,
    BusinessProfitAndLoss = 3,
    InvestmentPerformance = 4,
    AssetAllocation = 5,
    PropertyAppreciation = 6,
    LoanAnalysis = 7,
    GoalProgress = 8,
    DocumentSummary = 9,
    FinancialHealthScore = 10,
    AnalyticsSummary = 11,
}

/// <summary>Lifecycle status of a report definition.</summary>
public enum ReportDefinitionStatus
{
    Draft = 0,
    Active = 1,
    Deprecated = 2,
}

/// <summary>Outcome of a report execution.</summary>
public enum ReportExecutionStatus
{
    Pending = 0,
    Running = 1,
    Succeeded = 2,
    Failed = 3,
    Cancelled = 4,
}

/// <summary>Supported export formats (architecture placeholder — no generation libraries).</summary>
public enum ReportExportFormat
{
    Csv = 0,
    Excel = 1,
    Pdf = 2,
    Json = 3,
}

/// <summary>Lifecycle of an export request.</summary>
public enum ReportExportStatus
{
    Requested = 0,
    Queued = 1,
    Completed = 2,
    Failed = 3,
    NotImplemented = 4,
}

/// <summary>Analytics trend granularity.</summary>
public enum AnalyticsPeriod
{
    Monthly = 0,
    Yearly = 1,
}

/// <summary>Financial health grade band.</summary>
public enum FinancialHealthGrade
{
    F = 0,
    D = 1,
    C = 2,
    B = 3,
    A = 4,
    APlus = 5,
}

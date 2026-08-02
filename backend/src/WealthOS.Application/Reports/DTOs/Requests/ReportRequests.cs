using WealthOS.Domain.Reports.Enums;

namespace WealthOS.Application.Reports.DTOs.Requests;

/// <summary>
/// Shared filter / query parameters for report endpoints.
/// </summary>
public sealed class ReportFilterRequest
{
    /// <summary>Inclusive start date for the reporting window.</summary>
    public DateTime? FromDate { get; init; }

    /// <summary>Inclusive end date for the reporting window.</summary>
    public DateTime? ToDate { get; init; }

    /// <summary>Optional category filter (module-specific meaning).</summary>
    public string? Category { get; init; }

    /// <summary>Optional owner / party filter.</summary>
    public string? Owner { get; init; }

    /// <summary>Scope to a property.</summary>
    public Guid? PropertyId { get; init; }

    /// <summary>Scope to an investment account.</summary>
    public Guid? InvestmentAccountId { get; init; }

    /// <summary>Scope to a business client.</summary>
    public Guid? BusinessClientId { get; init; }

    /// <summary>Scope to a goal.</summary>
    public Guid? GoalId { get; init; }

    /// <summary>Scope to a loan.</summary>
    public Guid? LoanId { get; init; }

    /// <summary>Analytics period: Monthly or Yearly.</summary>
    public AnalyticsPeriod? Period { get; init; }

    /// <summary>Income/business period label (e.g. 2026-08 or 2026).</summary>
    public string? PeriodLabel { get; init; }
}

/// <summary>
/// Request to capture a report snapshot.
/// </summary>
public sealed class GenerateSnapshotRequest
{
    public ReportType ReportType { get; init; }

    public string? Title { get; init; }

    public ReportFilterRequest? Filters { get; init; }
}

/// <summary>
/// Request to export a report (architecture placeholder — no file generation).
/// </summary>
public sealed class ExportReportRequest
{
    public ReportType ReportType { get; init; }

    public ReportExportFormat Format { get; init; } = ReportExportFormat.Json;

    public Guid? SnapshotId { get; init; }

    public ReportFilterRequest? Filters { get; init; }
}

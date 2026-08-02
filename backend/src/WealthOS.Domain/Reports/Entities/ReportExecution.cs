using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Reports.Enums;

namespace WealthOS.Domain.Reports.Entities;

/// <summary>
/// Audit record of a report generation run.
/// </summary>
public sealed class ReportExecution : AuditableEntity
{
    public ReportExecution()
    {
    }

    public ReportExecution(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid? ReportId { get; set; }

    public Report? Report { get; set; }

    public Guid? ReportDefinitionId { get; set; }

    public ReportDefinition? Definition { get; set; }

    public ReportType ReportType { get; set; }

    public ReportExecutionStatus Status { get; set; } = ReportExecutionStatus.Pending;

    public string? FiltersJson { get; set; }

    public string? ParametersJson { get; set; }

    public string? ResultSummaryJson { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int? DurationMs { get; set; }
}

using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Reports.Enums;

namespace WealthOS.Domain.Reports.Entities;

/// <summary>
/// Persisted point-in-time snapshot of an aggregated report payload (JSON).
/// </summary>
public sealed class ReportSnapshot : AuditableEntity
{
    public ReportSnapshot()
    {
    }

    public ReportSnapshot(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid? ReportExecutionId { get; set; }

    public ReportExecution? Execution { get; set; }

    public ReportType ReportType { get; set; }

    public string Title { get; set; } = string.Empty;

    public string PayloadJson { get; set; } = "{}";

    public string? FiltersJson { get; set; }

    public DateTime CapturedAt { get; set; }

    public string CurrencyCode { get; set; } = "INR";
}

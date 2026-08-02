using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Reports.Enums;

namespace WealthOS.Domain.Reports.Entities;

/// <summary>
/// Export request metadata only — file generation is not implemented in Phase 12.
/// </summary>
public sealed class ReportExport : AuditableEntity
{
    public ReportExport()
    {
    }

    public ReportExport(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid? ReportSnapshotId { get; set; }

    public ReportSnapshot? Snapshot { get; set; }

    public Guid? ReportExecutionId { get; set; }

    public ReportType ReportType { get; set; }

    public ReportExportFormat Format { get; set; }

    public ReportExportStatus Status { get; set; } = ReportExportStatus.NotImplemented;

    public string? FileName { get; set; }

    public string? ContentType { get; set; }

    public string? Message { get; set; }

    public DateTime RequestedAt { get; set; }
}

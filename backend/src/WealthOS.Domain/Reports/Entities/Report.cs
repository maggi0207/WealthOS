using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Reports.Enums;

namespace WealthOS.Domain.Reports.Entities;

/// <summary>
/// User-facing report instance referencing a definition (metadata only — no business data ownership).
/// </summary>
public sealed class Report : AuditableEntity
{
    public Report()
    {
    }

    public Report(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid ReportDefinitionId { get; set; }

    public ReportDefinition? Definition { get; set; }

    public string Title { get; set; } = string.Empty;

    public ReportType ReportType { get; set; }

    public string? LastFiltersJson { get; set; }

    public DateTime? LastGeneratedAt { get; set; }

    public Guid? LastExecutionId { get; set; }
}

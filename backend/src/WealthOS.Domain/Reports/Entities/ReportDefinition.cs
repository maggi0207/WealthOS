using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Reports.Enums;

namespace WealthOS.Domain.Reports.Entities;

/// <summary>
/// Catalog entry describing a report type available in WealthOS.
/// </summary>
public sealed class ReportDefinition : AuditableEntity
{
    public ReportDefinition()
    {
    }

    public ReportDefinition(Guid id)
        : base(id)
    {
    }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ReportType ReportType { get; set; }

    public ReportDefinitionStatus Status { get; set; } = ReportDefinitionStatus.Active;

    public string? DefaultFiltersJson { get; set; }

    public string? ParameterSchemaJson { get; set; }

    public int SortOrder { get; set; }

    public bool IsSystem { get; set; } = true;
}

using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Properties.Entities;

/// <summary>
/// Property image metadata. Upload/storage APIs are intentionally out of Phase 4 scope.
/// </summary>
public sealed class PropertyImage : AuditableEntity
{
    public Guid PropertyId { get; set; }

    public Property Property { get; set; } = null!;

    public string? Url { get; set; }

    public string? StorageKey { get; set; }

    public string? Caption { get; set; }

    public string? Category { get; set; }

    public int SortOrder { get; set; }

    public bool IsPrimary { get; set; }
}

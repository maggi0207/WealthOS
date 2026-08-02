using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Properties.Entities;

/// <summary>
/// Stub link from a property to a future Documents module record (no FK to Documents yet).
/// </summary>
public sealed class PropertyDocumentLink : AuditableEntity
{
    public Guid PropertyId { get; set; }

    public Property Property { get; set; } = null!;

    public Guid DocumentId { get; set; }

    public string? Notes { get; set; }
}

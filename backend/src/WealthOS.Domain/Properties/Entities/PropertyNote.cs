using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Properties.Entities;

/// <summary>
/// Structured note attached to a property (distinct from the free-text Notes field on Property).
/// </summary>
public sealed class PropertyNote : AuditableEntity
{
    public Guid PropertyId { get; set; }

    public Property Property { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;
}

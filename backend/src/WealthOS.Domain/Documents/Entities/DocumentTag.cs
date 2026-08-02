using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Documents.Entities;

/// <summary>
/// Free-form tag attached to a document.
/// </summary>
public sealed class DocumentTag : AuditableEntity
{
    public DocumentTag()
    {
    }

    public DocumentTag(Guid id)
        : base(id)
    {
    }

    public Guid DocumentId { get; set; }

    public Document Document { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
}

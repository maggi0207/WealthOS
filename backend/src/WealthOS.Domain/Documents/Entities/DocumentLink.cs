using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Documents.Enums;

namespace WealthOS.Domain.Documents.Entities;

/// <summary>
/// Soft GUID link from a document to another WealthOS module record.
/// No EF navigation / cascade ownership of target modules.
/// </summary>
public sealed class DocumentLink : AuditableEntity
{
    public DocumentLink()
    {
    }

    public DocumentLink(Guid id)
        : base(id)
    {
    }

    public Guid DocumentId { get; set; }

    public Document Document { get; set; } = null!;

    public DocumentReferenceModule ReferenceModule { get; set; }

    public Guid ReferenceId { get; set; }

    public string? Notes { get; set; }
}

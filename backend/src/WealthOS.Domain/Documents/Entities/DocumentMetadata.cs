using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Documents.Entities;

/// <summary>
/// Extended metadata for a document (1:1). Placeholder fields for future OCR / checksums.
/// </summary>
public sealed class DocumentMetadata : AuditableEntity
{
    public DocumentMetadata()
    {
    }

    public DocumentMetadata(Guid id)
        : base(id)
    {
    }

    public Guid DocumentId { get; set; }

    public Document Document { get; set; } = null!;

    public string? DocumentNumber { get; set; }

    public string? IssuedBy { get; set; }

    public string? IssuerCountry { get; set; }

    /// <summary>
    /// Placeholder content checksum (not computed in Phase 9).
    /// </summary>
    public string? Checksum { get; set; }

    public int? PageCount { get; set; }

    /// <summary>
    /// Optional JSON bag for future custom attributes.
    /// </summary>
    public string? CustomAttributesJson { get; set; }
}

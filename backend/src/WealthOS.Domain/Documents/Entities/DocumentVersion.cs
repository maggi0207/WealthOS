using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Documents.Enums;

namespace WealthOS.Domain.Documents.Entities;

/// <summary>
/// Version snapshot of document file metadata (storage placeholders only).
/// </summary>
public sealed class DocumentVersion : AuditableEntity
{
    public DocumentVersion()
    {
    }

    public DocumentVersion(Guid id)
        : base(id)
    {
    }

    public Guid DocumentId { get; set; }

    public Document Document { get; set; } = null!;

    public int VersionNumber { get; set; }

    public string? OriginalFileName { get; set; }

    public string? ContentType { get; set; }

    public long FileSizeBytes { get; set; }

    public DocumentStorageProvider StorageProvider { get; set; } = DocumentStorageProvider.LocalPlaceholder;

    public string? StoragePath { get; set; }

    public string? Notes { get; set; }
}

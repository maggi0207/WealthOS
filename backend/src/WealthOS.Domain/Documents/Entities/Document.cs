using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Documents.Enums;

namespace WealthOS.Domain.Documents.Entities;

/// <summary>
/// Aggregate root for a vault document (metadata + storage placeholders only).
/// Cross-module links are GUID-only soft references — no cascade ownership.
/// </summary>
public sealed class Document : AuditableEntity
{
    public Document()
    {
    }

    public Document(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DocumentCategory Category { get; set; } = DocumentCategory.Other;

    /// <summary>
    /// Display owner label (may differ from the authenticated vault user).
    /// </summary>
    public string Owner { get; set; } = string.Empty;

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    /// <summary>
    /// Optional next reminder date mirrored from reminders when convenient.
    /// </summary>
    public DateOnly? ReminderDate { get; set; }

    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;

    public DocumentAccess AccessLevel { get; set; } = DocumentAccess.Private;

    /// <summary>
    /// Optional primary soft reference module (GUID-only).
    /// </summary>
    public DocumentReferenceModule ReferenceModule { get; set; } = DocumentReferenceModule.None;

    /// <summary>
    /// Optional primary soft reference id matching <see cref="ReferenceModule"/>.
    /// </summary>
    public Guid? ReferenceId { get; set; }

    public string? Notes { get; set; }

    public string? OriginalFileName { get; set; }

    public string? ContentType { get; set; }

    public long FileSizeBytes { get; set; }

    public DocumentStorageProvider StorageProvider { get; set; } = DocumentStorageProvider.None;

    /// <summary>
    /// Placeholder object key / path — not a real filesystem location in Phase 9.
    /// </summary>
    public string? StoragePath { get; set; }

    public DocumentMetadata? Metadata { get; set; }

    public ICollection<DocumentTag> Tags { get; set; } = new List<DocumentTag>();

    public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();

    public ICollection<DocumentLink> Links { get; set; } = new List<DocumentLink>();

    public ICollection<DocumentReminder> Reminders { get; set; } = new List<DocumentReminder>();
}

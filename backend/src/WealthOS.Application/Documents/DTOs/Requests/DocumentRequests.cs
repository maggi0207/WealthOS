using WealthOS.Domain.Documents.Enums;

namespace WealthOS.Application.Documents.DTOs.Requests;

/// <summary>
/// Creates a new vault document (metadata + storage placeholders).
/// </summary>
public sealed class CreateDocumentRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DocumentCategory Category { get; set; } = DocumentCategory.Other;

    public string Owner { get; set; } = string.Empty;

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public DateOnly? ReminderDate { get; set; }

    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;

    public DocumentAccess AccessLevel { get; set; } = DocumentAccess.Private;

    public DocumentReferenceModule ReferenceModule { get; set; } = DocumentReferenceModule.None;

    public Guid? ReferenceId { get; set; }

    public string? Notes { get; set; }

    public string? OriginalFileName { get; set; }

    public string? ContentType { get; set; }

    public long FileSizeBytes { get; set; }

    public DocumentStorageProvider StorageProvider { get; set; } = DocumentStorageProvider.LocalPlaceholder;

    public string? StoragePath { get; set; }

    public IReadOnlyList<string>? Tags { get; set; }

    public UpsertDocumentMetadataRequest? Metadata { get; set; }

    public IReadOnlyList<CreateDocumentLinkRequest>? Links { get; set; }
}

/// <summary>
/// Updates an existing vault document.
/// </summary>
public sealed class UpdateDocumentRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DocumentCategory Category { get; set; } = DocumentCategory.Other;

    public string Owner { get; set; } = string.Empty;

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public DateOnly? ReminderDate { get; set; }

    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;

    public DocumentAccess AccessLevel { get; set; } = DocumentAccess.Private;

    public DocumentReferenceModule ReferenceModule { get; set; } = DocumentReferenceModule.None;

    public Guid? ReferenceId { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// Updates storage metadata placeholders without performing a real file upload.
/// </summary>
public sealed class UploadDocumentMetadataRequest
{
    public string OriginalFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public DocumentStorageProvider StorageProvider { get; set; } = DocumentStorageProvider.LocalPlaceholder;

    public string? StoragePath { get; set; }

    public string? VersionNotes { get; set; }

    public UpsertDocumentMetadataRequest? Metadata { get; set; }
}

/// <summary>
/// Extended metadata upsert payload.
/// </summary>
public sealed class UpsertDocumentMetadataRequest
{
    public string? DocumentNumber { get; set; }

    public string? IssuedBy { get; set; }

    public string? IssuerCountry { get; set; }

    public string? Checksum { get; set; }

    public int? PageCount { get; set; }

    public string? CustomAttributesJson { get; set; }
}

/// <summary>
/// Adds a tag to a document.
/// </summary>
public sealed class AddDocumentTagRequest
{
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Creates a reminder for a document.
/// </summary>
public sealed class CreateDocumentReminderRequest
{
    public DateOnly ReminderDate { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? Notes { get; set; }
}

/// <summary>
/// Soft link to another module record.
/// </summary>
public sealed class CreateDocumentLinkRequest
{
    public DocumentReferenceModule ReferenceModule { get; set; }

    public Guid ReferenceId { get; set; }

    public string? Notes { get; set; }
}

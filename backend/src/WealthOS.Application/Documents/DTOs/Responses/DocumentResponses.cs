using WealthOS.Domain.Documents.Enums;

namespace WealthOS.Application.Documents.DTOs.Responses;

/// <summary>
/// Full document detail response.
/// </summary>
public sealed class DocumentResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DocumentCategory Category { get; set; }

    public string Owner { get; set; } = string.Empty;

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public DateOnly? ReminderDate { get; set; }

    public DocumentStatus Status { get; set; }

    public DocumentAccess AccessLevel { get; set; }

    public DocumentReferenceModule ReferenceModule { get; set; }

    public Guid? ReferenceId { get; set; }

    public string? Notes { get; set; }

    public string? OriginalFileName { get; set; }

    public string? ContentType { get; set; }

    public long FileSizeBytes { get; set; }

    public DocumentStorageProvider StorageProvider { get; set; }

    public string? StoragePath { get; set; }

    public DocumentMetadataResponse? Metadata { get; set; }

    public IReadOnlyList<DocumentTagResponse> Tags { get; set; } = Array.Empty<DocumentTagResponse>();

    public IReadOnlyList<DocumentVersionResponse> Versions { get; set; } =
        Array.Empty<DocumentVersionResponse>();

    public IReadOnlyList<DocumentLinkResponse> Links { get; set; } = Array.Empty<DocumentLinkResponse>();

    public IReadOnlyList<DocumentReminderResponse> Reminders { get; set; } =
        Array.Empty<DocumentReminderResponse>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Compact document row for list endpoints.
/// </summary>
public sealed class DocumentListItemResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public DocumentCategory Category { get; set; }

    public DocumentStatus Status { get; set; }

    public string Owner { get; set; } = string.Empty;

    public DateOnly? ExpiryDate { get; set; }

    public string? OriginalFileName { get; set; }

    public string? ContentType { get; set; }

    public long FileSizeBytes { get; set; }

    public DocumentReferenceModule ReferenceModule { get; set; }

    public Guid? ReferenceId { get; set; }

    public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Paginated document list.
/// </summary>
public sealed class DocumentListResponse
{
    public IReadOnlyList<DocumentListItemResponse> Items { get; set; } =
        Array.Empty<DocumentListItemResponse>();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}

/// <summary>
/// Extended metadata response.
/// </summary>
public sealed class DocumentMetadataResponse
{
    public Guid Id { get; set; }

    public Guid DocumentId { get; set; }

    public string? DocumentNumber { get; set; }

    public string? IssuedBy { get; set; }

    public string? IssuerCountry { get; set; }

    public string? Checksum { get; set; }

    public int? PageCount { get; set; }

    public string? CustomAttributesJson { get; set; }
}

/// <summary>
/// Tag response.
/// </summary>
public sealed class DocumentTagResponse
{
    public Guid Id { get; set; }

    public Guid DocumentId { get; set; }

    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Version response (storage placeholders).
/// </summary>
public sealed class DocumentVersionResponse
{
    public Guid Id { get; set; }

    public Guid DocumentId { get; set; }

    public int VersionNumber { get; set; }

    public string? OriginalFileName { get; set; }

    public string? ContentType { get; set; }

    public long FileSizeBytes { get; set; }

    public DocumentStorageProvider StorageProvider { get; set; }

    public string? StoragePath { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Soft link response.
/// </summary>
public sealed class DocumentLinkResponse
{
    public Guid Id { get; set; }

    public Guid DocumentId { get; set; }

    public DocumentReferenceModule ReferenceModule { get; set; }

    public Guid ReferenceId { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// Reminder response.
/// </summary>
public sealed class DocumentReminderResponse
{
    public Guid Id { get; set; }

    public Guid DocumentId { get; set; }

    public DateOnly ReminderDate { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool IsDismissed { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
}

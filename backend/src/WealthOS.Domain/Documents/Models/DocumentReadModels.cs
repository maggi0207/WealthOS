using WealthOS.Domain.Documents.Enums;

namespace WealthOS.Domain.Documents.Models;

/// <summary>
/// Criteria for document search across title, category, tags, owner, and references.
/// </summary>
public sealed class DocumentSearchCriteria
{
    public string? Title { get; init; }

    public DocumentCategory? Category { get; init; }

    public string? Tag { get; init; }

    public string? Owner { get; init; }

    public DocumentReferenceModule? ReferenceModule { get; init; }

    public Guid? ReferenceId { get; init; }

    public DocumentStatus? Status { get; init; }

    public string? FreeText { get; init; }
}

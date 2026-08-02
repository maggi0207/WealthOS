using WealthOS.Application.Common.Abstractions;
using WealthOS.Domain.Documents.Enums;

namespace WealthOS.Application.Documents.Queries;

/// <summary>
/// Lists documents for the authenticated user.
/// </summary>
public sealed class GetDocumentsQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? Search { get; init; }

    public DocumentCategory? Category { get; init; }

    public DocumentStatus? Status { get; init; }
}

/// <summary>
/// Gets a single document by identifier.
/// </summary>
public sealed class GetDocumentByIdQuery : IQuery
{
    public Guid DocumentId { get; init; }
}

/// <summary>
/// Searches documents by title, category, tags, owner, and references.
/// </summary>
public sealed class SearchDocumentsQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? Title { get; init; }

    public DocumentCategory? Category { get; init; }

    public string? Tag { get; init; }

    public string? Owner { get; init; }

    public DocumentReferenceModule? ReferenceModule { get; init; }

    public Guid? ReferenceId { get; init; }

    public DocumentStatus? Status { get; init; }

    public string? FreeText { get; init; }
}

/// <summary>
/// Returns recently updated documents.
/// </summary>
public sealed class GetRecentDocumentsQuery : IQuery
{
    public int Take { get; init; } = 10;
}

/// <summary>
/// Returns expired (or past-expiry) documents.
/// </summary>
public sealed class GetExpiredDocumentsQuery : IQuery
{
    public int Take { get; init; } = 50;
}

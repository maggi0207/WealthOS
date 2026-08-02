using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Documents.Entities;
using WealthOS.Domain.Documents.Enums;
using WealthOS.Domain.Documents.Models;

namespace WealthOS.Domain.Documents.Repositories;

/// <summary>
/// Persistence abstraction for the Document aggregate.
/// </summary>
public interface IDocumentRepository : IRepository<Document>
{
    Task<Document?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Document?> GetByIdWithDetailsAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Document> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        DocumentCategory? category,
        DocumentStatus? status,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Document> Items, int TotalCount)> SearchForUserAsync(
        Guid userId,
        DocumentSearchCriteria criteria,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Document>> ListRecentForUserAsync(
        Guid userId,
        int take,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Document>> ListExpiredForUserAsync(
        Guid userId,
        DateOnly asOf,
        int take,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Persistence abstraction for document tags.
/// </summary>
public interface IDocumentTagRepository : IRepository<DocumentTag>
{
    Task<DocumentTag?> GetByIdForUserAsync(
        Guid tagId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid documentId,
        string name,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Persistence abstraction for document reminders.
/// </summary>
public interface IDocumentReminderRepository : IRepository<DocumentReminder>
{
    Task<DocumentReminder?> GetByIdForUserAsync(
        Guid reminderId,
        Guid userId,
        CancellationToken cancellationToken = default);
}

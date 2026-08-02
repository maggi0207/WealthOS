using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Documents.Entities;
using WealthOS.Domain.Documents.Enums;
using WealthOS.Domain.Documents.Models;
using WealthOS.Domain.Documents.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Repositories;

namespace WealthOS.Infrastructure.Documents.Repositories;

/// <summary>
/// EF Core repository for the Document aggregate.
/// </summary>
public sealed class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Document?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            document => document.Id == id && document.UserId == userId,
            cancellationToken);

    public async Task<Document?> GetByIdWithDetailsAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsSplitQuery()
            .Include(document => document.Metadata)
            .Include(document => document.Tags)
            .Include(document => document.Versions)
            .Include(document => document.Links)
            .Include(document => document.Reminders)
            .FirstOrDefaultAsync(
                document => document.Id == id && document.UserId == userId,
                cancellationToken);

    public async Task<(IReadOnlyList<Document> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        DocumentCategory? category,
        DocumentStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(document => document.Tags)
            .Where(document => document.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(document =>
                document.Title.ToLower().Contains(term)
                || document.Owner.ToLower().Contains(term)
                || (document.Description != null && document.Description.ToLower().Contains(term))
                || document.Tags.Any(tag => tag.Name.Contains(term)));
        }

        if (category.HasValue)
        {
            query = query.Where(document => document.Category == category.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(document => document.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(document => document.UpdatedAt ?? document.CreatedAt)
            .ThenBy(document => document.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<Document> Items, int TotalCount)> SearchForUserAsync(
        Guid userId,
        DocumentSearchCriteria criteria,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(document => document.Tags)
            .Include(document => document.Links)
            .Where(document => document.UserId == userId);

        if (!string.IsNullOrWhiteSpace(criteria.Title))
        {
            var title = criteria.Title.Trim().ToLowerInvariant();
            query = query.Where(document => document.Title.ToLower().Contains(title));
        }

        if (criteria.Category.HasValue)
        {
            query = query.Where(document => document.Category == criteria.Category.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Tag))
        {
            var tag = criteria.Tag.Trim().ToLowerInvariant();
            query = query.Where(document => document.Tags.Any(item => item.Name.Contains(tag)));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Owner))
        {
            var owner = criteria.Owner.Trim().ToLowerInvariant();
            query = query.Where(document => document.Owner.ToLower().Contains(owner));
        }

        if (criteria.ReferenceModule.HasValue)
        {
            var module = criteria.ReferenceModule.Value;
            query = query.Where(document =>
                document.ReferenceModule == module
                || document.Links.Any(link => link.ReferenceModule == module));
        }

        if (criteria.ReferenceId.HasValue)
        {
            var referenceId = criteria.ReferenceId.Value;
            query = query.Where(document =>
                document.ReferenceId == referenceId
                || document.Links.Any(link => link.ReferenceId == referenceId));
        }

        if (criteria.Status.HasValue)
        {
            query = query.Where(document => document.Status == criteria.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.FreeText))
        {
            var term = criteria.FreeText.Trim().ToLowerInvariant();
            query = query.Where(document =>
                document.Title.ToLower().Contains(term)
                || document.Owner.ToLower().Contains(term)
                || (document.Description != null && document.Description.ToLower().Contains(term))
                || (document.Notes != null && document.Notes.ToLower().Contains(term))
                || document.Tags.Any(tag => tag.Name.Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(document => document.UpdatedAt ?? document.CreatedAt)
            .ThenBy(document => document.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Document>> ListRecentForUserAsync(
        Guid userId,
        int take,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Include(document => document.Tags)
            .Where(document => document.UserId == userId)
            .OrderByDescending(document => document.UpdatedAt ?? document.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Document>> ListExpiredForUserAsync(
        Guid userId,
        DateOnly asOf,
        int take,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Include(document => document.Tags)
            .Where(document =>
                document.UserId == userId
                && document.ExpiryDate.HasValue
                && (document.ExpiryDate.Value < asOf || document.Status == DocumentStatus.Expired))
            .OrderBy(document => document.ExpiryDate)
            .ThenBy(document => document.Title)
            .Take(take)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.AnyAsync(
            document => document.Id == id && document.UserId == userId,
            cancellationToken);
}

/// <summary>
/// EF Core repository for document tags.
/// </summary>
public sealed class DocumentTagRepository : Repository<DocumentTag>, IDocumentTagRepository
{
    public DocumentTagRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<DocumentTag?> GetByIdForUserAsync(
        Guid tagId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(tag => tag.Document)
            .FirstOrDefaultAsync(
                tag => tag.Id == tagId && tag.Document.UserId == userId,
                cancellationToken);

    public async Task<bool> ExistsAsync(
        Guid documentId,
        string name,
        CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim().ToLowerInvariant();
        return await DbSet.AnyAsync(
            tag => tag.DocumentId == documentId && tag.Name == normalized,
            cancellationToken);
    }
}

/// <summary>
/// EF Core repository for document reminders.
/// </summary>
public sealed class DocumentReminderRepository : Repository<DocumentReminder>, IDocumentReminderRepository
{
    public DocumentReminderRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<DocumentReminder?> GetByIdForUserAsync(
        Guid reminderId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(reminder => reminder.Document)
            .FirstOrDefaultAsync(
                reminder => reminder.Id == reminderId && reminder.Document.UserId == userId,
                cancellationToken);
}

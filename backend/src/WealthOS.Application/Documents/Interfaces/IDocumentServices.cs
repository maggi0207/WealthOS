using WealthOS.Application.Common.Models;
using WealthOS.Application.Documents.DTOs.Requests;
using WealthOS.Application.Documents.DTOs.Responses;
using WealthOS.Domain.Documents.Enums;

namespace WealthOS.Application.Documents.Interfaces;

/// <summary>
/// Application service for document CRUD, tags, reminders, and metadata uploads.
/// </summary>
public interface IDocumentService
{
    Task<Result<DocumentResponse>> CreateAsync(
        CreateDocumentRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<DocumentResponse>> UpdateAsync(
        Guid documentId,
        UpdateDocumentRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<Result<DocumentResponse>> UploadMetadataAsync(
        Guid documentId,
        UploadDocumentMetadataRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<DocumentTagResponse>> AddTagAsync(
        Guid documentId,
        AddDocumentTagRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveTagAsync(
        Guid documentId,
        Guid tagId,
        CancellationToken cancellationToken = default);

    Task<Result<DocumentReminderResponse>> CreateReminderAsync(
        Guid documentId,
        CreateDocumentReminderRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<DocumentResponse>> GetByIdAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task<Result<DocumentListResponse>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        DocumentCategory? category,
        DocumentStatus? status,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Search and filtered document queries.
/// </summary>
public interface IDocumentSearchService
{
    Task<Result<DocumentListResponse>> SearchAsync(
        string? title,
        DocumentCategory? category,
        string? tag,
        string? owner,
        DocumentReferenceModule? referenceModule,
        Guid? referenceId,
        DocumentStatus? status,
        string? freeText,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<DocumentListResponse>> GetRecentAsync(
        int take,
        CancellationToken cancellationToken = default);

    Task<Result<DocumentListResponse>> GetExpiredAsync(
        int take,
        CancellationToken cancellationToken = default);
}

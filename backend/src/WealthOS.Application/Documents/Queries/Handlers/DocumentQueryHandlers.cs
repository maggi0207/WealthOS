using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Documents.DTOs.Responses;
using WealthOS.Application.Documents.Interfaces;
using WealthOS.Application.Documents.Queries;

namespace WealthOS.Application.Documents.Queries.Handlers;

public sealed class GetDocumentsQueryHandler
    : IQueryHandler<GetDocumentsQuery, DocumentListResponse>
{
    private readonly IDocumentService _documentService;

    public GetDocumentsQueryHandler(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public Task<Result<DocumentListResponse>> HandleAsync(
        GetDocumentsQuery query,
        CancellationToken cancellationToken = default) =>
        _documentService.GetAllAsync(
            query.Page,
            query.PageSize,
            query.Search,
            query.Category,
            query.Status,
            cancellationToken);
}

public sealed class GetDocumentByIdQueryHandler
    : IQueryHandler<GetDocumentByIdQuery, DocumentResponse>
{
    private readonly IDocumentService _documentService;

    public GetDocumentByIdQueryHandler(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public Task<Result<DocumentResponse>> HandleAsync(
        GetDocumentByIdQuery query,
        CancellationToken cancellationToken = default) =>
        _documentService.GetByIdAsync(query.DocumentId, cancellationToken);
}

public sealed class SearchDocumentsQueryHandler
    : IQueryHandler<SearchDocumentsQuery, DocumentListResponse>
{
    private readonly IDocumentSearchService _searchService;

    public SearchDocumentsQueryHandler(IDocumentSearchService searchService)
    {
        _searchService = searchService;
    }

    public Task<Result<DocumentListResponse>> HandleAsync(
        SearchDocumentsQuery query,
        CancellationToken cancellationToken = default) =>
        _searchService.SearchAsync(
            query.Title,
            query.Category,
            query.Tag,
            query.Owner,
            query.ReferenceModule,
            query.ReferenceId,
            query.Status,
            query.FreeText,
            query.Page,
            query.PageSize,
            cancellationToken);
}

public sealed class GetRecentDocumentsQueryHandler
    : IQueryHandler<GetRecentDocumentsQuery, DocumentListResponse>
{
    private readonly IDocumentSearchService _searchService;

    public GetRecentDocumentsQueryHandler(IDocumentSearchService searchService)
    {
        _searchService = searchService;
    }

    public Task<Result<DocumentListResponse>> HandleAsync(
        GetRecentDocumentsQuery query,
        CancellationToken cancellationToken = default) =>
        _searchService.GetRecentAsync(query.Take, cancellationToken);
}

public sealed class GetExpiredDocumentsQueryHandler
    : IQueryHandler<GetExpiredDocumentsQuery, DocumentListResponse>
{
    private readonly IDocumentSearchService _searchService;

    public GetExpiredDocumentsQueryHandler(IDocumentSearchService searchService)
    {
        _searchService = searchService;
    }

    public Task<Result<DocumentListResponse>> HandleAsync(
        GetExpiredDocumentsQuery query,
        CancellationToken cancellationToken = default) =>
        _searchService.GetExpiredAsync(query.Take, cancellationToken);
}

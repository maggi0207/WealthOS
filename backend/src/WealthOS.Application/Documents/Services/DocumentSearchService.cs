using AutoMapper;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Documents.DTOs.Responses;
using WealthOS.Application.Documents.Interfaces;
using WealthOS.Domain.Documents.Entities;
using WealthOS.Domain.Documents.Enums;
using WealthOS.Domain.Documents.Models;
using WealthOS.Domain.Documents.Repositories;

namespace WealthOS.Application.Documents.Services;

/// <summary>
/// Search and filtered document query orchestration.
/// </summary>
public sealed class DocumentSearchService : IDocumentSearchService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public DocumentSearchService(
        IDocumentRepository documentRepository,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _documentRepository = documentRepository;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<DocumentListResponse>> SearchAsync(
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
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DocumentListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, totalCount) = await _documentRepository.SearchForUserAsync(
            userResult.Value,
            new DocumentSearchCriteria
            {
                Title = title,
                Category = category,
                Tag = tag,
                Owner = owner,
                ReferenceModule = referenceModule,
                ReferenceId = referenceId,
                Status = status,
                FreeText = freeText,
            },
            page,
            pageSize,
            cancellationToken);

        return Result.Success(DocumentServiceHelpers.BuildListResponse(
            items.Select(MapListItem).ToList(),
            page,
            pageSize,
            totalCount));
    }

    public async Task<Result<DocumentListResponse>> GetRecentAsync(
        int take,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DocumentListResponse>(userResult.Error!);
        }

        take = Math.Clamp(take, 1, 50);
        var items = await _documentRepository.ListRecentForUserAsync(
            userResult.Value,
            take,
            cancellationToken);

        return Result.Success(DocumentServiceHelpers.BuildListResponse(
            items.Select(MapListItem).ToList(),
            1,
            take,
            items.Count));
    }

    public async Task<Result<DocumentListResponse>> GetExpiredAsync(
        int take,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DocumentListResponse>(userResult.Error!);
        }

        take = Math.Clamp(take, 1, 100);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var items = await _documentRepository.ListExpiredForUserAsync(
            userResult.Value,
            today,
            take,
            cancellationToken);

        return Result.Success(DocumentServiceHelpers.BuildListResponse(
            items.Select(MapListItem).ToList(),
            1,
            take,
            items.Count));
    }

    private DocumentListItemResponse MapListItem(Document document)
    {
        var item = _mapper.Map<DocumentListItemResponse>(document);
        item.Tags = document.Tags
            .OrderBy(tag => tag.Name)
            .Select(tag => tag.Name)
            .ToList();
        return item;
    }

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }
}

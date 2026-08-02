using AutoMapper;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Documents.DTOs.Requests;
using WealthOS.Application.Documents.DTOs.Responses;
using WealthOS.Application.Documents.Interfaces;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Documents.Entities;
using WealthOS.Domain.Documents.Enums;
using WealthOS.Domain.Documents.Repositories;

namespace WealthOS.Application.Documents.Services;

/// <summary>
/// Orchestrates document CRUD, tags, reminders, and metadata placeholder updates.
/// </summary>
public sealed class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public DocumentService(
        IDocumentRepository documentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<DocumentResponse>> CreateAsync(
        CreateDocumentRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DocumentResponse>(userResult.Error!);
        }

        var linkValidation = DocumentServiceHelpers.ValidatePrimaryReference(
            request.ReferenceModule,
            request.ReferenceId);
        if (linkValidation.IsFailure)
        {
            return Result.Failure<DocumentResponse>(linkValidation.Error!);
        }

        var document = _mapper.Map<Document>(request);
        document.UserId = userResult.Value;
        document.Status = DocumentServiceHelpers.ResolveStatus(document.Status, document.ExpiryDate);
        ApplyCreateChildren(document, request);

        await _documentRepository.AddAsync(document, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _documentRepository.GetByIdWithDetailsAsync(
            document.Id,
            userResult.Value,
            cancellationToken);

        return Result.Success(MapDetail(created!));
    }

    public async Task<Result<DocumentResponse>> UpdateAsync(
        Guid documentId,
        UpdateDocumentRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DocumentResponse>(userResult.Error!);
        }

        var document = await _documentRepository.GetByIdWithDetailsAsync(
            documentId,
            userResult.Value,
            cancellationToken);

        if (document is null)
        {
            return Result.Failure<DocumentResponse>(Error.NotFound(nameof(Document), documentId));
        }

        var linkValidation = DocumentServiceHelpers.ValidatePrimaryReference(
            request.ReferenceModule,
            request.ReferenceId);
        if (linkValidation.IsFailure)
        {
            return Result.Failure<DocumentResponse>(linkValidation.Error!);
        }

        ApplyUpdate(document, request);
        _documentRepository.Update(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _documentRepository.GetByIdWithDetailsAsync(
            documentId,
            userResult.Value,
            cancellationToken);

        return Result.Success(MapDetail(updated!));
    }

    public async Task<Result> DeleteAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error!);
        }

        var document = await _documentRepository.GetByIdForUserAsync(
            documentId,
            userResult.Value,
            cancellationToken);

        if (document is null)
        {
            return Result.Failure(Error.NotFound(nameof(Document), documentId));
        }

        _documentRepository.Remove(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<DocumentResponse>> UploadMetadataAsync(
        Guid documentId,
        UploadDocumentMetadataRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DocumentResponse>(userResult.Error!);
        }

        var document = await _documentRepository.GetByIdWithDetailsAsync(
            documentId,
            userResult.Value,
            cancellationToken);

        if (document is null)
        {
            return Result.Failure<DocumentResponse>(Error.NotFound(nameof(Document), documentId));
        }

        ApplyUploadMetadata(document, userResult.Value, request);
        _documentRepository.Update(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _documentRepository.GetByIdWithDetailsAsync(
            documentId,
            userResult.Value,
            cancellationToken);

        return Result.Success(MapDetail(updated!));
    }

    public async Task<Result<DocumentTagResponse>> AddTagAsync(
        Guid documentId,
        AddDocumentTagRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DocumentTagResponse>(userResult.Error!);
        }

        var document = await _documentRepository.GetByIdWithDetailsAsync(
            documentId,
            userResult.Value,
            cancellationToken);

        if (document is null)
        {
            return Result.Failure<DocumentTagResponse>(Error.NotFound(nameof(Document), documentId));
        }

        var name = DocumentServiceHelpers.NormalizeTag(request.Name);
        if (document.Tags.Any(tag => string.Equals(tag.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            return Result.Failure<DocumentTagResponse>(
                Error.Conflict($"Tag '{name}' already exists on this document."));
        }

        var tag = new DocumentTag { Name = name };
        document.Tags.Add(tag);
        _documentRepository.Update(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<DocumentTagResponse>(tag));
    }

    public async Task<Result> RemoveTagAsync(
        Guid documentId,
        Guid tagId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error!);
        }

        var document = await _documentRepository.GetByIdWithDetailsAsync(
            documentId,
            userResult.Value,
            cancellationToken);

        if (document is null)
        {
            return Result.Failure(Error.NotFound(nameof(Document), documentId));
        }

        var tag = document.Tags.FirstOrDefault(item => item.Id == tagId);
        if (tag is null)
        {
            return Result.Failure(Error.NotFound(nameof(DocumentTag), tagId));
        }

        document.Tags.Remove(tag);
        _documentRepository.Update(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<DocumentReminderResponse>> CreateReminderAsync(
        Guid documentId,
        CreateDocumentReminderRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DocumentReminderResponse>(userResult.Error!);
        }

        var document = await _documentRepository.GetByIdWithDetailsAsync(
            documentId,
            userResult.Value,
            cancellationToken);

        if (document is null)
        {
            return Result.Failure<DocumentReminderResponse>(Error.NotFound(nameof(Document), documentId));
        }

        var reminder = new DocumentReminder
        {
            ReminderDate = request.ReminderDate,
            Message = request.Message.Trim(),
            Notes = request.Notes,
        };

        document.Reminders.Add(reminder);
        if (!document.ReminderDate.HasValue || request.ReminderDate < document.ReminderDate.Value)
        {
            document.ReminderDate = request.ReminderDate;
        }

        _documentRepository.Update(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<DocumentReminderResponse>(reminder));
    }

    public async Task<Result<DocumentResponse>> GetByIdAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DocumentResponse>(userResult.Error!);
        }

        var document = await _documentRepository.GetByIdWithDetailsAsync(
            documentId,
            userResult.Value,
            cancellationToken);

        if (document is null)
        {
            return Result.Failure<DocumentResponse>(Error.NotFound(nameof(Document), documentId));
        }

        return Result.Success(MapDetail(document));
    }

    public async Task<Result<DocumentListResponse>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        DocumentCategory? category,
        DocumentStatus? status,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DocumentListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, totalCount) = await _documentRepository.ListForUserAsync(
            userResult.Value,
            page,
            pageSize,
            search,
            category,
            status,
            cancellationToken);

        return Result.Success(DocumentServiceHelpers.BuildListResponse(
            items.Select(MapListItem).ToList(),
            page,
            pageSize,
            totalCount));
    }

    private static void ApplyCreateChildren(Document document, CreateDocumentRequest request)
    {
        if (request.Tags is { Count: > 0 })
        {
            foreach (var tagName in request.Tags
                         .Select(DocumentServiceHelpers.NormalizeTag)
                         .Where(name => !string.IsNullOrWhiteSpace(name))
                         .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                document.Tags.Add(new DocumentTag { Name = tagName });
            }
        }

        if (request.Links is { Count: > 0 })
        {
            foreach (var link in request.Links)
            {
                if (link.ReferenceModule == DocumentReferenceModule.None || link.ReferenceId == Guid.Empty)
                {
                    continue;
                }

                document.Links.Add(new DocumentLink
                {
                    ReferenceModule = link.ReferenceModule,
                    ReferenceId = link.ReferenceId,
                    Notes = link.Notes,
                });
            }
        }

        if (request.Metadata is not null)
        {
            document.Metadata = DocumentServiceHelpers.MapMetadata(request.Metadata);
        }

        if (!string.IsNullOrWhiteSpace(document.OriginalFileName)
            || document.FileSizeBytes > 0
            || !string.IsNullOrWhiteSpace(document.StoragePath))
        {
            document.Versions.Add(DocumentServiceHelpers.CreateVersion(document, 1, request.Notes));
        }
    }

    private static void ApplyUpdate(Document document, UpdateDocumentRequest request)
    {
        document.Title = request.Title.Trim();
        document.Description = request.Description;
        document.Category = request.Category;
        document.Owner = request.Owner.Trim();
        document.IssueDate = request.IssueDate;
        document.ExpiryDate = request.ExpiryDate;
        document.ReminderDate = request.ReminderDate;
        document.Status = DocumentServiceHelpers.ResolveStatus(request.Status, request.ExpiryDate);
        document.AccessLevel = request.AccessLevel;
        document.ReferenceModule = request.ReferenceModule;
        document.ReferenceId = request.ReferenceId;
        document.Notes = request.Notes;
    }

    private static void ApplyUploadMetadata(
        Document document,
        Guid userId,
        UploadDocumentMetadataRequest request)
    {
        document.OriginalFileName = request.OriginalFileName.Trim();
        document.ContentType = request.ContentType.Trim();
        document.FileSizeBytes = request.FileSizeBytes;
        document.StorageProvider = request.StorageProvider;
        document.StoragePath = string.IsNullOrWhiteSpace(request.StoragePath)
            ? DocumentServiceHelpers.BuildPlaceholderPath(userId, document.Id, request.OriginalFileName)
            : request.StoragePath.Trim();

        if (request.Metadata is not null)
        {
            if (document.Metadata is null)
            {
                document.Metadata = DocumentServiceHelpers.MapMetadata(request.Metadata);
            }
            else
            {
                DocumentServiceHelpers.ApplyMetadata(document.Metadata, request.Metadata);
            }
        }

        var nextVersion = document.Versions.Count == 0
            ? 1
            : document.Versions.Max(version => version.VersionNumber) + 1;

        document.Versions.Add(DocumentServiceHelpers.CreateVersion(document, nextVersion, request.VersionNotes));
    }

    private DocumentResponse MapDetail(Document document)
    {
        var response = _mapper.Map<DocumentResponse>(document);
        response.Tags = document.Tags
            .OrderBy(tag => tag.Name)
            .Select(tag => _mapper.Map<DocumentTagResponse>(tag))
            .ToList();
        response.Versions = document.Versions
            .OrderByDescending(version => version.VersionNumber)
            .Select(version => _mapper.Map<DocumentVersionResponse>(version))
            .ToList();
        response.Links = document.Links
            .Select(link => _mapper.Map<DocumentLinkResponse>(link))
            .ToList();
        response.Reminders = document.Reminders
            .OrderBy(reminder => reminder.ReminderDate)
            .Select(reminder => _mapper.Map<DocumentReminderResponse>(reminder))
            .ToList();
        response.Metadata = document.Metadata is null
            ? null
            : _mapper.Map<DocumentMetadataResponse>(document.Metadata);
        return response;
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

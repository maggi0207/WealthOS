using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Documents.Commands;
using WealthOS.Application.Documents.DTOs.Requests;
using WealthOS.Application.Documents.DTOs.Responses;
using WealthOS.Application.Documents.Queries;
using WealthOS.Domain.Documents.Enums;

namespace WealthOS.Api.Controllers;

/// <summary>
/// Document vault endpoints (CRUD, search, tags, reminders, metadata placeholders).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/documents")]
public sealed class DocumentController : ControllerBase
{
    private readonly ICommandHandler<CreateDocumentCommand, DocumentResponse> _createHandler;
    private readonly ICommandHandler<UpdateDocumentCommand, DocumentResponse> _updateHandler;
    private readonly ICommandHandler<DeleteDocumentCommand> _deleteHandler;
    private readonly ICommandHandler<UploadDocumentMetadataCommand, DocumentResponse> _uploadMetadataHandler;
    private readonly ICommandHandler<AddDocumentTagCommand, DocumentTagResponse> _addTagHandler;
    private readonly ICommandHandler<RemoveDocumentTagCommand> _removeTagHandler;
    private readonly ICommandHandler<CreateDocumentReminderCommand, DocumentReminderResponse> _createReminderHandler;
    private readonly IQueryHandler<GetDocumentsQuery, DocumentListResponse> _getAllHandler;
    private readonly IQueryHandler<GetDocumentByIdQuery, DocumentResponse> _getByIdHandler;
    private readonly IQueryHandler<SearchDocumentsQuery, DocumentListResponse> _searchHandler;
    private readonly IQueryHandler<GetRecentDocumentsQuery, DocumentListResponse> _recentHandler;
    private readonly IQueryHandler<GetExpiredDocumentsQuery, DocumentListResponse> _expiredHandler;

    /// <summary>
    /// Creates a new <see cref="DocumentController"/>.
    /// </summary>
    public DocumentController(
        ICommandHandler<CreateDocumentCommand, DocumentResponse> createHandler,
        ICommandHandler<UpdateDocumentCommand, DocumentResponse> updateHandler,
        ICommandHandler<DeleteDocumentCommand> deleteHandler,
        ICommandHandler<UploadDocumentMetadataCommand, DocumentResponse> uploadMetadataHandler,
        ICommandHandler<AddDocumentTagCommand, DocumentTagResponse> addTagHandler,
        ICommandHandler<RemoveDocumentTagCommand> removeTagHandler,
        ICommandHandler<CreateDocumentReminderCommand, DocumentReminderResponse> createReminderHandler,
        IQueryHandler<GetDocumentsQuery, DocumentListResponse> getAllHandler,
        IQueryHandler<GetDocumentByIdQuery, DocumentResponse> getByIdHandler,
        IQueryHandler<SearchDocumentsQuery, DocumentListResponse> searchHandler,
        IQueryHandler<GetRecentDocumentsQuery, DocumentListResponse> recentHandler,
        IQueryHandler<GetExpiredDocumentsQuery, DocumentListResponse> expiredHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _uploadMetadataHandler = uploadMetadataHandler;
        _addTagHandler = addTagHandler;
        _removeTagHandler = removeTagHandler;
        _createReminderHandler = createReminderHandler;
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _searchHandler = searchHandler;
        _recentHandler = recentHandler;
        _expiredHandler = expiredHandler;
    }

    /// <summary>
    /// Returns a paginated list of documents for the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<DocumentListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] DocumentCategory? category = null,
        [FromQuery] DocumentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _getAllHandler.HandleAsync(
            new GetDocumentsQuery
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                Category = category,
                Status = status,
            },
            cancellationToken);

        return ToActionResult(result, "Documents retrieved.");
    }

    /// <summary>
    /// Searches documents by title, category, tags, owner, and references.
    /// Registered before <c>{id}</c> to avoid route conflicts.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<DocumentListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Search(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? title = null,
        [FromQuery] DocumentCategory? category = null,
        [FromQuery] string? tag = null,
        [FromQuery] string? owner = null,
        [FromQuery] DocumentReferenceModule? referenceModule = null,
        [FromQuery] Guid? referenceId = null,
        [FromQuery] DocumentStatus? status = null,
        [FromQuery] string? freeText = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _searchHandler.HandleAsync(
            new SearchDocumentsQuery
            {
                Page = page,
                PageSize = pageSize,
                Title = title,
                Category = category,
                Tag = tag,
                Owner = owner,
                ReferenceModule = referenceModule,
                ReferenceId = referenceId,
                Status = status,
                FreeText = freeText,
            },
            cancellationToken);

        return ToActionResult(result, "Document search completed.");
    }

    /// <summary>
    /// Returns recently updated documents.
    /// </summary>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(ApiResponse<DocumentListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRecent(
        [FromQuery] int take = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _recentHandler.HandleAsync(
            new GetRecentDocumentsQuery { Take = take },
            cancellationToken);

        return ToActionResult(result, "Recent documents retrieved.");
    }

    /// <summary>
    /// Returns expired documents.
    /// </summary>
    [HttpGet("expired")]
    [ProducesResponseType(typeof(ApiResponse<DocumentListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExpired(
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _expiredHandler.HandleAsync(
            new GetExpiredDocumentsQuery { Take = take },
            cancellationToken);

        return ToActionResult(result, "Expired documents retrieved.");
    }

    /// <summary>
    /// Returns a single document by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.HandleAsync(
            new GetDocumentByIdQuery { DocumentId = id },
            cancellationToken);

        return ToActionResult(result, "Document retrieved.");
    }

    /// <summary>
    /// Creates a new vault document (metadata + storage placeholders).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DocumentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreateDocumentCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<DocumentResponse>.Ok(result.Value, "Document created."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Updates an existing vault document.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateHandler.HandleAsync(
            new UpdateDocumentCommand { DocumentId = id, Request = request },
            cancellationToken);

        return ToActionResult(result, "Document updated.");
    }

    /// <summary>
    /// Soft-deletes a vault document.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _deleteHandler.HandleAsync(
            new DeleteDocumentCommand { DocumentId = id },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object>.Ok(null!, "Document deleted."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Updates storage metadata placeholders and appends a version record (no real file upload).
    /// </summary>
    [HttpPost("{id:guid}/metadata")]
    [ProducesResponseType(typeof(ApiResponse<DocumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UploadMetadata(
        Guid id,
        [FromBody] UploadDocumentMetadataRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _uploadMetadataHandler.HandleAsync(
            new UploadDocumentMetadataCommand { DocumentId = id, Request = request },
            cancellationToken);

        return ToActionResult(result, "Document metadata updated.");
    }

    /// <summary>
    /// Adds a tag to a document.
    /// </summary>
    [HttpPost("{id:guid}/tags")]
    [ProducesResponseType(typeof(ApiResponse<DocumentTagResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AddTag(
        Guid id,
        [FromBody] AddDocumentTagRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _addTagHandler.HandleAsync(
            new AddDocumentTagCommand { DocumentId = id, Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<DocumentTagResponse>.Ok(result.Value, "Tag added."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Removes a tag from a document.
    /// </summary>
    [HttpDelete("{id:guid}/tags/{tagId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTag(
        Guid id,
        Guid tagId,
        CancellationToken cancellationToken)
    {
        var result = await _removeTagHandler.HandleAsync(
            new RemoveDocumentTagCommand { DocumentId = id, TagId = tagId },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object>.Ok(null!, "Tag removed."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Creates a reminder for a document.
    /// </summary>
    [HttpPost("{id:guid}/reminders")]
    [ProducesResponseType(typeof(ApiResponse<DocumentReminderResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateReminder(
        Guid id,
        [FromBody] CreateDocumentReminderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createReminderHandler.HandleAsync(
            new CreateDocumentReminderCommand { DocumentId = id, Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<DocumentReminderResponse>.Ok(result.Value, "Reminder created."));
        }

        return ToFailureResult(result.Error!);
    }

    private IActionResult ToActionResult<T>(Result<T> result, string successMessage)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.Ok(result.Value, successMessage));
        }

        return ToFailureResult(result.Error!);
    }

    private IActionResult ToFailureResult(Error error)
    {
        var errors = new List<ApiErrorDetail>();

        if (error.ValidationErrors is not null)
        {
            foreach (var (field, messages) in error.ValidationErrors)
            {
                errors.AddRange(messages.Select(message => new ApiErrorDetail
                {
                    Code = error.Code,
                    Message = message,
                    Field = field,
                }));
            }
        }
        else
        {
            errors.Add(new ApiErrorDetail
            {
                Code = error.Code,
                Message = error.Message,
            });
        }

        var payload = ApiResponse<object>.Fail(error.Message, errors);

        return error.Code switch
        {
            "unauthorized" => Unauthorized(payload),
            "forbidden" => StatusCode(StatusCodes.Status403Forbidden, payload),
            "not_found" => NotFound(payload),
            "conflict" => Conflict(payload),
            "validation_error" => UnprocessableEntity(payload),
            _ => BadRequest(payload),
        };
    }
}

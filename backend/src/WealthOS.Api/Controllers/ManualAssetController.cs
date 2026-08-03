using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.Assets.Commands;
using WealthOS.Application.Assets.DTOs.Requests;
using WealthOS.Application.Assets.DTOs.Responses;
using WealthOS.Application.Assets.Queries;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Common.Models;
using WealthOS.Domain.Assets.Enums;

namespace WealthOS.Api.Controllers;

/// <summary>
/// Manual asset endpoints (CRUD). Derived assets live in Properties / Investments.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/assets/manual")]
public sealed class ManualAssetController : ControllerBase
{
    private readonly ICommandHandler<CreateManualAssetCommand, ManualAssetResponse> _createHandler;
    private readonly ICommandHandler<UpdateManualAssetCommand, ManualAssetResponse> _updateHandler;
    private readonly ICommandHandler<DeleteManualAssetCommand> _deleteHandler;
    private readonly IQueryHandler<GetManualAssetsQuery, ManualAssetListResponse> _getAllHandler;
    private readonly IQueryHandler<GetManualAssetByIdQuery, ManualAssetResponse> _getByIdHandler;

    public ManualAssetController(
        ICommandHandler<CreateManualAssetCommand, ManualAssetResponse> createHandler,
        ICommandHandler<UpdateManualAssetCommand, ManualAssetResponse> updateHandler,
        ICommandHandler<DeleteManualAssetCommand> deleteHandler,
        IQueryHandler<GetManualAssetsQuery, ManualAssetListResponse> getAllHandler,
        IQueryHandler<GetManualAssetByIdQuery, ManualAssetResponse> getByIdHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
    }

    /// <summary>
    /// Returns a paginated list of manual assets for the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<ManualAssetListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? search = null,
        [FromQuery] ManualAssetType? type = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _getAllHandler.HandleAsync(
            new GetManualAssetsQuery
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                Type = type,
            },
            cancellationToken);

        return ToActionResult(result, "Manual assets retrieved.");
    }

    /// <summary>
    /// Returns a single manual asset by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ManualAssetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.HandleAsync(
            new GetManualAssetByIdQuery { AssetId = id },
            cancellationToken);

        return ToActionResult(result, "Manual asset retrieved.");
    }

    /// <summary>
    /// Creates a new manual asset.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ManualAssetResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateManualAssetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreateManualAssetCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<ManualAssetResponse>.Ok(result.Value, "Manual asset created."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Updates an existing manual asset.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ManualAssetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateManualAssetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateHandler.HandleAsync(
            new UpdateManualAssetCommand { AssetId = id, Request = request },
            cancellationToken);

        return ToActionResult(result, "Manual asset updated.");
    }

    /// <summary>
    /// Soft-deletes a manual asset.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _deleteHandler.HandleAsync(
            new DeleteManualAssetCommand { AssetId = id },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object>.Ok(null!, "Manual asset deleted."));
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

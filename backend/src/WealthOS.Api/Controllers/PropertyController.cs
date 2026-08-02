using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Properties.Commands;
using WealthOS.Application.Properties.DTOs.Requests;
using WealthOS.Application.Properties.DTOs.Responses;
using WealthOS.Application.Properties.Queries;
using WealthOS.Domain.Properties.Enums;

namespace WealthOS.Api.Controllers;

/// <summary>
/// Property portfolio management endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/properties")]
public sealed class PropertyController : ControllerBase
{
    private readonly ICommandHandler<CreatePropertyCommand, PropertyResponse> _createHandler;
    private readonly ICommandHandler<UpdatePropertyCommand, PropertyResponse> _updateHandler;
    private readonly ICommandHandler<DeletePropertyCommand> _deleteHandler;
    private readonly IQueryHandler<GetPropertyByIdQuery, PropertyResponse> _getByIdHandler;
    private readonly IQueryHandler<GetAllPropertiesQuery, PropertyListResponse> _getAllHandler;
    private readonly IQueryHandler<GetPropertySummaryQuery, PropertySummaryResponse> _summaryHandler;
    private readonly IQueryHandler<GetPropertyDashboardQuery, PropertyDashboardResponse> _dashboardHandler;

    /// <summary>
    /// Creates a new <see cref="PropertyController"/>.
    /// </summary>
    public PropertyController(
        ICommandHandler<CreatePropertyCommand, PropertyResponse> createHandler,
        ICommandHandler<UpdatePropertyCommand, PropertyResponse> updateHandler,
        ICommandHandler<DeletePropertyCommand> deleteHandler,
        IQueryHandler<GetPropertyByIdQuery, PropertyResponse> getByIdHandler,
        IQueryHandler<GetAllPropertiesQuery, PropertyListResponse> getAllHandler,
        IQueryHandler<GetPropertySummaryQuery, PropertySummaryResponse> summaryHandler,
        IQueryHandler<GetPropertyDashboardQuery, PropertyDashboardResponse> dashboardHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getByIdHandler = getByIdHandler;
        _getAllHandler = getAllHandler;
        _summaryHandler = summaryHandler;
        _dashboardHandler = dashboardHandler;
    }

    /// <summary>
    /// Returns a paginated list of properties for the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PropertyListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] PropertyStatus? status = null,
        [FromQuery] PropertyType? type = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _getAllHandler.HandleAsync(
            new GetAllPropertiesQuery
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                Status = status,
                Type = type,
            },
            cancellationToken);

        return ToActionResult(result, "Properties retrieved.");
    }

    /// <summary>
    /// Returns portfolio-level property summary totals.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<PropertySummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await _summaryHandler.HandleAsync(new GetPropertySummaryQuery(), cancellationToken);
        return ToActionResult(result, "Property summary retrieved.");
    }

    /// <summary>
    /// Returns a single property by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.HandleAsync(
            new GetPropertyByIdQuery { PropertyId = id },
            cancellationToken);

        return ToActionResult(result, "Property retrieved.");
    }

    /// <summary>
    /// Returns a per-property dashboard snapshot (detail, equity, and related stub counts).
    /// </summary>
    [HttpGet("{id:guid}/dashboard")]
    [ProducesResponseType(typeof(ApiResponse<PropertyDashboardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDashboard(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dashboardHandler.HandleAsync(
            new GetPropertyDashboardQuery { PropertyId = id },
            cancellationToken);

        return ToActionResult(result, "Property dashboard retrieved.");
    }

    /// <summary>
    /// Creates a new property for the authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreatePropertyCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value.Id, version = "1.0" },
                ApiResponse<PropertyResponse>.Ok(result.Value, "Property created."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Updates an existing property owned by the authenticated user.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateHandler.HandleAsync(
            new UpdatePropertyCommand { PropertyId = id, Request = request },
            cancellationToken);

        return ToActionResult(result, "Property updated.");
    }

    /// <summary>
    /// Soft-deletes a property owned by the authenticated user.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _deleteHandler.HandleAsync(
            new DeletePropertyCommand { PropertyId = id },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object?>.Ok(null, "Property deleted."));
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

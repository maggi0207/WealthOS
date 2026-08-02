using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Notifications.Commands;
using WealthOS.Application.Notifications.DTOs.Requests;
using WealthOS.Application.Notifications.DTOs.Responses;
using WealthOS.Application.Notifications.Queries;
using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.Api.Controllers;

/// <summary>
/// Cross-cutting reminder endpoints (separate resource from notifications).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/reminders")]
public sealed class ReminderController : ControllerBase
{
    private readonly ICommandHandler<CreateReminderCommand, ReminderResponse> _createHandler;
    private readonly IQueryHandler<GetRemindersQuery, ReminderListResponse> _getAllHandler;

    /// <summary>
    /// Creates a new <see cref="ReminderController"/>.
    /// </summary>
    public ReminderController(
        ICommandHandler<CreateReminderCommand, ReminderResponse> createHandler,
        IQueryHandler<GetRemindersQuery, ReminderListResponse> getAllHandler)
    {
        _createHandler = createHandler;
        _getAllHandler = getAllHandler;
    }

    /// <summary>
    /// Returns a paginated list of reminders for the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<ReminderListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] ReminderStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _getAllHandler.HandleAsync(
            new GetRemindersQuery
            {
                Page = page,
                PageSize = pageSize,
                Status = status,
            },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<ReminderListResponse>.Ok(result.Value, "Reminders retrieved."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Creates a reminder for the authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReminderResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateReminderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreateReminderCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<ReminderResponse>.Ok(result.Value, "Reminder created."));
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

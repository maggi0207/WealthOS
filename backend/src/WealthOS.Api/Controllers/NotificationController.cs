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
/// Notification inbox, reminders, and preference endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/notifications")]
public sealed class NotificationController : ControllerBase
{
    private readonly ICommandHandler<CreateNotificationCommand, NotificationResponse> _createHandler;
    private readonly ICommandHandler<MarkNotificationAsReadCommand> _markReadHandler;
    private readonly ICommandHandler<DeleteNotificationCommand> _deleteHandler;
    private readonly ICommandHandler<CreateReminderCommand, ReminderResponse> _createReminderHandler;
    private readonly ICommandHandler<UpdateNotificationPreferencesCommand, NotificationPreferenceListResponse>
        _updatePreferencesHandler;
    private readonly IQueryHandler<GetNotificationsQuery, NotificationListResponse> _getAllHandler;
    private readonly IQueryHandler<GetUnreadNotificationsQuery, NotificationListResponse> _getUnreadHandler;
    private readonly IQueryHandler<GetNotificationSummaryQuery, NotificationSummaryResponse> _summaryHandler;
    private readonly IQueryHandler<GetRemindersQuery, ReminderListResponse> _getRemindersHandler;
    private readonly IQueryHandler<GetNotificationPreferencesQuery, NotificationPreferenceListResponse>
        _getPreferencesHandler;

    /// <summary>
    /// Creates a new <see cref="NotificationController"/>.
    /// </summary>
    public NotificationController(
        ICommandHandler<CreateNotificationCommand, NotificationResponse> createHandler,
        ICommandHandler<MarkNotificationAsReadCommand> markReadHandler,
        ICommandHandler<DeleteNotificationCommand> deleteHandler,
        ICommandHandler<CreateReminderCommand, ReminderResponse> createReminderHandler,
        ICommandHandler<UpdateNotificationPreferencesCommand, NotificationPreferenceListResponse>
            updatePreferencesHandler,
        IQueryHandler<GetNotificationsQuery, NotificationListResponse> getAllHandler,
        IQueryHandler<GetUnreadNotificationsQuery, NotificationListResponse> getUnreadHandler,
        IQueryHandler<GetNotificationSummaryQuery, NotificationSummaryResponse> summaryHandler,
        IQueryHandler<GetRemindersQuery, ReminderListResponse> getRemindersHandler,
        IQueryHandler<GetNotificationPreferencesQuery, NotificationPreferenceListResponse>
            getPreferencesHandler)
    {
        _createHandler = createHandler;
        _markReadHandler = markReadHandler;
        _deleteHandler = deleteHandler;
        _createReminderHandler = createReminderHandler;
        _updatePreferencesHandler = updatePreferencesHandler;
        _getAllHandler = getAllHandler;
        _getUnreadHandler = getUnreadHandler;
        _summaryHandler = summaryHandler;
        _getRemindersHandler = getRemindersHandler;
        _getPreferencesHandler = getPreferencesHandler;
    }

    /// <summary>
    /// Returns a paginated list of notifications for the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<NotificationListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] NotificationType? type = null,
        [FromQuery] NotificationStatus? status = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _getAllHandler.HandleAsync(
            new GetNotificationsQuery
            {
                Page = page,
                PageSize = pageSize,
                Type = type,
                Status = status,
                Search = search,
            },
            cancellationToken);

        return ToActionResult(result, "Notifications retrieved.");
    }

    /// <summary>
    /// Returns unread notifications. Registered before <c>{id}</c> routes.
    /// </summary>
    [HttpGet("unread")]
    [ProducesResponseType(typeof(ApiResponse<NotificationListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUnread(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _getUnreadHandler.HandleAsync(
            new GetUnreadNotificationsQuery { Page = page, PageSize = pageSize },
            cancellationToken);

        return ToActionResult(result, "Unread notifications retrieved.");
    }

    /// <summary>
    /// Returns inbox summary counts. Registered before <c>{id}</c> routes.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<NotificationSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken = default)
    {
        var result = await _summaryHandler.HandleAsync(
            new GetNotificationSummaryQuery(),
            cancellationToken);

        return ToActionResult(result, "Notification summary retrieved.");
    }

    /// <summary>
    /// Returns notification channel preferences.
    /// </summary>
    [HttpGet("preferences")]
    [ProducesResponseType(typeof(ApiResponse<NotificationPreferenceListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPreferences(CancellationToken cancellationToken = default)
    {
        var result = await _getPreferencesHandler.HandleAsync(
            new GetNotificationPreferencesQuery(),
            cancellationToken);

        return ToActionResult(result, "Notification preferences retrieved.");
    }

    /// <summary>
    /// Upserts notification channel preferences.
    /// </summary>
    [HttpPut("preferences")]
    [ProducesResponseType(typeof(ApiResponse<NotificationPreferenceListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdatePreferences(
        [FromBody] UpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updatePreferencesHandler.HandleAsync(
            new UpdateNotificationPreferencesCommand { Request = request },
            cancellationToken);

        return ToActionResult(result, "Notification preferences updated.");
    }

    /// <summary>
    /// Creates an in-app notification for the authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NotificationResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateNotificationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreateNotificationCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<NotificationResponse>.Ok(result.Value, "Notification created."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Marks a notification as read.
    /// </summary>
    [HttpPut("{id:guid}/read")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        var result = await _markReadHandler.HandleAsync(
            new MarkNotificationAsReadCommand { NotificationId = id },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object>.Ok(null!, "Notification marked as read."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Soft-deletes a notification.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _deleteHandler.HandleAsync(
            new DeleteNotificationCommand { NotificationId = id },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object>.Ok(null!, "Notification deleted."));
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

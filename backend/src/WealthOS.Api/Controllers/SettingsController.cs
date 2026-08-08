using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Settings.Commands;
using WealthOS.Application.Settings.DTOs.Requests;
using WealthOS.Application.Settings.DTOs.Responses;
using WealthOS.Application.Settings.Queries;

namespace WealthOS.Api.Controllers;

/// <summary>
/// Authenticated user settings (profile, preferences, notifications, security, import/export).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/settings")]
public sealed class SettingsController : ControllerBase
{
    private readonly IQueryHandler<GetUserSettingsQuery, UserSettingsResponse> _getHandler;
    private readonly ICommandHandler<UpdateUserSettingsCommand, UserSettingsResponse> _updateHandler;
    private readonly ICommandHandler<UpdateProfileSettingsCommand, UserSettingsResponse> _profileHandler;
    private readonly ICommandHandler<UpdatePreferencesSettingsCommand, UserSettingsResponse> _preferencesHandler;
    private readonly ICommandHandler<UpdateNotificationSettingsCommand, UserSettingsResponse> _notificationsHandler;
    private readonly ICommandHandler<UpdateSecuritySettingsCommand, UserSettingsResponse> _securityHandler;
    private readonly ICommandHandler<ExportSettingsCommand, SettingsExportResponse> _exportHandler;
    private readonly ICommandHandler<ImportSettingsCommand, UserSettingsResponse> _importHandler;
    private readonly ICommandHandler<ClearSettingsCacheCommand> _clearCacheHandler;
    private readonly ICommandHandler<DeleteAccountCommand> _deleteAccountHandler;

    public SettingsController(
        IQueryHandler<GetUserSettingsQuery, UserSettingsResponse> getHandler,
        ICommandHandler<UpdateUserSettingsCommand, UserSettingsResponse> updateHandler,
        ICommandHandler<UpdateProfileSettingsCommand, UserSettingsResponse> profileHandler,
        ICommandHandler<UpdatePreferencesSettingsCommand, UserSettingsResponse> preferencesHandler,
        ICommandHandler<UpdateNotificationSettingsCommand, UserSettingsResponse> notificationsHandler,
        ICommandHandler<UpdateSecuritySettingsCommand, UserSettingsResponse> securityHandler,
        ICommandHandler<ExportSettingsCommand, SettingsExportResponse> exportHandler,
        ICommandHandler<ImportSettingsCommand, UserSettingsResponse> importHandler,
        ICommandHandler<ClearSettingsCacheCommand> clearCacheHandler,
        ICommandHandler<DeleteAccountCommand> deleteAccountHandler)
    {
        _getHandler = getHandler;
        _updateHandler = updateHandler;
        _profileHandler = profileHandler;
        _preferencesHandler = preferencesHandler;
        _notificationsHandler = notificationsHandler;
        _securityHandler = securityHandler;
        _exportHandler = exportHandler;
        _importHandler = importHandler;
        _clearCacheHandler = clearCacheHandler;
        _deleteAccountHandler = deleteAccountHandler;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<UserSettingsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _getHandler.HandleAsync(new GetUserSettingsQuery(), cancellationToken);
        return ToActionResult(result, "Settings retrieved.");
    }

    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<UserSettingsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        [FromBody] UpdateSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateHandler.HandleAsync(
            new UpdateUserSettingsCommand { Request = request },
            cancellationToken);
        return ToActionResult(result, "Settings updated.");
    }

    [HttpPut("profile")]
    [ProducesResponseType(typeof(ApiResponse<UserSettingsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _profileHandler.HandleAsync(
            new UpdateProfileSettingsCommand { Request = request },
            cancellationToken);
        return ToActionResult(result, "Profile updated.");
    }

    [HttpPut("preferences")]
    [ProducesResponseType(typeof(ApiResponse<UserSettingsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePreferences(
        [FromBody] UpdatePreferencesSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _preferencesHandler.HandleAsync(
            new UpdatePreferencesSettingsCommand { Request = request },
            cancellationToken);
        return ToActionResult(result, "Preferences updated.");
    }

    [HttpPut("notifications")]
    [ProducesResponseType(typeof(ApiResponse<UserSettingsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateNotifications(
        [FromBody] UpdateNotificationSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _notificationsHandler.HandleAsync(
            new UpdateNotificationSettingsCommand { Request = request },
            cancellationToken);
        return ToActionResult(result, "Notification preferences updated.");
    }

    [HttpPut("security")]
    [ProducesResponseType(typeof(ApiResponse<UserSettingsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSecurity(
        [FromBody] UpdateSecuritySettingsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _securityHandler.HandleAsync(
            new UpdateSecuritySettingsCommand { Request = request },
            cancellationToken);
        return ToActionResult(result, "Security settings updated.");
    }

    [HttpPost("export")]
    [ProducesResponseType(typeof(ApiResponse<SettingsExportResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Export(
        [FromBody] ExportSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _exportHandler.HandleAsync(
            new ExportSettingsCommand { Request = request },
            cancellationToken);
        return ToActionResult(result, "Export generated.");
    }

    [HttpPost("import")]
    [ProducesResponseType(typeof(ApiResponse<UserSettingsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Import(
        [FromBody] ImportSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _importHandler.HandleAsync(
            new ImportSettingsCommand { Request = request },
            cancellationToken);
        return ToActionResult(result, "Import completed.");
    }

    [HttpPost("clear-cache")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ClearCache(CancellationToken cancellationToken)
    {
        var result = await _clearCacheHandler.HandleAsync(new ClearSettingsCacheCommand(), cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object>.Ok(null!, "Local cache clear acknowledged."));
        }

        return ToFailureResult(result.Error!);
    }

    [HttpDelete("account")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAccount(CancellationToken cancellationToken)
    {
        var result = await _deleteAccountHandler.HandleAsync(new DeleteAccountCommand(), cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object>.Ok(null!, "Account deleted."));
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

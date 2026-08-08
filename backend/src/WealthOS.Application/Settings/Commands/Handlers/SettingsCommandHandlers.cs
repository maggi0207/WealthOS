using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Settings.Commands;
using WealthOS.Application.Settings.DTOs.Responses;
using WealthOS.Application.Settings.Interfaces;
using WealthOS.Application.Settings.Queries;

namespace WealthOS.Application.Settings.Commands.Handlers;

public sealed class GetUserSettingsQueryHandler
    : IQueryHandler<GetUserSettingsQuery, UserSettingsResponse>
{
    private readonly IUserSettingsService _service;

    public GetUserSettingsQueryHandler(IUserSettingsService service) => _service = service;

    public Task<Result<UserSettingsResponse>> HandleAsync(
        GetUserSettingsQuery query,
        CancellationToken cancellationToken = default) =>
        _service.GetAsync(cancellationToken);
}

public sealed class UpdateUserSettingsCommandHandler
    : ICommandHandler<UpdateUserSettingsCommand, UserSettingsResponse>
{
    private readonly IUserSettingsService _service;

    public UpdateUserSettingsCommandHandler(IUserSettingsService service) => _service = service;

    public Task<Result<UserSettingsResponse>> HandleAsync(
        UpdateUserSettingsCommand command,
        CancellationToken cancellationToken = default) =>
        _service.UpdateAsync(command.Request, cancellationToken);
}

public sealed class UpdateProfileSettingsCommandHandler
    : ICommandHandler<UpdateProfileSettingsCommand, UserSettingsResponse>
{
    private readonly IUserSettingsService _service;

    public UpdateProfileSettingsCommandHandler(IUserSettingsService service) => _service = service;

    public Task<Result<UserSettingsResponse>> HandleAsync(
        UpdateProfileSettingsCommand command,
        CancellationToken cancellationToken = default) =>
        _service.UpdateProfileAsync(command.Request, cancellationToken);
}

public sealed class UpdatePreferencesSettingsCommandHandler
    : ICommandHandler<UpdatePreferencesSettingsCommand, UserSettingsResponse>
{
    private readonly IUserSettingsService _service;

    public UpdatePreferencesSettingsCommandHandler(IUserSettingsService service) => _service = service;

    public Task<Result<UserSettingsResponse>> HandleAsync(
        UpdatePreferencesSettingsCommand command,
        CancellationToken cancellationToken = default) =>
        _service.UpdatePreferencesAsync(command.Request, cancellationToken);
}

public sealed class UpdateNotificationSettingsCommandHandler
    : ICommandHandler<UpdateNotificationSettingsCommand, UserSettingsResponse>
{
    private readonly IUserSettingsService _service;

    public UpdateNotificationSettingsCommandHandler(IUserSettingsService service) => _service = service;

    public Task<Result<UserSettingsResponse>> HandleAsync(
        UpdateNotificationSettingsCommand command,
        CancellationToken cancellationToken = default) =>
        _service.UpdateNotificationsAsync(command.Request, cancellationToken);
}

public sealed class UpdateSecuritySettingsCommandHandler
    : ICommandHandler<UpdateSecuritySettingsCommand, UserSettingsResponse>
{
    private readonly IUserSettingsService _service;

    public UpdateSecuritySettingsCommandHandler(IUserSettingsService service) => _service = service;

    public Task<Result<UserSettingsResponse>> HandleAsync(
        UpdateSecuritySettingsCommand command,
        CancellationToken cancellationToken = default) =>
        _service.UpdateSecurityAsync(command.Request, cancellationToken);
}

public sealed class ExportSettingsCommandHandler
    : ICommandHandler<ExportSettingsCommand, SettingsExportResponse>
{
    private readonly IUserSettingsService _service;

    public ExportSettingsCommandHandler(IUserSettingsService service) => _service = service;

    public Task<Result<SettingsExportResponse>> HandleAsync(
        ExportSettingsCommand command,
        CancellationToken cancellationToken = default) =>
        _service.ExportAsync(command.Request, cancellationToken);
}

public sealed class ImportSettingsCommandHandler
    : ICommandHandler<ImportSettingsCommand, UserSettingsResponse>
{
    private readonly IUserSettingsService _service;

    public ImportSettingsCommandHandler(IUserSettingsService service) => _service = service;

    public Task<Result<UserSettingsResponse>> HandleAsync(
        ImportSettingsCommand command,
        CancellationToken cancellationToken = default) =>
        _service.ImportAsync(command.Request, cancellationToken);
}

public sealed class ClearSettingsCacheCommandHandler : ICommandHandler<ClearSettingsCacheCommand>
{
    private readonly IUserSettingsService _service;

    public ClearSettingsCacheCommandHandler(IUserSettingsService service) => _service = service;

    public Task<Result> HandleAsync(
        ClearSettingsCacheCommand command,
        CancellationToken cancellationToken = default) =>
        _service.ClearCacheAsync(cancellationToken);
}

public sealed class DeleteAccountCommandHandler : ICommandHandler<DeleteAccountCommand>
{
    private readonly IUserSettingsService _service;

    public DeleteAccountCommandHandler(IUserSettingsService service) => _service = service;

    public Task<Result> HandleAsync(
        DeleteAccountCommand command,
        CancellationToken cancellationToken = default) =>
        _service.DeleteAccountAsync(cancellationToken);
}

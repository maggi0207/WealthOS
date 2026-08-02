using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Notifications.Commands;
using WealthOS.Application.Notifications.DTOs.Responses;
using WealthOS.Application.Notifications.Interfaces;

namespace WealthOS.Application.Notifications.Commands.Handlers;

public sealed class CreateNotificationCommandHandler
    : ICommandHandler<CreateNotificationCommand, NotificationResponse>
{
    private readonly INotificationService _notificationService;

    public CreateNotificationCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task<Result<NotificationResponse>> HandleAsync(
        CreateNotificationCommand command,
        CancellationToken cancellationToken = default) =>
        _notificationService.CreateAsync(command.Request, cancellationToken);
}

public sealed class MarkNotificationAsReadCommandHandler
    : ICommandHandler<MarkNotificationAsReadCommand>
{
    private readonly INotificationService _notificationService;

    public MarkNotificationAsReadCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task<Result> HandleAsync(
        MarkNotificationAsReadCommand command,
        CancellationToken cancellationToken = default) =>
        _notificationService.MarkAsReadAsync(command.NotificationId, cancellationToken);
}

public sealed class DeleteNotificationCommandHandler
    : ICommandHandler<DeleteNotificationCommand>
{
    private readonly INotificationService _notificationService;

    public DeleteNotificationCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task<Result> HandleAsync(
        DeleteNotificationCommand command,
        CancellationToken cancellationToken = default) =>
        _notificationService.DeleteAsync(command.NotificationId, cancellationToken);
}

public sealed class CreateReminderCommandHandler
    : ICommandHandler<CreateReminderCommand, ReminderResponse>
{
    private readonly IReminderService _reminderService;

    public CreateReminderCommandHandler(IReminderService reminderService)
    {
        _reminderService = reminderService;
    }

    public Task<Result<ReminderResponse>> HandleAsync(
        CreateReminderCommand command,
        CancellationToken cancellationToken = default) =>
        _reminderService.CreateAsync(command.Request, cancellationToken);
}

public sealed class UpdateNotificationPreferencesCommandHandler
    : ICommandHandler<UpdateNotificationPreferencesCommand, NotificationPreferenceListResponse>
{
    private readonly INotificationService _notificationService;

    public UpdateNotificationPreferencesCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task<Result<NotificationPreferenceListResponse>> HandleAsync(
        UpdateNotificationPreferencesCommand command,
        CancellationToken cancellationToken = default) =>
        _notificationService.UpdatePreferencesAsync(command.Request, cancellationToken);
}

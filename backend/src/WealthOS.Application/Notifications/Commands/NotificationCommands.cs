using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Notifications.DTOs.Requests;

namespace WealthOS.Application.Notifications.Commands;

/// <summary>
/// Creates a notification for the authenticated user.
/// </summary>
public sealed class CreateNotificationCommand : ICommand
{
    public CreateNotificationRequest Request { get; init; } = null!;
}

/// <summary>
/// Marks a notification as read.
/// </summary>
public sealed class MarkNotificationAsReadCommand : ICommand
{
    public Guid NotificationId { get; init; }
}

/// <summary>
/// Soft-deletes a notification.
/// </summary>
public sealed class DeleteNotificationCommand : ICommand
{
    public Guid NotificationId { get; init; }
}

/// <summary>
/// Creates a cross-cutting reminder.
/// </summary>
public sealed class CreateReminderCommand : ICommand
{
    public CreateReminderRequest Request { get; init; } = null!;
}

/// <summary>
/// Upserts notification channel preferences.
/// </summary>
public sealed class UpdateNotificationPreferencesCommand : ICommand
{
    public UpdateNotificationPreferencesRequest Request { get; init; } = null!;
}

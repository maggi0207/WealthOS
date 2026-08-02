using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.Application.Notifications.DTOs.Requests;

/// <summary>
/// Creates an in-app notification for the authenticated user.
/// </summary>
public sealed class CreateNotificationRequest
{
    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; } = NotificationType.GeneralReminder;

    public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;

    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    public DateTime? ScheduledAt { get; set; }

    public string? ReferenceModule { get; set; }

    public Guid? ReferenceId { get; set; }

    public string? PayloadJson { get; set; }
}

/// <summary>
/// Creates a cross-cutting reminder for the authenticated user.
/// </summary>
public sealed class CreateReminderRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Message { get; set; }

    public NotificationType ReminderType { get; set; } = NotificationType.GeneralReminder;

    public DateTime DueAt { get; set; }

    public string? ReferenceModule { get; set; }

    public Guid? ReferenceId { get; set; }

    public string? RecurrenceRule { get; set; }
}

/// <summary>
/// Upserts channel preferences for a notification type.
/// </summary>
public sealed class UpdateNotificationPreferencesRequest
{
    public IReadOnlyList<NotificationPreferenceItemRequest> Preferences { get; set; } =
        Array.Empty<NotificationPreferenceItemRequest>();
}

/// <summary>
/// Single preference row for a notification type.
/// </summary>
public sealed class NotificationPreferenceItemRequest
{
    public NotificationType NotificationType { get; set; }

    public bool EnableInApp { get; set; } = true;

    public bool EnableEmail { get; set; }

    public bool EnableSms { get; set; }

    public bool EnablePush { get; set; }

    public bool EnableWhatsApp { get; set; }

    public int? QuietHoursStartMinutes { get; set; }

    public int? QuietHoursEndMinutes { get; set; }
}

using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.Application.Notifications.DTOs.Responses;

/// <summary>
/// Full notification detail response.
/// </summary>
public sealed class NotificationResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; }

    public NotificationChannel Channel { get; set; }

    public NotificationPriority Priority { get; set; }

    public NotificationStatus Status { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime? ScheduledAt { get; set; }

    public DateTime? SentAt { get; set; }

    public string? ReferenceModule { get; set; }

    public Guid? ReferenceId { get; set; }

    public string? PayloadJson { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Compact notification row for list endpoints.
/// </summary>
public sealed class NotificationListItemResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public NotificationType Type { get; set; }

    public NotificationChannel Channel { get; set; }

    public NotificationPriority Priority { get; set; }

    public NotificationStatus Status { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Paginated notification list.
/// </summary>
public sealed class NotificationListResponse
{
    public IReadOnlyList<NotificationListItemResponse> Items { get; set; } =
        Array.Empty<NotificationListItemResponse>();

    public int TotalCount { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }
}

/// <summary>
/// Inbox summary counts.
/// </summary>
public sealed class NotificationSummaryResponse
{
    public int TotalCount { get; set; }

    public int UnreadCount { get; set; }

    public int HighPriorityUnreadCount { get; set; }

    public int PendingReminderCount { get; set; }
}

/// <summary>
/// Reminder detail response.
/// </summary>
public sealed class ReminderResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Message { get; set; }

    public NotificationType ReminderType { get; set; }

    public ReminderStatus Status { get; set; }

    public DateTime DueAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? ReferenceModule { get; set; }

    public Guid? ReferenceId { get; set; }

    public string? RecurrenceRule { get; set; }

    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Paginated reminder list.
/// </summary>
public sealed class ReminderListResponse
{
    public IReadOnlyList<ReminderResponse> Items { get; set; } = Array.Empty<ReminderResponse>();

    public int TotalCount { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }
}

/// <summary>
/// Single preference row response.
/// </summary>
public sealed class NotificationPreferenceResponse
{
    public Guid Id { get; set; }

    public NotificationType NotificationType { get; set; }

    public bool EnableInApp { get; set; }

    public bool EnableEmail { get; set; }

    public bool EnableSms { get; set; }

    public bool EnablePush { get; set; }

    public bool EnableWhatsApp { get; set; }

    public int? QuietHoursStartMinutes { get; set; }

    public int? QuietHoursEndMinutes { get; set; }
}

/// <summary>
/// Collection of preference rows for the authenticated user.
/// </summary>
public sealed class NotificationPreferenceListResponse
{
    public IReadOnlyList<NotificationPreferenceResponse> Preferences { get; set; } =
        Array.Empty<NotificationPreferenceResponse>();
}

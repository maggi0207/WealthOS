using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.Domain.Notifications.Models;

/// <summary>
/// Filter criteria for listing notifications.
/// </summary>
public sealed class NotificationSearchCriteria
{
    public NotificationType? Type { get; init; }

    public NotificationStatus? Status { get; init; }

    public NotificationChannel? Channel { get; init; }

    public NotificationPriority? Priority { get; init; }

    public bool? UnreadOnly { get; init; }

    public string? Search { get; init; }
}

/// <summary>
/// Aggregated unread / priority counts for the notification inbox.
/// </summary>
public sealed class NotificationSummary
{
    public int TotalCount { get; init; }

    public int UnreadCount { get; init; }

    public int HighPriorityUnreadCount { get; init; }

    public int PendingReminderCount { get; init; }
}

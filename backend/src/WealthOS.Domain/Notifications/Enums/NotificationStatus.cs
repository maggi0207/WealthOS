namespace WealthOS.Domain.Notifications.Enums;

/// <summary>
/// Lifecycle status of a notification delivery attempt.
/// </summary>
public enum NotificationStatus
{
    Pending = 0,
    Scheduled = 1,
    Sent = 2,
    Read = 3,
    Failed = 4,
    Cancelled = 5,
}

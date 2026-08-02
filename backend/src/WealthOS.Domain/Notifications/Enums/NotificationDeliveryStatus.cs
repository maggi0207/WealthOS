namespace WealthOS.Domain.Notifications.Enums;

/// <summary>
/// Per-recipient channel delivery status.
/// </summary>
public enum NotificationDeliveryStatus
{
    Pending = 0,
    Queued = 1,
    Delivered = 2,
    Failed = 3,
    Skipped = 4,
}

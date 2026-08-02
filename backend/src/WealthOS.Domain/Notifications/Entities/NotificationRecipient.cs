using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.Domain.Notifications.Entities;

/// <summary>
/// Per-channel delivery target for a notification.
/// </summary>
public sealed class NotificationRecipient : AuditableEntity
{
    public NotificationRecipient()
    {
    }

    public NotificationRecipient(Guid id)
        : base(id)
    {
    }

    public Guid NotificationId { get; set; }

    public Guid UserId { get; set; }

    public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;

    /// <summary>Optional address (email/phone) — placeholder only.</summary>
    public string? Address { get; set; }

    public NotificationDeliveryStatus DeliveryStatus { get; set; } = NotificationDeliveryStatus.Pending;

    public DateTime? DeliveredAt { get; set; }

    public string? FailureReason { get; set; }

    public Notification? Notification { get; set; }
}

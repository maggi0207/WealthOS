using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.Domain.Notifications.Entities;

/// <summary>
/// Reusable message template for a notification type and channel.
/// </summary>
public sealed class NotificationTemplate : AuditableEntity
{
    public NotificationTemplate()
    {
    }

    public NotificationTemplate(Guid id)
        : base(id)
    {
    }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public NotificationType Type { get; set; }

    public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;

    public string SubjectTemplate { get; set; } = string.Empty;

    public string BodyTemplate { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

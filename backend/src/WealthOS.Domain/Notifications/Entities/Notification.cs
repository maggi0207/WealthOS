using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.Domain.Notifications.Entities;

/// <summary>
/// Aggregate root for an in-app (or multi-channel) notification.
/// Cross-module links are GUID-only soft references.
/// </summary>
public sealed class Notification : AuditableEntity
{
    public Notification()
    {
    }

    public Notification(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; } = NotificationType.GeneralReminder;

    public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;

    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    public DateTime? ReadAt { get; set; }

    public DateTime? ScheduledAt { get; set; }

    public DateTime? SentAt { get; set; }

    /// <summary>Optional soft reference module name (e.g. Loans, Documents).</summary>
    public string? ReferenceModule { get; set; }

    /// <summary>Optional soft reference entity id.</summary>
    public Guid? ReferenceId { get; set; }

    public Guid? TemplateId { get; set; }

    public string? PayloadJson { get; set; }

    public NotificationTemplate? Template { get; set; }

    public ICollection<NotificationRecipient> Recipients { get; set; } = new List<NotificationRecipient>();
}

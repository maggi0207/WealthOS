using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.Domain.Notifications.Entities;

/// <summary>
/// Framework stub for user-level scheduled notification rules.
/// Real scheduling business logic is not implemented yet.
/// </summary>
public sealed class NotificationSchedule : AuditableEntity
{
    public NotificationSchedule()
    {
    }

    public NotificationSchedule(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public NotificationType NotificationType { get; set; }

    public string? CronExpression { get; set; }

    public DateTime? NextRunAt { get; set; }

    public bool IsEnabled { get; set; } = true;

    public string? PayloadJson { get; set; }

    public string? Notes { get; set; }
}

using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.Domain.Notifications.Entities;

/// <summary>
/// Cross-cutting user reminder independent of module-specific reminder children.
/// </summary>
public sealed class Reminder : AuditableEntity
{
    public Reminder()
    {
    }

    public Reminder(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Message { get; set; }

    public NotificationType ReminderType { get; set; } = NotificationType.GeneralReminder;

    public ReminderStatus Status { get; set; } = ReminderStatus.Active;

    public DateTime DueAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? ReferenceModule { get; set; }

    public Guid? ReferenceId { get; set; }

    /// <summary>Optional recurrence rule placeholder (not evaluated yet).</summary>
    public string? RecurrenceRule { get; set; }
}

using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.Domain.Notifications.Entities;

/// <summary>
/// Per-user, per-type channel preference flags.
/// </summary>
public sealed class NotificationPreference : AuditableEntity
{
    public NotificationPreference()
    {
    }

    public NotificationPreference(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public NotificationType NotificationType { get; set; }

    public bool EnableInApp { get; set; } = true;

    /// <summary>Email channel placeholder — not wired to a provider.</summary>
    public bool EnableEmail { get; set; }

    /// <summary>SMS channel placeholder — not wired to a provider.</summary>
    public bool EnableSms { get; set; }

    /// <summary>Push channel placeholder — not wired to a provider.</summary>
    public bool EnablePush { get; set; }

    /// <summary>WhatsApp channel — future.</summary>
    public bool EnableWhatsApp { get; set; }

    /// <summary>Optional quiet-hours start (UTC time-of-day minutes from midnight).</summary>
    public int? QuietHoursStartMinutes { get; set; }

    /// <summary>Optional quiet-hours end (UTC time-of-day minutes from midnight).</summary>
    public int? QuietHoursEndMinutes { get; set; }
}

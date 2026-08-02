namespace WealthOS.Domain.Notifications.Enums;

/// <summary>
/// Delivery channels. Only In-App is implemented; others are placeholders.
/// </summary>
public enum NotificationChannel
{
    /// <summary>Persisted in-app notification (default, implemented).</summary>
    InApp = 0,

    /// <summary>Email delivery placeholder — not implemented.</summary>
    Email = 1,

    /// <summary>SMS delivery placeholder — not implemented.</summary>
    Sms = 2,

    /// <summary>Push notification placeholder — not implemented.</summary>
    Push = 3,

    /// <summary>WhatsApp delivery — future.</summary>
    WhatsApp = 4,
}

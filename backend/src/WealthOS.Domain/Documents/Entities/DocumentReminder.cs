using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Documents.Entities;

/// <summary>
/// Reminder scheduled against a document (e.g. renewal / expiry).
/// </summary>
public sealed class DocumentReminder : AuditableEntity
{
    public DocumentReminder()
    {
    }

    public DocumentReminder(Guid id)
        : base(id)
    {
    }

    public Guid DocumentId { get; set; }

    public Document Document { get; set; } = null!;

    public DateOnly ReminderDate { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool IsDismissed { get; set; }

    public string? Notes { get; set; }
}

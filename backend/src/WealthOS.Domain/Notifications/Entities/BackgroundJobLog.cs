using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.Domain.Notifications.Entities;

/// <summary>
/// Audit log entry for a Hangfire background job execution.
/// </summary>
public sealed class BackgroundJobLog : AuditableEntity
{
    public BackgroundJobLog()
    {
    }

    public BackgroundJobLog(Guid id)
        : base(id)
    {
    }

    public string JobName { get; set; } = string.Empty;

    public string? HangfireJobId { get; set; }

    public BackgroundJobStatus Status { get; set; } = BackgroundJobStatus.Started;

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int AttemptCount { get; set; } = 1;

    public string? Message { get; set; }

    public string? ErrorDetails { get; set; }
}

namespace WealthOS.Domain.Notifications.Enums;

/// <summary>
/// Execution status recorded for a Hangfire background job run.
/// </summary>
public enum BackgroundJobStatus
{
    Started = 0,
    Succeeded = 1,
    Failed = 2,
    Skipped = 3,
}

using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Notifications.Entities;
using WealthOS.Domain.Notifications.Enums;
using WealthOS.Domain.Notifications.Models;

namespace WealthOS.Domain.Notifications.Repositories;

/// <summary>
/// Persistence abstraction for the Notification aggregate.
/// </summary>
public interface INotificationRepository : IRepository<Notification>
{
    Task<Notification?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Notification> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        NotificationSearchCriteria criteria,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Notification> Items, int TotalCount)> ListUnreadForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<NotificationSummary> GetSummaryForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<int> CountUnreadForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Persistence abstraction for notification preferences.
/// </summary>
public interface INotificationPreferenceRepository : IRepository<NotificationPreference>
{
    Task<IReadOnlyList<NotificationPreference>> ListForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<NotificationPreference?> GetByUserAndTypeAsync(
        Guid userId,
        NotificationType type,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Persistence abstraction for cross-cutting reminders.
/// </summary>
public interface IReminderRepository : IRepository<Reminder>
{
    Task<Reminder?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Reminder> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        ReminderStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountActiveForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Persistence abstraction for notification templates.
/// </summary>
public interface INotificationTemplateRepository : IRepository<NotificationTemplate>
{
    Task<NotificationTemplate?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Persistence abstraction for background job execution logs.
/// </summary>
public interface IBackgroundJobLogRepository : IRepository<BackgroundJobLog>
{
    Task<(IReadOnlyList<BackgroundJobLog> Items, int TotalCount)> ListRecentAsync(
        int take,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Persistence abstraction for notification schedules (framework stub).
/// </summary>
public interface INotificationScheduleRepository : IRepository<NotificationSchedule>
{
    Task<IReadOnlyList<NotificationSchedule>> ListEnabledAsync(
        CancellationToken cancellationToken = default);
}

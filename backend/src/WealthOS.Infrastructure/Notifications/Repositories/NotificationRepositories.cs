using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Notifications.Entities;
using WealthOS.Domain.Notifications.Enums;
using WealthOS.Domain.Notifications.Models;
using WealthOS.Domain.Notifications.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Repositories;

namespace WealthOS.Infrastructure.Notifications.Repositories;

/// <summary>
/// EF Core repository for the Notification aggregate.
/// </summary>
public sealed class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Notification?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            notification => notification.Id == id && notification.UserId == userId,
            cancellationToken);

    public async Task<(IReadOnlyList<Notification> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        NotificationSearchCriteria criteria,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(notification => notification.UserId == userId);

        if (criteria.Type.HasValue)
        {
            query = query.Where(notification => notification.Type == criteria.Type.Value);
        }

        if (criteria.Status.HasValue)
        {
            query = query.Where(notification => notification.Status == criteria.Status.Value);
        }

        if (criteria.Channel.HasValue)
        {
            query = query.Where(notification => notification.Channel == criteria.Channel.Value);
        }

        if (criteria.Priority.HasValue)
        {
            query = query.Where(notification => notification.Priority == criteria.Priority.Value);
        }

        if (criteria.UnreadOnly == true)
        {
            query = query.Where(notification => notification.Status != NotificationStatus.Read);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Search))
        {
            var term = criteria.Search.Trim().ToLowerInvariant();
            query = query.Where(notification =>
                notification.Title.ToLower().Contains(term)
                || notification.Message.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(notification => notification.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<Notification> Items, int TotalCount)> ListUnreadForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default) =>
        await ListForUserAsync(
            userId,
            new NotificationSearchCriteria { UnreadOnly = true },
            page,
            pageSize,
            cancellationToken);

    public async Task<NotificationSummary> GetSummaryForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(notification => notification.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);
        var unreadCount = await query.CountAsync(
            notification => notification.Status != NotificationStatus.Read,
            cancellationToken);
        var highPriorityUnreadCount = await query.CountAsync(
            notification =>
                notification.Status != NotificationStatus.Read
                && (notification.Priority == NotificationPriority.High
                    || notification.Priority == NotificationPriority.Critical),
            cancellationToken);

        return new NotificationSummary
        {
            TotalCount = totalCount,
            UnreadCount = unreadCount,
            HighPriorityUnreadCount = highPriorityUnreadCount,
        };
    }

    public Task<int> CountUnreadForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        DbSet.CountAsync(
            notification =>
                notification.UserId == userId && notification.Status != NotificationStatus.Read,
            cancellationToken);
}

/// <summary>
/// EF Core repository for notification preferences.
/// </summary>
public sealed class NotificationPreferenceRepository
    : Repository<NotificationPreference>, INotificationPreferenceRepository
{
    public NotificationPreferenceRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<NotificationPreference>> ListForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Where(preference => preference.UserId == userId)
            .OrderBy(preference => preference.NotificationType)
            .ToListAsync(cancellationToken);

    public async Task<NotificationPreference?> GetByUserAndTypeAsync(
        Guid userId,
        NotificationType type,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            preference => preference.UserId == userId && preference.NotificationType == type,
            cancellationToken);
}

/// <summary>
/// EF Core repository for cross-cutting reminders.
/// </summary>
public sealed class ReminderRepository : Repository<Reminder>, IReminderRepository
{
    public ReminderRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Reminder?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            reminder => reminder.Id == id && reminder.UserId == userId,
            cancellationToken);

    public async Task<(IReadOnlyList<Reminder> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        ReminderStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(reminder => reminder.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(reminder => reminder.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(reminder => reminder.DueAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<int> CountActiveForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        DbSet.CountAsync(
            reminder => reminder.UserId == userId && reminder.Status == ReminderStatus.Active,
            cancellationToken);
}

/// <summary>
/// EF Core repository for notification templates.
/// </summary>
public sealed class NotificationTemplateRepository
    : Repository<NotificationTemplate>, INotificationTemplateRepository
{
    public NotificationTemplateRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<NotificationTemplate?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().FirstOrDefaultAsync(
            template => template.Code == code,
            cancellationToken);
}

/// <summary>
/// EF Core repository for background job logs.
/// </summary>
public sealed class BackgroundJobLogRepository
    : Repository<BackgroundJobLog>, IBackgroundJobLogRepository
{
    public BackgroundJobLogRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<(IReadOnlyList<BackgroundJobLog> Items, int TotalCount)> ListRecentAsync(
        int take,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().OrderByDescending(log => log.StartedAt);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Take(take).ToListAsync(cancellationToken);
        return (items, totalCount);
    }
}

/// <summary>
/// EF Core repository for notification schedules (framework stub).
/// </summary>
public sealed class NotificationScheduleRepository
    : Repository<NotificationSchedule>, INotificationScheduleRepository
{
    public NotificationScheduleRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<NotificationSchedule>> ListEnabledAsync(
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Where(schedule => schedule.IsEnabled)
            .OrderBy(schedule => schedule.NextRunAt)
            .ToListAsync(cancellationToken);
}

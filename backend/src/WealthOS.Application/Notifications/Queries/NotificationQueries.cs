using WealthOS.Application.Common.Abstractions;
using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.Application.Notifications.Queries;

/// <summary>
/// Lists notifications for the authenticated user.
/// </summary>
public sealed class GetNotificationsQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public NotificationType? Type { get; init; }

    public NotificationStatus? Status { get; init; }

    public string? Search { get; init; }
}

/// <summary>
/// Lists unread notifications for the authenticated user.
/// </summary>
public sealed class GetUnreadNotificationsQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Returns inbox summary counts.
/// </summary>
public sealed class GetNotificationSummaryQuery : IQuery;

/// <summary>
/// Lists reminders for the authenticated user.
/// </summary>
public sealed class GetRemindersQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public ReminderStatus? Status { get; init; }
}

/// <summary>
/// Returns notification preferences for the authenticated user.
/// </summary>
public sealed class GetNotificationPreferencesQuery : IQuery;

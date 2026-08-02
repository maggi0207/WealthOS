using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Notifications.DTOs.Responses;
using WealthOS.Application.Notifications.Interfaces;
using WealthOS.Application.Notifications.Queries;

namespace WealthOS.Application.Notifications.Queries.Handlers;

public sealed class GetNotificationsQueryHandler
    : IQueryHandler<GetNotificationsQuery, NotificationListResponse>
{
    private readonly INotificationService _notificationService;

    public GetNotificationsQueryHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task<Result<NotificationListResponse>> HandleAsync(
        GetNotificationsQuery query,
        CancellationToken cancellationToken = default) =>
        _notificationService.GetAllAsync(
            query.Page,
            query.PageSize,
            query.Type,
            query.Status,
            query.Search,
            cancellationToken);
}

public sealed class GetUnreadNotificationsQueryHandler
    : IQueryHandler<GetUnreadNotificationsQuery, NotificationListResponse>
{
    private readonly INotificationService _notificationService;

    public GetUnreadNotificationsQueryHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task<Result<NotificationListResponse>> HandleAsync(
        GetUnreadNotificationsQuery query,
        CancellationToken cancellationToken = default) =>
        _notificationService.GetUnreadAsync(query.Page, query.PageSize, cancellationToken);
}

public sealed class GetNotificationSummaryQueryHandler
    : IQueryHandler<GetNotificationSummaryQuery, NotificationSummaryResponse>
{
    private readonly INotificationService _notificationService;

    public GetNotificationSummaryQueryHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task<Result<NotificationSummaryResponse>> HandleAsync(
        GetNotificationSummaryQuery query,
        CancellationToken cancellationToken = default) =>
        _notificationService.GetSummaryAsync(cancellationToken);
}

public sealed class GetRemindersQueryHandler
    : IQueryHandler<GetRemindersQuery, ReminderListResponse>
{
    private readonly IReminderService _reminderService;

    public GetRemindersQueryHandler(IReminderService reminderService)
    {
        _reminderService = reminderService;
    }

    public Task<Result<ReminderListResponse>> HandleAsync(
        GetRemindersQuery query,
        CancellationToken cancellationToken = default) =>
        _reminderService.GetAllAsync(query.Page, query.PageSize, query.Status, cancellationToken);
}

public sealed class GetNotificationPreferencesQueryHandler
    : IQueryHandler<GetNotificationPreferencesQuery, NotificationPreferenceListResponse>
{
    private readonly INotificationService _notificationService;

    public GetNotificationPreferencesQueryHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task<Result<NotificationPreferenceListResponse>> HandleAsync(
        GetNotificationPreferencesQuery query,
        CancellationToken cancellationToken = default) =>
        _notificationService.GetPreferencesAsync(cancellationToken);
}

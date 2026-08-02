using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Notifications.Commands;
using WealthOS.Application.Notifications.Commands.Handlers;
using WealthOS.Application.Notifications.DTOs.Responses;
using WealthOS.Application.Notifications.Interfaces;
using WealthOS.Application.Notifications.Queries;
using WealthOS.Application.Notifications.Queries.Handlers;
using WealthOS.Application.Notifications.Services;

namespace WealthOS.Application.Notifications;

/// <summary>
/// Registers Notifications application services and CQRS handlers.
/// </summary>
public static class NotificationServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationsApplication(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IReminderService, ReminderService>();

        services.AddScoped<
            ICommandHandler<CreateNotificationCommand, NotificationResponse>,
            CreateNotificationCommandHandler>();
        services.AddScoped<
            ICommandHandler<MarkNotificationAsReadCommand>,
            MarkNotificationAsReadCommandHandler>();
        services.AddScoped<
            ICommandHandler<DeleteNotificationCommand>,
            DeleteNotificationCommandHandler>();
        services.AddScoped<
            ICommandHandler<CreateReminderCommand, ReminderResponse>,
            CreateReminderCommandHandler>();
        services.AddScoped<
            ICommandHandler<UpdateNotificationPreferencesCommand, NotificationPreferenceListResponse>,
            UpdateNotificationPreferencesCommandHandler>();

        services.AddScoped<
            IQueryHandler<GetNotificationsQuery, NotificationListResponse>,
            GetNotificationsQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetUnreadNotificationsQuery, NotificationListResponse>,
            GetUnreadNotificationsQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetNotificationSummaryQuery, NotificationSummaryResponse>,
            GetNotificationSummaryQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetRemindersQuery, ReminderListResponse>,
            GetRemindersQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetNotificationPreferencesQuery, NotificationPreferenceListResponse>,
            GetNotificationPreferencesQueryHandler>();

        return services;
    }
}

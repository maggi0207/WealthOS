using WealthOS.Application.Common.Models;
using WealthOS.Application.Notifications.DTOs.Requests;
using WealthOS.Application.Notifications.DTOs.Responses;
using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.Application.Notifications.Interfaces;

/// <summary>
/// Application service for notification CRUD and inbox queries.
/// </summary>
public interface INotificationService
{
    Task<Result<NotificationResponse>> CreateAsync(
        CreateNotificationRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> MarkAsReadAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default);

    Task<Result<NotificationListResponse>> GetAllAsync(
        int page,
        int pageSize,
        NotificationType? type,
        NotificationStatus? status,
        string? search,
        CancellationToken cancellationToken = default);

    Task<Result<NotificationListResponse>> GetUnreadAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<NotificationSummaryResponse>> GetSummaryAsync(
        CancellationToken cancellationToken = default);

    Task<Result<NotificationPreferenceListResponse>> UpdatePreferencesAsync(
        UpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<NotificationPreferenceListResponse>> GetPreferencesAsync(
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Application service for cross-cutting reminders.
/// </summary>
public interface IReminderService
{
    Task<Result<ReminderResponse>> CreateAsync(
        CreateReminderRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<ReminderListResponse>> GetAllAsync(
        int page,
        int pageSize,
        ReminderStatus? status,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Application-facing abstraction for enqueueing / registering background work.
/// Hangfire wiring lives in Infrastructure.
/// </summary>
public interface IBackgroundJobService
{
    /// <summary>
    /// Enqueues a fire-and-forget stub job by name (framework placeholder).
    /// </summary>
    Task<Result<string>> EnqueueStubJobAsync(
        string jobName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensures recurring stub jobs are registered (idempotent).
    /// </summary>
    void RegisterRecurringJobs();
}

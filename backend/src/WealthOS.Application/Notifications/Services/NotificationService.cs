using AutoMapper;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Notifications.DTOs.Requests;
using WealthOS.Application.Notifications.DTOs.Responses;
using WealthOS.Application.Notifications.Interfaces;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Notifications.Entities;
using WealthOS.Domain.Notifications.Enums;
using WealthOS.Domain.Notifications.Models;
using WealthOS.Domain.Notifications.Repositories;

namespace WealthOS.Application.Notifications.Services;

/// <summary>
/// Orchestrates notification create/read/delete and preference updates.
/// </summary>
public sealed class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationPreferenceRepository _preferenceRepository;
    private readonly IReminderRepository _reminderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public NotificationService(
        INotificationRepository notificationRepository,
        INotificationPreferenceRepository preferenceRepository,
        IReminderRepository reminderRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _preferenceRepository = preferenceRepository;
        _reminderRepository = reminderRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<NotificationResponse>> CreateAsync(
        CreateNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<NotificationResponse>(userResult.Error!);
        }

        var notification = _mapper.Map<Notification>(request);
        notification.UserId = userResult.Value;
        notification.Status = request.ScheduledAt.HasValue && request.ScheduledAt > DateTime.UtcNow
            ? NotificationStatus.Scheduled
            : NotificationStatus.Sent;
        notification.SentAt = notification.Status == NotificationStatus.Sent ? DateTime.UtcNow : null;

        notification.Recipients.Add(new NotificationRecipient
        {
            UserId = userResult.Value,
            Channel = notification.Channel,
            DeliveryStatus = notification.Status == NotificationStatus.Sent
                ? NotificationDeliveryStatus.Delivered
                : NotificationDeliveryStatus.Pending,
            DeliveredAt = notification.SentAt,
        });

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<NotificationResponse>(notification));
    }

    public async Task<Result> MarkAsReadAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error!);
        }

        var notification = await _notificationRepository.GetByIdForUserAsync(
            notificationId,
            userResult.Value,
            cancellationToken);

        if (notification is null)
        {
            return Result.Failure(Error.NotFound(nameof(Notification), notificationId));
        }

        if (notification.Status != NotificationStatus.Read)
        {
            notification.Status = NotificationStatus.Read;
            notification.ReadAt = DateTime.UtcNow;
            _notificationRepository.Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error!);
        }

        var notification = await _notificationRepository.GetByIdForUserAsync(
            notificationId,
            userResult.Value,
            cancellationToken);

        if (notification is null)
        {
            return Result.Failure(Error.NotFound(nameof(Notification), notificationId));
        }

        _notificationRepository.Remove(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<NotificationListResponse>> GetAllAsync(
        int page,
        int pageSize,
        NotificationType? type,
        NotificationStatus? status,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<NotificationListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, totalCount) = await _notificationRepository.ListForUserAsync(
            userResult.Value,
            new NotificationSearchCriteria
            {
                Type = type,
                Status = status,
                Search = search,
            },
            page,
            pageSize,
            cancellationToken);

        return Result.Success(new NotificationListResponse
        {
            Items = _mapper.Map<IReadOnlyList<NotificationListItemResponse>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        });
    }

    public async Task<Result<NotificationListResponse>> GetUnreadAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<NotificationListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, totalCount) = await _notificationRepository.ListUnreadForUserAsync(
            userResult.Value,
            page,
            pageSize,
            cancellationToken);

        return Result.Success(new NotificationListResponse
        {
            Items = _mapper.Map<IReadOnlyList<NotificationListItemResponse>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        });
    }

    public async Task<Result<NotificationSummaryResponse>> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<NotificationSummaryResponse>(userResult.Error!);
        }

        var summary = await _notificationRepository.GetSummaryForUserAsync(
            userResult.Value,
            cancellationToken);

        var pendingReminders = await _reminderRepository.CountActiveForUserAsync(
            userResult.Value,
            cancellationToken);

        return Result.Success(new NotificationSummaryResponse
        {
            TotalCount = summary.TotalCount,
            UnreadCount = summary.UnreadCount,
            HighPriorityUnreadCount = summary.HighPriorityUnreadCount,
            PendingReminderCount = pendingReminders,
        });
    }

    public async Task<Result<NotificationPreferenceListResponse>> UpdatePreferencesAsync(
        UpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<NotificationPreferenceListResponse>(userResult.Error!);
        }

        foreach (var item in request.Preferences)
        {
            var existing = await _preferenceRepository.GetByUserAndTypeAsync(
                userResult.Value,
                item.NotificationType,
                cancellationToken);

            if (existing is null)
            {
                var preference = _mapper.Map<NotificationPreference>(item);
                preference.UserId = userResult.Value;
                await _preferenceRepository.AddAsync(preference, cancellationToken);
            }
            else
            {
                existing.EnableInApp = item.EnableInApp;
                existing.EnableEmail = item.EnableEmail;
                existing.EnableSms = item.EnableSms;
                existing.EnablePush = item.EnablePush;
                existing.EnableWhatsApp = item.EnableWhatsApp;
                existing.QuietHoursStartMinutes = item.QuietHoursStartMinutes;
                existing.QuietHoursEndMinutes = item.QuietHoursEndMinutes;
                _preferenceRepository.Update(existing);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetPreferencesAsync(cancellationToken);
    }

    public async Task<Result<NotificationPreferenceListResponse>> GetPreferencesAsync(
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<NotificationPreferenceListResponse>(userResult.Error!);
        }

        var preferences = await _preferenceRepository.ListForUserAsync(
            userResult.Value,
            cancellationToken);

        return Result.Success(new NotificationPreferenceListResponse
        {
            Preferences = _mapper.Map<IReadOnlyList<NotificationPreferenceResponse>>(preferences),
        });
    }

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }
}

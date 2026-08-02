using AutoMapper;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Notifications.DTOs.Requests;
using WealthOS.Application.Notifications.DTOs.Responses;
using WealthOS.Application.Notifications.Interfaces;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Notifications.Entities;
using WealthOS.Domain.Notifications.Enums;
using WealthOS.Domain.Notifications.Repositories;

namespace WealthOS.Application.Notifications.Services;

/// <summary>
/// Orchestrates cross-cutting reminder create and list operations.
/// </summary>
public sealed class ReminderService : IReminderService
{
    private readonly IReminderRepository _reminderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public ReminderService(
        IReminderRepository reminderRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _reminderRepository = reminderRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<ReminderResponse>> CreateAsync(
        CreateReminderRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ReminderResponse>(userResult.Error!);
        }

        var reminder = _mapper.Map<Reminder>(request);
        reminder.UserId = userResult.Value;
        reminder.Status = ReminderStatus.Active;

        await _reminderRepository.AddAsync(reminder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<ReminderResponse>(reminder));
    }

    public async Task<Result<ReminderListResponse>> GetAllAsync(
        int page,
        int pageSize,
        ReminderStatus? status,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ReminderListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, totalCount) = await _reminderRepository.ListForUserAsync(
            userResult.Value,
            status,
            page,
            pageSize,
            cancellationToken);

        return Result.Success(new ReminderListResponse
        {
            Items = _mapper.Map<IReadOnlyList<ReminderResponse>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
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

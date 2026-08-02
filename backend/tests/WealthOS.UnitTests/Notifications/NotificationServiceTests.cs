using AutoMapper;
using FluentAssertions;
using Moq;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Notifications.DTOs.Requests;
using WealthOS.Application.Notifications.Mapping;
using WealthOS.Application.Notifications.Services;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Notifications.Entities;
using WealthOS.Domain.Notifications.Enums;
using WealthOS.Domain.Notifications.Models;
using WealthOS.Domain.Notifications.Repositories;

namespace WealthOS.UnitTests.Notifications;

/// <summary>
/// Unit tests for NotificationService create / mark-read paths.
/// </summary>
public sealed class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _notificationRepository = new();
    private readonly Mock<INotificationPreferenceRepository> _preferenceRepository = new();
    private readonly Mock<IReminderRepository> _reminderRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly NotificationService _sut;
    private readonly Guid _userId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    public NotificationServiceTests()
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<NotificationMappingProfile>())
            .CreateMapper();

        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(true);
        _currentUser.SetupGet(user => user.UserId).Returns(_userId);

        _notificationRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _sut = new NotificationService(
            _notificationRepository.Object,
            _preferenceRepository.Object,
            _reminderRepository.Object,
            _unitOfWork.Object,
            _currentUser.Object,
            mapper);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_ShouldPersistAndReturnNotification()
    {
        var result = await _sut.CreateAsync(new CreateNotificationRequest
        {
            Title = "EMI due",
            Message = "Home loan EMI is due tomorrow.",
            Type = NotificationType.EmiReminder,
            Channel = NotificationChannel.InApp,
            Priority = NotificationPriority.High,
        });

        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be("EMI due");
        result.Value.Status.Should().Be(NotificationStatus.Sent);
        _notificationRepository.Verify(
            repo => repo.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsReadAsync_WhenFound_ShouldUpdateStatus()
    {
        var notificationId = Guid.NewGuid();
        var notification = new Notification(notificationId)
        {
            UserId = _userId,
            Title = "Test",
            Message = "Body",
            Status = NotificationStatus.Sent,
        };

        _notificationRepository
            .Setup(repo => repo.GetByIdForUserAsync(notificationId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        var result = await _sut.MarkAsReadAsync(notificationId);

        result.IsSuccess.Should().BeTrue();
        notification.Status.Should().Be(NotificationStatus.Read);
        notification.ReadAt.Should().NotBeNull();
        _notificationRepository.Verify(repo => repo.Update(notification), Times.Once);
    }

    [Fact]
    public async Task MarkAsReadAsync_WhenMissing_ShouldReturnNotFound()
    {
        _notificationRepository
            .Setup(repo => repo.GetByIdForUserAsync(It.IsAny<Guid>(), _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification?)null);

        var result = await _sut.MarkAsReadAsync(Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("not_found");
    }

    [Fact]
    public async Task GetSummaryAsync_WhenAuthenticated_ShouldReturnCounts()
    {
        _notificationRepository
            .Setup(repo => repo.GetSummaryForUserAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NotificationSummary
            {
                TotalCount = 5,
                UnreadCount = 2,
                HighPriorityUnreadCount = 1,
            });

        _reminderRepository
            .Setup(repo => repo.CountActiveForUserAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var result = await _sut.GetSummaryAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(5);
        result.Value.UnreadCount.Should().Be(2);
        result.Value.PendingReminderCount.Should().Be(3);
    }
}

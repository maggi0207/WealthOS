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
using WealthOS.Domain.Notifications.Repositories;

namespace WealthOS.UnitTests.Notifications;

/// <summary>
/// Unit tests for ReminderService.
/// </summary>
public sealed class ReminderServiceTests
{
    private readonly Mock<IReminderRepository> _reminderRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly ReminderService _sut;
    private readonly Guid _userId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    public ReminderServiceTests()
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<NotificationMappingProfile>())
            .CreateMapper();

        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(true);
        _currentUser.SetupGet(user => user.UserId).Returns(_userId);

        _reminderRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Reminder>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _sut = new ReminderService(
            _reminderRepository.Object,
            _unitOfWork.Object,
            _currentUser.Object,
            mapper);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_ShouldPersistReminder()
    {
        var dueAt = DateTime.UtcNow.AddDays(3);

        var result = await _sut.CreateAsync(new CreateReminderRequest
        {
            Title = "Document renewal",
            Message = "Renew passport",
            ReminderType = NotificationType.DocumentExpiry,
            DueAt = dueAt,
        });

        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be("Document renewal");
        result.Value.Status.Should().Be(ReminderStatus.Active);
        result.Value.DueAt.Should().Be(dueAt);
        _reminderRepository.Verify(
            repo => repo.AddAsync(It.IsAny<Reminder>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenUnauthenticated_ShouldFail()
    {
        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(false);
        _currentUser.SetupGet(user => user.UserId).Returns((Guid?)null);

        var result = await _sut.CreateAsync(new CreateReminderRequest
        {
            Title = "Test",
            DueAt = DateTime.UtcNow.AddDays(1),
        });

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("unauthorized");
    }
}

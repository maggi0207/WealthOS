using FluentAssertions;
using Moq;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.DTOs.Responses;
using WealthOS.Application.Dashboard.Interfaces;
using WealthOS.Application.Dashboard.Queries;
using WealthOS.Application.Dashboard.Queries.Handlers;
using WealthOS.Application.Dashboard.Validators;

namespace WealthOS.UnitTests.Dashboard;

public sealed class GetRecentActivitiesQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenLimitInvalid_ShouldReturnValidationError()
    {
        var dashboard = new Mock<IDashboardService>();
        var handler = new GetRecentActivitiesQueryHandler(dashboard.Object, new GetRecentActivitiesQueryValidator());

        var result = await handler.HandleAsync(new GetRecentActivitiesQuery { Limit = 0 });

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("validation_error");
        dashboard.Verify(
            service => service.GetRecentActivitiesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenLimitValid_ShouldDelegateToService()
    {
        var dashboard = new Mock<IDashboardService>();
        dashboard
            .Setup(service => service.GetRecentActivitiesAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IReadOnlyList<RecentActivityResponse>>([]));

        var handler = new GetRecentActivitiesQueryHandler(dashboard.Object, new GetRecentActivitiesQueryValidator());

        var result = await handler.HandleAsync(new GetRecentActivitiesQuery { Limit = 5 });

        result.IsSuccess.Should().BeTrue();
        dashboard.Verify(
            service => service.GetRecentActivitiesAsync(5, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

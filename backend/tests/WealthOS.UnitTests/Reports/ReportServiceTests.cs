using FluentAssertions;
using Moq;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.DTOs.Responses;
using WealthOS.Application.Dashboard.Interfaces;
using WealthOS.Application.Dashboard.Providers;
using WealthOS.Application.Documents.DTOs.Responses;
using WealthOS.Application.Documents.Interfaces;
using WealthOS.Application.Goals.Interfaces;
using WealthOS.Application.Income.Interfaces;
using WealthOS.Application.Investments.DTOs.Responses;
using WealthOS.Application.Investments.Interfaces;
using WealthOS.Application.Loans.DTOs.Responses;
using WealthOS.Application.Loans.Interfaces;
using WealthOS.Application.Notifications.DTOs.Responses;
using WealthOS.Application.Notifications.Interfaces;
using WealthOS.Application.Properties.DTOs.Responses;
using WealthOS.Application.Properties.Interfaces;
using WealthOS.Application.Reports.Services;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Reports.Entities;
using WealthOS.Domain.Reports.Enums;
using WealthOS.Domain.Reports.Repositories;

namespace WealthOS.UnitTests.Reports;

/// <summary>
/// Unit test skeleton for ReportService aggregation via module interfaces.
/// </summary>
public sealed class ReportServiceTests
{
    private readonly Mock<IDashboardService> _dashboard = new();
    private readonly Mock<IPropertyService> _properties = new();
    private readonly Mock<ILoanService> _loans = new();
    private readonly Mock<IIncomeService> _income = new();
    private readonly Mock<IInvestmentService> _investments = new();
    private readonly Mock<IPortfolioService> _portfolio = new();
    private readonly Mock<IAllocationService> _allocation = new();
    private readonly Mock<IGoalService> _goals = new();
    private readonly Mock<IDocumentSummaryProvider> _documentSummary = new();
    private readonly Mock<IDocumentSearchService> _documentSearch = new();
    private readonly Mock<INotificationService> _notifications = new();
    private readonly Mock<IReportDefinitionRepository> _definitions = new();
    private readonly Mock<IReportExecutionRepository> _executions = new();
    private readonly Mock<IReportSnapshotRepository> _snapshots = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly ReportService _sut;
    private readonly Guid _userId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    public ReportServiceTests()
    {
        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(true);
        _currentUser.SetupGet(user => user.UserId).Returns(_userId);
        _unitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _executions
            .Setup(repo => repo.AddAsync(It.IsAny<ReportExecution>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _definitions
            .Setup(repo => repo.GetByTypeAsync(It.IsAny<ReportType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReportDefinition?)null);

        _sut = new ReportService(
            _dashboard.Object,
            _properties.Object,
            _loans.Object,
            _income.Object,
            _investments.Object,
            _portfolio.Object,
            _allocation.Object,
            _goals.Object,
            _documentSummary.Object,
            _documentSearch.Object,
            _notifications.Object,
            _definitions.Object,
            _executions.Object,
            _snapshots.Object,
            _unitOfWork.Object,
            _currentUser.Object);
    }

    [Fact]
    public async Task GetNetWorthReportAsync_ShouldAggregate_WhenDashboardSucceeds()
    {
        _dashboard
            .Setup(service => service.GetNetWorthAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new NetWorthResponse
            {
                NetWorth = 900_000m,
                AssetValue = 1_100_000m,
                LiabilityValue = 200_000m,
                ChangePercent = 3.5m,
                CurrencyCode = "INR",
            }));

        _dashboard
            .Setup(service => service.GetSummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new DashboardResponse
            {
                PropertyValue = 500_000m,
                InvestmentValue = 400_000m,
                LoanBalance = 200_000m,
                CurrencyCode = "INR",
            }));

        _properties
            .Setup(service => service.GetSummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new PropertySummaryResponse
            {
                TotalMarketValue = 500_000m,
                CurrencyCode = "INR",
            }));

        _investments
            .Setup(service => service.GetDashboardSummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new InvestmentDashboardResponse
            {
                PortfolioValue = 400_000m,
            }));

        _loans
            .Setup(service => service.GetSummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new LoanSummaryResponse
            {
                OutstandingBalance = 200_000m,
                CurrencyCode = "INR",
            }));

        var result = await _sut.GetNetWorthReportAsync(null);

        result.IsSuccess.Should().BeTrue();
        result.Value.NetWorth.Should().Be(900_000m);
        result.Value.ReportType.Should().Be(ReportType.NetWorth);
        result.Value.DataSources.Should().Contain("Dashboard");
    }

    [Fact]
    public async Task GetDocumentReportAsync_ShouldIncludeNotificationUnread()
    {
        _documentSummary
            .Setup(provider => provider.GetSummaryAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DocumentModuleSummary { DocumentCount = 5, PendingReviewCount = 1 });

        _documentSearch
            .Setup(service => service.GetRecentAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new DocumentListResponse
            {
                Items = Array.Empty<DocumentListItemResponse>(),
            }));

        _documentSearch
            .Setup(service => service.GetExpiredAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new DocumentListResponse
            {
                Items = Array.Empty<DocumentListItemResponse>(),
            }));

        _notifications
            .Setup(service => service.GetSummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new NotificationSummaryResponse { UnreadCount = 3 }));

        var result = await _sut.GetDocumentReportAsync(null);

        result.IsSuccess.Should().BeTrue();
        result.Value.DocumentCount.Should().Be(5);
        result.Value.UnreadNotifications.Should().Be(3);
    }
}

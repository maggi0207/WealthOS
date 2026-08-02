using AutoMapper;
using FluentAssertions;
using Moq;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Dashboard.Mapping;
using WealthOS.Application.Dashboard.Providers;
using WealthOS.Application.Dashboard.Services;

namespace WealthOS.UnitTests.Dashboard;

public sealed class DashboardServiceTests
{
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IPropertySummaryProvider> _property = new();
    private readonly Mock<ILoanSummaryProvider> _loan = new();
    private readonly Mock<IInvestmentSummaryProvider> _investment = new();
    private readonly Mock<IIncomeSummaryProvider> _income = new();
    private readonly Mock<IDocumentSummaryProvider> _document = new();
    private readonly IMapper _mapper;

    public DashboardServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<DashboardMappingProfile>());
        _mapper = config.CreateMapper();

        var userId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(true);
        _currentUser.SetupGet(user => user.UserId).Returns(userId);

        _property
            .Setup(provider => provider.GetSummaryAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PropertyModuleSummary { TotalValue = 1_068_000m, PropertyCount = 2 });

        _loan
            .Setup(provider => provider.GetSummaryAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoanModuleSummary { TotalBalance = 655_600m, LoanCount = 4 });

        _investment
            .Setup(provider => provider.GetSummaryAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InvestmentModuleSummary { TotalValue = 1_697_000m, HoldingCount = 12 });

        _income
            .Setup(provider => provider.GetSummaryAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IncomeModuleSummary
            {
                MonthlyIncome = 24_800m,
                MonthlyExpense = 15_380m,
                CurrencyCode = "USD",
            });

        _document
            .Setup(provider => provider.GetSummaryAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DocumentModuleSummary { DocumentCount = 18, PendingReviewCount = 2 });
    }

    [Fact]
    public async Task GetSummaryAsync_WhenAuthenticated_ShouldAggregateModuleProviders()
    {
        var service = CreateService();

        var result = await service.GetSummaryAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.NetWorth.Should().Be(2_486_400m);
        result.Value.AssetValue.Should().Be(3_142_000m);
        result.Value.LiabilityValue.Should().Be(655_600m);
        result.Value.PropertyValue.Should().Be(1_068_000m);
        result.Value.InvestmentValue.Should().Be(1_697_000m);
        result.Value.LoanBalance.Should().Be(655_600m);
        result.Value.MonthlyIncome.Should().Be(24_800m);
        result.Value.MonthlyExpense.Should().Be(15_380m);
        result.Value.HealthScore.Score.Should().BeGreaterThan(0);
        result.Value.QuickActions.Should().HaveCount(4);
        result.Value.RecentActivities.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetNetWorthAsync_WhenUnauthenticated_ShouldFailUnauthorized()
    {
        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(false);
        _currentUser.SetupGet(user => user.UserId).Returns((Guid?)null);

        var service = CreateService();
        var result = await service.GetNetWorthAsync();

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("unauthorized");
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_ShouldRespectLimit()
    {
        var service = CreateService();

        var result = await service.GetRecentActivitiesAsync(2);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetHealthAsync_WhenProvidersSucceed_ShouldBeHealthy()
    {
        var service = CreateService();

        var result = await service.GetHealthAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Healthy");
        result.Value.ProvidersReady.Should().BeTrue();
        result.Value.ProviderStatuses.Should().ContainKey("property");
    }

    private DashboardService CreateService() =>
        new(
            _currentUser.Object,
            _property.Object,
            _loan.Object,
            _investment.Object,
            _income.Object,
            _document.Object,
            _mapper);
}

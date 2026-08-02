using FluentAssertions;
using Moq;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.DTOs.Responses;
using WealthOS.Application.Dashboard.Interfaces;
using WealthOS.Application.Goals.DTOs.Responses;
using WealthOS.Application.Goals.Interfaces;
using WealthOS.Application.Income.DTOs.Responses;
using WealthOS.Application.Income.Interfaces;
using WealthOS.Application.Loans.DTOs.Responses;
using WealthOS.Application.Loans.Interfaces;
using WealthOS.Application.Reports.Services;
using WealthOS.Domain.Reports.Enums;

namespace WealthOS.UnitTests.Reports;

/// <summary>
/// Unit test skeleton for FinancialHealthService scoring.
/// </summary>
public sealed class FinancialHealthServiceTests
{
    private readonly Mock<IDashboardService> _dashboard = new();
    private readonly Mock<ILoanService> _loans = new();
    private readonly Mock<IIncomeService> _income = new();
    private readonly Mock<IGoalService> _goals = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly FinancialHealthService _sut;
    private readonly Guid _userId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    public FinancialHealthServiceTests()
    {
        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(true);
        _currentUser.SetupGet(user => user.UserId).Returns(_userId);

        _sut = new FinancialHealthService(
            _dashboard.Object,
            _loans.Object,
            _income.Object,
            _goals.Object,
            _currentUser.Object);
    }

    [Fact]
    public async Task GetFinancialHealthAsync_ShouldReturnScore_WhenModulesSucceed()
    {
        _dashboard
            .Setup(service => service.GetSummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new DashboardResponse
            {
                NetWorth = 1_000_000m,
                AssetValue = 1_200_000m,
                LiabilityValue = 200_000m,
                MonthlyIncome = 100_000m,
                ChangePercent = 5m,
                CurrencyCode = "INR",
                HealthScore = new HealthScoreResponse { Score = 80, Grade = "B" },
            }));

        _loans
            .Setup(service => service.GetSummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new LoanSummaryResponse
            {
                OutstandingBalance = 200_000m,
                LoanCount = 1,
                CurrencyCode = "INR",
            }));

        _income
            .Setup(service => service.GetProfitLossAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new ProfitLossResponse
            {
                SavingsRatePercent = 25m,
                NetProfit = 50_000m,
            }));

        _income
            .Setup(service => service.GetCashFlowAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new CashFlowResponse
            {
                NetCashFlow = 30_000m,
                CurrencyCode = "INR",
            }));

        _goals
            .Setup(service => service.GetDashboardAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new GoalDashboardResponse
            {
                OverallProgressPercent = 60m,
                CurrencyCode = "INR",
            }));

        var result = await _sut.GetFinancialHealthAsync(null);

        result.IsSuccess.Should().BeTrue();
        result.Value.Score.Should().BeInRange(0, 100);
        result.Value.ReportType.Should().Be(ReportType.FinancialHealthScore);
        result.Value.Factors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetFinancialHealthAsync_ShouldFail_WhenUnauthenticated()
    {
        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(false);
        _currentUser.SetupGet(user => user.UserId).Returns((Guid?)null);

        var result = await _sut.GetFinancialHealthAsync(null);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("unauthorized");
    }
}

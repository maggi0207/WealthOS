using AutoMapper;
using FluentAssertions;
using WealthOS.Application.Dashboard.DTOs.Responses;
using WealthOS.Application.Dashboard.Mapping;
using WealthOS.Domain.Dashboard.Models;

namespace WealthOS.UnitTests.Dashboard;

public sealed class DashboardMappingTests
{
    private readonly IMapper _mapper;

    public DashboardMappingTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<DashboardMappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void DashboardMappingProfile_ShouldBeValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<DashboardMappingProfile>());

        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_DashboardSummary_ShouldFlattenFinancials()
    {
        var summary = new DashboardSummary
        {
            Financials = new FinancialSummary
            {
                NetWorth = 100m,
                AssetValue = 150m,
                LiabilityValue = 50m,
                MonthlyIncome = 10m,
                MonthlyExpense = 4m,
                InvestmentValue = 70m,
                PropertyValue = 80m,
                LoanBalance = 50m,
                ChangePercent = 1.5m,
                CurrencyCode = "USD",
            },
            HealthScore = new HealthScore
            {
                Score = 78,
                Grade = "Strong",
                ChangePoints = 4,
                Factors =
                [
                    new HealthScoreFactor { Label = "Savings rate", Value = 86, Weight = "High" },
                ],
            },
            RecentActivities =
            [
                new RecentActivity
                {
                    Id = Guid.NewGuid(),
                    Title = "Salary",
                    Detail = "Payroll",
                    Amount = 1000m,
                    Direction = "in",
                    Category = "Income",
                    OccurredAt = DateTimeOffset.UtcNow,
                },
            ],
            QuickActions =
            [
                new QuickAction
                {
                    Key = "add-income",
                    Label = "Add income",
                    Route = "/income",
                    Icon = "banknote",
                },
            ],
            GeneratedAt = DateTimeOffset.UtcNow,
        };

        var response = _mapper.Map<DashboardResponse>(summary);

        response.NetWorth.Should().Be(100m);
        response.AssetValue.Should().Be(150m);
        response.HealthScore.Score.Should().Be(78);
        response.HealthScore.Factors.Should().HaveCount(1);
        response.RecentActivities.Should().HaveCount(1);
        response.QuickActions.Should().HaveCount(1);
    }
}

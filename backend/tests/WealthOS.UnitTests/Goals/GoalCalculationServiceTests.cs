using FluentAssertions;
using WealthOS.Application.Goals.Calculations;
using WealthOS.Domain.Goals.Enums;

namespace WealthOS.UnitTests.Goals;

public sealed class GoalCalculationServiceTests
{
    private readonly GoalCalculationService _sut = new();

    [Fact]
    public void CalculateCompletionPercent_ShouldClampToHundred()
    {
        _sut.CalculateCompletionPercent(31_50_000m, 90_00_000m).Should().Be(35m);
        _sut.CalculateCompletionPercent(12_00_000m, 12_00_000m).Should().Be(100m);
        _sut.CalculateCompletionPercent(15_00_000m, 12_00_000m).Should().Be(100m);
        _sut.CalculateCompletionPercent(100m, 0m).Should().Be(0m);
    }

    [Fact]
    public void CalculateRemainingAmount_ShouldNeverBeNegative()
    {
        _sut.CalculateRemainingAmount(31_50_000m, 90_00_000m).Should().Be(58_50_000m);
        _sut.CalculateRemainingAmount(12_00_000m, 12_00_000m).Should().Be(0m);
        _sut.CalculateRemainingAmount(13_00_000m, 12_00_000m).Should().Be(0m);
    }

    [Fact]
    public void CalculateMonthlyRequiredContribution_ShouldDivideRemainingByMonths()
    {
        var from = new DateOnly(2026, 8, 1);
        var target = new DateOnly(2031, 4, 1);

        var required = _sut.CalculateMonthlyRequiredContribution(
            31_50_000m,
            90_00_000m,
            from,
            target);

        required.Should().BeGreaterThan(0m);
        var months = _sut.CalculateMonthsRemaining(from, target);
        months.Should().Be(56);
        required.Should().Be(_sut.RoundMoney(58_50_000m / 56m));
    }

    [Fact]
    public void EstimateCompletionDate_WhenNoContribution_ShouldReturnNull()
    {
        var estimate = _sut.EstimateCompletionDate(
            10_000m,
            100_000m,
            0m,
            new DateOnly(2026, 8, 1));

        estimate.Should().BeNull();
    }

    [Fact]
    public void DetermineTrend_ShouldClassifyAheadOnTrackBehind()
    {
        var from = new DateOnly(2026, 8, 1);
        var target = new DateOnly(2027, 8, 1);

        _sut.DetermineTrend(50_000m, 100_000m, 10_000m, from, target, GoalStatus.Active)
            .Should().Be(ProgressTrend.Ahead);

        _sut.DetermineTrend(50_000m, 100_000m, 4_000m, from, target, GoalStatus.Active)
            .Should().BeOneOf(ProgressTrend.OnTrack, ProgressTrend.Behind);

        _sut.DetermineTrend(50_000m, 100_000m, 1_000m, from, target, GoalStatus.Active)
            .Should().Be(ProgressTrend.Behind);

        _sut.DetermineTrend(100_000m, 100_000m, 0m, from, target, GoalStatus.Completed)
            .Should().Be(ProgressTrend.Completed);
    }

    [Fact]
    public void BuildProjection_ShouldProducePointsAndShortfall()
    {
        var projection = _sut.BuildProjection(
            Guid.NewGuid(),
            "Buy second house",
            90_00_000m,
            31_50_000m,
            85_000m,
            new DateOnly(2031, 4, 1),
            new DateOnly(2026, 8, 1));

        projection.Points.Should().NotBeEmpty();
        projection.MonthlyRequiredContribution.Should().BeGreaterThan(0m);
        projection.GoalName.Should().Be("Buy second house");
    }
}

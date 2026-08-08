using FluentAssertions;
using WealthOS.Application.Investments.Calculations;
using WealthOS.Domain.Investments.Enums;

namespace WealthOS.UnitTests.Investments;

public sealed class InvestmentCalculationServiceTests
{
    private readonly InvestmentCalculationService _sut = new();

    [Fact]
    public void BuildPortfolio_ShouldComputeGainsAndReturn()
    {
        var portfolio = _sut.BuildPortfolio(
            investedAmount: 1_26_00_000m,
            currentValue: 1_47_20_000m,
            todaysGain: 62_400m,
            accountCount: 4,
            holdingCount: 13);

        portfolio.OverallGain.Should().Be(21_20_000m);
        portfolio.AbsoluteReturnPercent.Should().Be(16.83m);
        portfolio.TodaysGain.Should().Be(62_400m);
        portfolio.XirrPercent.Should().BeNull();
        portfolio.AccountCount.Should().Be(4);
        portfolio.HoldingCount.Should().Be(13);
    }

    [Fact]
    public void BuildAllocation_ShouldWeightSlices()
    {
        var allocation = _sut.BuildAllocation(
        [
            (InvestmentCategory.Stocks, 52_30_000m),
            (InvestmentCategory.MutualFunds, 41_60_000m),
            (InvestmentCategory.Cash, 7_00_000m),
        ]);

        allocation.TotalValue.Should().Be(1_00_90_000m);
        allocation.Slices.Should().HaveCount(3);
        allocation.Slices[0].Category.Should().Be(InvestmentCategory.Stocks);
        allocation.Slices.Sum(s => s.WeightPercent).Should().BeApproximately(100m, 0.1m);
    }

    [Fact]
    public void BuildPortfolio_WhenZeroInvested_ShouldReturnZeroAbsoluteReturn()
    {
        var portfolio = _sut.BuildPortfolio(0m, 0m, 0m, 0, 0);
        portfolio.AbsoluteReturnPercent.Should().Be(0m);
        portfolio.OverallGain.Should().Be(0m);
    }

    [Fact]
    public void CalculateXirrPlaceholder_ShouldReturnNullUnlessProvided()
    {
        _sut.CalculateXirrPlaceholder().Should().BeNull();
        _sut.CalculateXirrPlaceholder(null).Should().BeNull();
        _sut.CalculateXirrPlaceholder(12.5m).Should().Be(12.5m);
    }
}

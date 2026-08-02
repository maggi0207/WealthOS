using FluentAssertions;
using WealthOS.Application.Income.Calculations;

namespace WealthOS.UnitTests.Income;

public sealed class IncomeCalculationServiceTests
{
    private readonly IncomeCalculationService _sut = new();

    [Fact]
    public void BuildDashboard_ShouldComputeKpisWithoutTax()
    {
        var summary = _sut.BuildDashboard(
            period: "2026-07",
            salaryIncome: 385_000m,
            businessRevenue: 640_000m,
            developerCost: 310_000m,
            businessExpenses: 78_500m,
            outstandingInvoices: 398_000m);

        summary.MonthlyIncome.Should().Be(1_025_000m);
        summary.NetProfit.Should().Be(251_500m);
        summary.CashAvailable.Should().Be(636_500m);
        summary.OutstandingInvoices.Should().Be(398_000m);
        summary.SavingsRatePercent.Should().Be(62.10m);
    }

    [Fact]
    public void BuildMonthlyProfit_WhenNoInflows_ShouldReturnZeroSavingsRate()
    {
        var profit = _sut.BuildMonthlyProfit(
            period: "2026-07",
            salaryIncome: 0m,
            businessRevenue: 0m,
            developerCost: 0m,
            businessExpenses: 0m);

        profit.SavingsRatePercent.Should().Be(0m);
        profit.CashAvailable.Should().Be(0m);
    }

    [Fact]
    public void FormatPeriodLabel_ShouldReturnMonthYear()
    {
        _sut.FormatPeriodLabel("2026-07").Should().Be("July 2026");
    }

    [Fact]
    public void CurrentPeriod_ShouldUseUtcMonth()
    {
        var period = _sut.CurrentPeriod(new DateTime(2026, 8, 2, 0, 0, 0, DateTimeKind.Utc));
        period.Should().Be("2026-08");
    }
}

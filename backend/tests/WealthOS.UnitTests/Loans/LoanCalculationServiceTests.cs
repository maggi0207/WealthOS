using FluentAssertions;
using WealthOS.Application.Loans.Calculations;

namespace WealthOS.UnitTests.Loans;

public sealed class LoanCalculationServiceTests
{
    private readonly LoanCalculationService _sut = new();

    [Fact]
    public void CalculateTotalPrincipalPaid_ShouldSubtractOutstanding()
    {
        _sut.CalculateTotalPrincipalPaid(1_000_000m, 400_000m).Should().Be(600_000m);
    }

    [Fact]
    public void CalculateLoanProgressPercent_ShouldClampTo100()
    {
        _sut.CalculateLoanProgressPercent(100m, 0m).Should().Be(100m);
        _sut.CalculateLoanProgressPercent(0m, 0m).Should().Be(0m);
    }

    [Fact]
    public void CalculateEmiProgressPercent_ShouldReflectRemainingTenure()
    {
        _sut.CalculateEmiProgressPercent(100, 25).Should().Be(75m);
    }

    [Fact]
    public void EstimatePrepayment_WhenLumpClearsBalance_ShouldZeroRemaining()
    {
        var estimate = _sut.EstimatePrepayment(
            outstandingBalance: 50_000m,
            emiAmount: 10_000m,
            annualRatePercent: 12m,
            remainingTenureMonths: 6,
            lumpSum: 50_000m);

        estimate.NewOutstanding.Should().Be(0m);
        estimate.EstimatedRemainingMonths.Should().Be(0);
        estimate.MonthsSaved.Should().Be(6);
    }

    [Fact]
    public void CalculateOutstandingAfterPrincipalPayment_ShouldNotGoNegative()
    {
        _sut.CalculateOutstandingAfterPrincipalPayment(1_000m, 2_000m).Should().Be(0m);
    }
}

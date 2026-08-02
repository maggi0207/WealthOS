using WealthOS.Domain.Investments.Enums;
using WealthOS.Domain.Investments.Models;

namespace WealthOS.Application.Investments.Calculations;

/// <summary>
/// Pure portfolio math helpers (no I/O). XIRR is a documented placeholder.
/// </summary>
public interface IInvestmentCalculationService
{
    Portfolio BuildPortfolio(
        decimal investedAmount,
        decimal currentValue,
        decimal todaysGain,
        int accountCount,
        int holdingCount,
        string currencyCode = "INR",
        decimal? xirrPlaceholder = 14.6m);

    PortfolioSummary BuildSummary(
        decimal investedAmount,
        decimal currentValue,
        decimal todaysGain,
        int accountCount,
        int holdingCount,
        string? largestHoldingName,
        decimal? largestHoldingValue,
        string currencyCode = "INR",
        decimal xirrPlaceholder = 14.6m);

    AssetAllocation BuildAllocation(
        IEnumerable<(InvestmentCategory Category, decimal Value)> holdings,
        string currencyCode = "INR");

    InvestmentPerformance BuildPerformance(
        PerformanceRange range,
        IReadOnlyList<PerformancePoint> points,
        decimal investedAmount,
        decimal currentValue,
        string currencyCode = "INR");

    /// <summary>
    /// Placeholder XIRR — returns the supplied stub value without computing IRR.
    /// </summary>
    decimal? CalculateXirrPlaceholder(decimal? stubPercent = 14.6m);

    decimal RoundMoney(decimal value);

    decimal RoundPercent(decimal value);
}

/// <summary>
/// Investment portfolio calculation service.
/// </summary>
public sealed class InvestmentCalculationService : IInvestmentCalculationService
{
    public Portfolio BuildPortfolio(
        decimal investedAmount,
        decimal currentValue,
        decimal todaysGain,
        int accountCount,
        int holdingCount,
        string currencyCode = "INR",
        decimal? xirrPlaceholder = 14.6m)
    {
        var overallGain = RoundMoney(currentValue - investedAmount);
        var absoluteReturn = investedAmount == 0m
            ? 0m
            : RoundPercent(overallGain / investedAmount * 100m);
        var todaysGainPercent = currentValue == 0m
            ? 0m
            : RoundPercent(todaysGain / (currentValue - todaysGain) * 100m);

        if (currentValue - todaysGain == 0m)
        {
            todaysGainPercent = 0m;
        }

        return new Portfolio
        {
            InvestedAmount = RoundMoney(investedAmount),
            CurrentValue = RoundMoney(currentValue),
            TodaysGain = RoundMoney(todaysGain),
            TodaysGainPercent = todaysGainPercent,
            OverallGain = overallGain,
            AbsoluteReturnPercent = absoluteReturn,
            XirrPercent = CalculateXirrPlaceholder(xirrPlaceholder),
            AccountCount = accountCount,
            HoldingCount = holdingCount,
            CurrencyCode = currencyCode,
        };
    }

    public PortfolioSummary BuildSummary(
        decimal investedAmount,
        decimal currentValue,
        decimal todaysGain,
        int accountCount,
        int holdingCount,
        string? largestHoldingName,
        decimal? largestHoldingValue,
        string currencyCode = "INR",
        decimal xirrPlaceholder = 14.6m)
    {
        var portfolio = BuildPortfolio(
            investedAmount,
            currentValue,
            todaysGain,
            accountCount,
            holdingCount,
            currencyCode,
            xirrPlaceholder);

        return new PortfolioSummary
        {
            PortfolioValue = portfolio.CurrentValue,
            InvestedAmount = portfolio.InvestedAmount,
            TodaysGain = portfolio.TodaysGain,
            TodaysGainPercent = portfolio.TodaysGainPercent,
            TotalReturn = portfolio.OverallGain,
            AbsoluteReturnPercent = portfolio.AbsoluteReturnPercent,
            XirrPlaceholderPercent = xirrPlaceholder,
            AccountCount = accountCount,
            HoldingCount = holdingCount,
            LargestHoldingName = largestHoldingName,
            LargestHoldingValue = largestHoldingValue.HasValue ? RoundMoney(largestHoldingValue.Value) : null,
            CurrencyCode = currencyCode,
        };
    }

    public AssetAllocation BuildAllocation(
        IEnumerable<(InvestmentCategory Category, decimal Value)> holdings,
        string currencyCode = "INR")
    {
        var grouped = holdings
            .GroupBy(x => x.Category)
            .Select(g => (Category: g.Key, Value: RoundMoney(g.Sum(x => x.Value))))
            .Where(x => x.Value > 0m)
            .OrderByDescending(x => x.Value)
            .ToList();

        var total = RoundMoney(grouped.Sum(x => x.Value));

        var slices = grouped.Select(x => new AssetAllocationSlice
        {
            Category = x.Category,
            CategoryName = FormatCategory(x.Category),
            Value = x.Value,
            WeightPercent = total == 0m ? 0m : RoundPercent(x.Value / total * 100m),
        }).ToList();

        return new AssetAllocation
        {
            TotalValue = total,
            Slices = slices,
            CurrencyCode = currencyCode,
        };
    }

    public InvestmentPerformance BuildPerformance(
        PerformanceRange range,
        IReadOnlyList<PerformancePoint> points,
        decimal investedAmount,
        decimal currentValue,
        string currencyCode = "INR")
    {
        var absoluteReturn = investedAmount == 0m
            ? 0m
            : RoundPercent((currentValue - investedAmount) / investedAmount * 100m);

        return new InvestmentPerformance
        {
            Range = range,
            Points = points,
            AbsoluteReturnPercent = absoluteReturn,
            XirrPercent = CalculateXirrPlaceholder(),
            CurrencyCode = currencyCode,
        };
    }

    public decimal? CalculateXirrPlaceholder(decimal? stubPercent = 14.6m) => stubPercent;

    public decimal RoundMoney(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

    public decimal RoundPercent(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

    private static string FormatCategory(InvestmentCategory category) =>
        category switch
        {
            InvestmentCategory.Stocks => "Stocks",
            InvestmentCategory.MutualFunds => "Mutual Funds",
            InvestmentCategory.CorporateBonds => "Corporate Bonds",
            InvestmentCategory.GoldEtfs => "Gold ETFs",
            InvestmentCategory.Cash => "Cash",
            _ => "Other",
        };
}

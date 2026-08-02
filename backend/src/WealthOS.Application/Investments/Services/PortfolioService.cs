using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Investments.Calculations;
using WealthOS.Application.Investments.DTOs.Responses;
using WealthOS.Application.Investments.Interfaces;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Domain.Investments.Models;
using WealthOS.Domain.Investments.Repositories;

namespace WealthOS.Application.Investments.Services;

/// <summary>
/// Portfolio aggregation and performance series.
/// </summary>
public sealed class PortfolioService : IPortfolioService
{
    private readonly IHoldingRepository _holdingRepository;
    private readonly IInvestmentAccountRepository _accountRepository;
    private readonly IPortfolioSnapshotRepository _snapshotRepository;
    private readonly IInvestmentCalculationService _calculator;
    private readonly ICurrentUserService _currentUser;

    public PortfolioService(
        IHoldingRepository holdingRepository,
        IInvestmentAccountRepository accountRepository,
        IPortfolioSnapshotRepository snapshotRepository,
        IInvestmentCalculationService calculator,
        ICurrentUserService currentUser)
    {
        _holdingRepository = holdingRepository;
        _accountRepository = accountRepository;
        _snapshotRepository = snapshotRepository;
        _calculator = calculator;
        _currentUser = currentUser;
    }

    public async Task<Result<PortfolioResponse>> GetPortfolioAsync(
        Guid? accountId = null,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<PortfolioResponse>(userResult.Error!);
        }

        var holdings = await _holdingRepository.ListAllForUserAsync(userResult.Value, cancellationToken);
        if (accountId.HasValue)
        {
            holdings = holdings.Where(h => h.AccountId == accountId.Value).ToList();
        }

        var accountCount = accountId.HasValue
            ? (holdings.Count > 0 ? 1 : 0)
            : await _accountRepository.CountForUserAsync(userResult.Value, cancellationToken);

        var portfolio = _calculator.BuildPortfolio(
            holdings.Sum(h => h.InvestedAmount),
            holdings.Sum(h => h.CurrentValue),
            holdings.Sum(h => h.DayChange),
            accountCount,
            holdings.Count);

        return Result.Success(new PortfolioResponse
        {
            InvestedAmount = portfolio.InvestedAmount,
            CurrentValue = portfolio.CurrentValue,
            TodaysGain = portfolio.TodaysGain,
            TodaysGainPercent = portfolio.TodaysGainPercent,
            OverallGain = portfolio.OverallGain,
            AbsoluteReturnPercent = portfolio.AbsoluteReturnPercent,
            XirrPercent = portfolio.XirrPercent,
            AccountCount = portfolio.AccountCount,
            HoldingCount = portfolio.HoldingCount,
            CurrencyCode = portfolio.CurrencyCode,
        });
    }

    public async Task<Result<PortfolioSummaryResponse>> GetPortfolioSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<PortfolioSummaryResponse>(userResult.Error!);
        }

        var holdings = await _holdingRepository.ListAllForUserAsync(userResult.Value, cancellationToken);
        var accountCount = await _accountRepository.CountForUserAsync(userResult.Value, cancellationToken);
        var largest = holdings.OrderByDescending(h => h.CurrentValue).FirstOrDefault();

        var summary = _calculator.BuildSummary(
            holdings.Sum(h => h.InvestedAmount),
            holdings.Sum(h => h.CurrentValue),
            holdings.Sum(h => h.DayChange),
            accountCount,
            holdings.Count,
            largest?.Name,
            largest?.CurrentValue);

        return Result.Success(new PortfolioSummaryResponse
        {
            PortfolioValue = summary.PortfolioValue,
            InvestedAmount = summary.InvestedAmount,
            TodaysGain = summary.TodaysGain,
            TodaysGainPercent = summary.TodaysGainPercent,
            TotalReturn = summary.TotalReturn,
            AbsoluteReturnPercent = summary.AbsoluteReturnPercent,
            XirrPlaceholderPercent = summary.XirrPlaceholderPercent,
            AccountCount = summary.AccountCount,
            HoldingCount = summary.HoldingCount,
            LargestHoldingName = summary.LargestHoldingName,
            LargestHoldingValue = summary.LargestHoldingValue,
            CurrencyCode = summary.CurrencyCode,
        });
    }

    public async Task<Result<InvestmentPerformanceResponse>> GetPerformanceAsync(
        PerformanceRange range = PerformanceRange.OneYear,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<InvestmentPerformanceResponse>(userResult.Error!);
        }

        var holdings = await _holdingRepository.ListAllForUserAsync(userResult.Value, cancellationToken);
        var invested = holdings.Sum(h => h.InvestedAmount);
        var current = holdings.Sum(h => h.CurrentValue);

        var snapshots = await _snapshotRepository.ListRecentForUserAsync(userResult.Value, 24, cancellationToken);
        var points = BuildPoints(range, snapshots, current);

        var performance = _calculator.BuildPerformance(range, points, invested, current);

        return Result.Success(new InvestmentPerformanceResponse
        {
            Range = performance.Range,
            Points = performance.Points.Select(p => new PerformancePointResponse
            {
                Label = p.Label,
                Value = p.Value,
            }).ToList(),
            AbsoluteReturnPercent = performance.AbsoluteReturnPercent,
            XirrPercent = performance.XirrPercent,
            CurrencyCode = performance.CurrencyCode,
        });
    }

    private static IReadOnlyList<PerformancePoint> BuildPoints(
        PerformanceRange range,
        IReadOnlyList<Domain.Investments.Entities.PortfolioSnapshot> snapshots,
        decimal currentValue)
    {
        // Prefer seeded / stored snapshots when available; otherwise fall back to frontend-aligned stubs.
        if (snapshots.Count >= 2)
        {
            var ordered = snapshots.OrderBy(s => s.SnapshotDate).ToList();
            var take = range switch
            {
                PerformanceRange.OneMonth => Math.Min(5, ordered.Count),
                PerformanceRange.SixMonths => Math.Min(6, ordered.Count),
                PerformanceRange.OneYear => Math.Min(7, ordered.Count),
                _ => ordered.Count,
            };

            return ordered
                .TakeLast(take)
                .Select(s => new PerformancePoint
                {
                    Label = s.SnapshotDate.ToString("MMM yyyy"),
                    Value = Math.Round(s.CurrentValue / 100_000m, 1, MidpointRounding.AwayFromZero),
                })
                .ToList();
        }

        return GetStubSeries(range, currentValue);
    }

    private static IReadOnlyList<PerformancePoint> GetStubSeries(PerformanceRange range, decimal currentValue)
    {
        var nowLakhs = Math.Round(currentValue / 100_000m, 1, MidpointRounding.AwayFromZero);
        return range switch
        {
            PerformanceRange.OneMonth =>
            [
                new PerformancePoint { Label = "W1", Value = nowLakhs - 4.0m },
                new PerformancePoint { Label = "W2", Value = nowLakhs - 3.1m },
                new PerformancePoint { Label = "W3", Value = nowLakhs - 1.4m },
                new PerformancePoint { Label = "W4", Value = nowLakhs - 0.8m },
                new PerformancePoint { Label = "Now", Value = nowLakhs },
            ],
            PerformanceRange.SixMonths =>
            [
                new PerformancePoint { Label = "Dec", Value = nowLakhs - 18.8m },
                new PerformancePoint { Label = "Jan", Value = nowLakhs - 15.3m },
                new PerformancePoint { Label = "Feb", Value = nowLakhs - 13.0m },
                new PerformancePoint { Label = "Mar", Value = nowLakhs - 8.6m },
                new PerformancePoint { Label = "Apr", Value = nowLakhs - 4.5m },
                new PerformancePoint { Label = "May", Value = nowLakhs },
            ],
            PerformanceRange.OneYear =>
            [
                new PerformancePoint { Label = "Jun", Value = nowLakhs - 34.8m },
                new PerformancePoint { Label = "Aug", Value = nowLakhs - 29.1m },
                new PerformancePoint { Label = "Oct", Value = nowLakhs - 24.6m },
                new PerformancePoint { Label = "Dec", Value = nowLakhs - 18.8m },
                new PerformancePoint { Label = "Feb", Value = nowLakhs - 13.0m },
                new PerformancePoint { Label = "Apr", Value = nowLakhs - 4.5m },
                new PerformancePoint { Label = "Jun", Value = nowLakhs },
            ],
            _ =>
            [
                new PerformancePoint { Label = "2021", Value = 42.0m },
                new PerformancePoint { Label = "2022", Value = 61.5m },
                new PerformancePoint { Label = "2023", Value = 84.2m },
                new PerformancePoint { Label = "2024", Value = 108.9m },
                new PerformancePoint { Label = "2025", Value = 131.4m },
                new PerformancePoint { Label = "2026", Value = nowLakhs },
            ],
        };
    }

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }
}

using WealthOS.Domain.Goals.Enums;
using WealthOS.Domain.Goals.Models;

namespace WealthOS.Application.Goals.Calculations;

/// <summary>
/// Pure goal math helpers (no I/O).
/// </summary>
public interface IGoalCalculationService
{
    decimal CalculateCompletionPercent(decimal currentAmount, decimal targetAmount);

    decimal CalculateRemainingAmount(decimal currentAmount, decimal targetAmount);

    int CalculateMonthsRemaining(DateOnly from, DateOnly targetDate);

    decimal CalculateMonthlyRequiredContribution(
        decimal currentAmount,
        decimal targetAmount,
        DateOnly from,
        DateOnly targetDate);

    DateOnly? EstimateCompletionDate(
        decimal currentAmount,
        decimal targetAmount,
        decimal monthlyContribution,
        DateOnly from);

    ProgressTrend DetermineTrend(
        decimal currentAmount,
        decimal targetAmount,
        decimal monthlyContribution,
        DateOnly from,
        DateOnly targetDate,
        GoalStatus status);

    GoalProgress BuildProgress(
        Guid goalId,
        string goalName,
        decimal targetAmount,
        decimal currentAmount,
        decimal monthlyContribution,
        DateOnly targetDate,
        GoalStatus status,
        DateOnly asOf,
        string currencyCode = "INR");

    GoalProjection BuildProjection(
        Guid goalId,
        string goalName,
        decimal targetAmount,
        decimal currentAmount,
        decimal monthlyContribution,
        DateOnly targetDate,
        DateOnly asOf,
        string currencyCode = "INR",
        int maxPoints = 12);

    decimal RoundMoney(decimal value);

    decimal RoundPercent(decimal value);
}

/// <summary>
/// Goal progress and projection calculation service.
/// </summary>
public sealed class GoalCalculationService : IGoalCalculationService
{
    public decimal CalculateCompletionPercent(decimal currentAmount, decimal targetAmount)
    {
        if (targetAmount <= 0m)
        {
            return 0m;
        }

        var percent = currentAmount / targetAmount * 100m;
        return RoundPercent(Math.Min(100m, Math.Max(0m, percent)));
    }

    public decimal CalculateRemainingAmount(decimal currentAmount, decimal targetAmount) =>
        RoundMoney(Math.Max(0m, targetAmount - currentAmount));

    public int CalculateMonthsRemaining(DateOnly from, DateOnly targetDate)
    {
        var months = (targetDate.Year - from.Year) * 12 + (targetDate.Month - from.Month);
        if (targetDate.Day < from.Day)
        {
            months--;
        }

        return Math.Max(0, months);
    }

    public decimal CalculateMonthlyRequiredContribution(
        decimal currentAmount,
        decimal targetAmount,
        DateOnly from,
        DateOnly targetDate)
    {
        var remaining = CalculateRemainingAmount(currentAmount, targetAmount);
        var months = CalculateMonthsRemaining(from, targetDate);
        if (months == 0)
        {
            return remaining;
        }

        return RoundMoney(remaining / months);
    }

    public DateOnly? EstimateCompletionDate(
        decimal currentAmount,
        decimal targetAmount,
        decimal monthlyContribution,
        DateOnly from)
    {
        var remaining = CalculateRemainingAmount(currentAmount, targetAmount);
        if (remaining <= 0m)
        {
            return from;
        }

        if (monthlyContribution <= 0m)
        {
            return null;
        }

        var monthsNeeded = (int)Math.Ceiling(remaining / monthlyContribution);
        return from.AddMonths(monthsNeeded);
    }

    public ProgressTrend DetermineTrend(
        decimal currentAmount,
        decimal targetAmount,
        decimal monthlyContribution,
        DateOnly from,
        DateOnly targetDate,
        GoalStatus status)
    {
        if (status == GoalStatus.Completed || currentAmount >= targetAmount)
        {
            return ProgressTrend.Completed;
        }

        var required = CalculateMonthlyRequiredContribution(currentAmount, targetAmount, from, targetDate);
        if (required <= 0m)
        {
            return ProgressTrend.OnTrack;
        }

        if (monthlyContribution <= 0m)
        {
            return ProgressTrend.Behind;
        }

        var ratio = monthlyContribution / required;
        if (ratio >= 1.05m)
        {
            return ProgressTrend.Ahead;
        }

        if (ratio < 0.95m)
        {
            return ProgressTrend.Behind;
        }

        return ProgressTrend.OnTrack;
    }

    public GoalProgress BuildProgress(
        Guid goalId,
        string goalName,
        decimal targetAmount,
        decimal currentAmount,
        decimal monthlyContribution,
        DateOnly targetDate,
        GoalStatus status,
        DateOnly asOf,
        string currencyCode = "INR")
    {
        return new GoalProgress
        {
            GoalId = goalId,
            GoalName = goalName,
            TargetAmount = RoundMoney(targetAmount),
            CurrentAmount = RoundMoney(currentAmount),
            RemainingAmount = CalculateRemainingAmount(currentAmount, targetAmount),
            CompletionPercent = CalculateCompletionPercent(currentAmount, targetAmount),
            MonthlyContribution = RoundMoney(monthlyContribution),
            MonthlyRequiredContribution = CalculateMonthlyRequiredContribution(
                currentAmount,
                targetAmount,
                asOf,
                targetDate),
            EstimatedCompletionDate = EstimateCompletionDate(
                currentAmount,
                targetAmount,
                monthlyContribution,
                asOf),
            TargetDate = targetDate,
            Trend = DetermineTrend(
                currentAmount,
                targetAmount,
                monthlyContribution,
                asOf,
                targetDate,
                status),
            MonthsRemaining = CalculateMonthsRemaining(asOf, targetDate),
            CurrencyCode = currencyCode,
        };
    }

    public GoalProjection BuildProjection(
        Guid goalId,
        string goalName,
        decimal targetAmount,
        decimal currentAmount,
        decimal monthlyContribution,
        DateOnly targetDate,
        DateOnly asOf,
        string currencyCode = "INR",
        int maxPoints = 12)
    {
        var months = CalculateMonthsRemaining(asOf, targetDate);
        var projectedAtTarget = RoundMoney(currentAmount + monthlyContribution * months);
        var shortfall = RoundMoney(Math.Max(0m, targetAmount - projectedAtTarget));
        var estimated = EstimateCompletionDate(currentAmount, targetAmount, monthlyContribution, asOf);
        var required = CalculateMonthlyRequiredContribution(currentAmount, targetAmount, asOf, targetDate);

        var points = new List<GoalProjectionPoint>
        {
            new()
            {
                AsOf = asOf,
                Label = asOf.ToString("MMM yyyy"),
                ProjectedAmount = RoundMoney(currentAmount),
            },
        };

        var monthsToProject = Math.Min(Math.Max(months, 1), maxPoints);
        var running = currentAmount;
        for (var i = 1; i <= monthsToProject; i++)
        {
            running = RoundMoney(running + monthlyContribution);
            if (running > targetAmount)
            {
                running = RoundMoney(targetAmount);
            }

            var pointDate = asOf.AddMonths(i);
            points.Add(new GoalProjectionPoint
            {
                AsOf = pointDate,
                Label = pointDate.ToString("MMM yyyy"),
                ProjectedAmount = running,
            });

            if (running >= targetAmount)
            {
                break;
            }
        }

        return new GoalProjection
        {
            GoalId = goalId,
            GoalName = goalName,
            TargetAmount = RoundMoney(targetAmount),
            CurrentAmount = RoundMoney(currentAmount),
            MonthlyContribution = RoundMoney(monthlyContribution),
            TargetDate = targetDate,
            EstimatedCompletionDate = estimated,
            MonthlyRequiredContribution = required,
            ProjectedAmountAtTargetDate = projectedAtTarget > targetAmount
                ? RoundMoney(targetAmount)
                : projectedAtTarget,
            ShortfallAtTargetDate = shortfall,
            IsOnTrack = shortfall <= 0m,
            Points = points,
            CurrencyCode = currencyCode,
        };
    }

    public decimal RoundMoney(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

    public decimal RoundPercent(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}

using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.Interfaces;
using WealthOS.Application.Goals.Interfaces;
using WealthOS.Application.Income.Interfaces;
using WealthOS.Application.Loans.Interfaces;
using WealthOS.Application.Reports.DTOs.Requests;
using WealthOS.Application.Reports.DTOs.Responses;
using WealthOS.Application.Reports.Interfaces;
using WealthOS.Domain.Reports.Enums;
using WealthOS.Domain.Reports.Models;

namespace WealthOS.Application.Reports.Services;

/// <summary>
/// Computes a composite financial health score from module summaries (no owned business data).
/// </summary>
public sealed class FinancialHealthService : IFinancialHealthService
{
    private readonly IDashboardService _dashboardService;
    private readonly ILoanService _loanService;
    private readonly IIncomeService _incomeService;
    private readonly IGoalService _goalService;
    private readonly ICurrentUserService _currentUser;

    public FinancialHealthService(
        IDashboardService dashboardService,
        ILoanService loanService,
        IIncomeService incomeService,
        IGoalService goalService,
        ICurrentUserService currentUser)
    {
        _dashboardService = dashboardService;
        _loanService = loanService;
        _incomeService = incomeService;
        _goalService = goalService;
        _currentUser = currentUser;
    }

    public async Task<Result<FinancialHealthResponse>> GetFinancialHealthAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<FinancialHealthResponse>(Error.Unauthorized());
        }

        var dashboard = await _dashboardService.GetSummaryAsync(cancellationToken);
        if (dashboard.IsFailure)
        {
            return Result.Failure<FinancialHealthResponse>(dashboard.Error!);
        }

        var loans = await _loanService.GetSummaryAsync(cancellationToken);
        var pnl = await _incomeService.GetProfitLossAsync(filters?.PeriodLabel, cancellationToken);
        var goals = await _goalService.GetDashboardAsync(cancellationToken);
        var cashFlow = await _incomeService.GetCashFlowAsync(filters?.PeriodLabel, cancellationToken);

        var assets = dashboard.Value.AssetValue;
        var liabilities = loans.IsSuccess ? loans.Value.OutstandingBalance : dashboard.Value.LoanBalance;
        var debtRatio = assets > 0
            ? Math.Round(liabilities / assets * 100m, 2)
            : (liabilities > 0 ? 100m : 0m);

        var savingsRate = pnl.IsSuccess ? pnl.Value.SavingsRatePercent : 0m;
        var goalProgress = goals.IsSuccess ? goals.Value.OverallProgressPercent : 0m;
        var netCashFlow = cashFlow.IsSuccess ? cashFlow.Value.NetCashFlow : 0m;

        var debtScore = ScoreDebtRatio(debtRatio);
        var savingsScore = ScoreSavingsRate(savingsRate);
        var goalScore = ScoreGoalProgress(goalProgress);
        var liquidityScore = ScoreLiquidity(netCashFlow, dashboard.Value.MonthlyIncome);
        var netWorthScore = ScoreNetWorth(dashboard.Value.NetWorth, dashboard.Value.ChangePercent);

        // Weighted composite (out of 100).
        var composite = (int)Math.Round(
            debtScore * 0.25m +
            savingsScore * 0.25m +
            goalScore * 0.20m +
            liquidityScore * 0.15m +
            netWorthScore * 0.15m);

        composite = Math.Clamp(composite, 0, 100);
        var grade = ToGrade(composite);

        var factors = new List<FinancialHealthFactorResponse>
        {
            new()
            {
                Code = "debt_ratio",
                Label = "Debt Ratio",
                Score = debtScore,
                Weight = "25%",
                Detail = $"Debt-to-asset ratio {debtRatio:0.##}%",
            },
            new()
            {
                Code = "savings_rate",
                Label = "Savings Rate",
                Score = savingsScore,
                Weight = "25%",
                Detail = $"Savings rate {savingsRate:0.##}%",
            },
            new()
            {
                Code = "goal_progress",
                Label = "Goal Progress",
                Score = goalScore,
                Weight = "20%",
                Detail = $"Overall goal progress {goalProgress:0.##}%",
            },
            new()
            {
                Code = "liquidity",
                Label = "Liquidity",
                Score = liquidityScore,
                Weight = "15%",
                Detail = $"Net cash flow {netCashFlow:0.##}",
            },
            new()
            {
                Code = "net_worth",
                Label = "Net Worth Trend",
                Score = netWorthScore,
                Weight = "15%",
                Detail = $"Net worth change {dashboard.Value.ChangePercent:0.##}%",
            },
        };

        // Prefer dashboard health score when present as an additional signal.
        if (dashboard.Value.HealthScore.Score > 0)
        {
            composite = (int)Math.Round((composite + dashboard.Value.HealthScore.Score) / 2m);
            composite = Math.Clamp(composite, 0, 100);
            grade = ToGrade(composite);
        }

        var health = new FinancialHealth
        {
            Score = composite,
            Grade = grade,
            Summary = BuildSummary(composite, grade),
            CalculatedAt = DateTime.UtcNow,
            DebtToAssetRatioPercent = debtRatio,
            SavingsRatePercent = savingsRate,
            GoalProgressPercent = goalProgress,
            LiquidityScore = liquidityScore,
            Factors = factors.Select(factor => new FinancialHealthFactor
            {
                Code = factor.Code,
                Label = factor.Label,
                Score = factor.Score,
                Weight = factor.Weight,
                Detail = factor.Detail,
            }).ToList(),
        };

        return Result.Success(new FinancialHealthResponse
        {
            ReportType = ReportType.FinancialHealthScore,
            Title = "Financial Health Score",
            GeneratedAt = health.CalculatedAt,
            CurrencyCode = dashboard.Value.CurrencyCode,
            Filters = MapFilters(filters),
            DataSources = new[] { "Dashboard", "Loans", "Income", "Goals" },
            Score = health.Score,
            Grade = health.Grade,
            GradeLabel = FormatGrade(health.Grade),
            Summary = health.Summary,
            DebtToAssetRatioPercent = health.DebtToAssetRatioPercent,
            SavingsRatePercent = health.SavingsRatePercent,
            GoalProgressPercent = health.GoalProgressPercent,
            LiquidityScore = health.LiquidityScore,
            Factors = factors,
        });
    }

    private static int ScoreDebtRatio(decimal debtRatioPercent) =>
        debtRatioPercent switch
        {
            <= 20m => 100,
            <= 35m => 85,
            <= 50m => 70,
            <= 65m => 55,
            <= 80m => 40,
            _ => 20,
        };

    private static int ScoreSavingsRate(decimal savingsRatePercent) =>
        savingsRatePercent switch
        {
            >= 30m => 100,
            >= 20m => 85,
            >= 10m => 70,
            >= 5m => 55,
            >= 0m => 40,
            _ => 20,
        };

    private static int ScoreGoalProgress(decimal progressPercent) =>
        progressPercent switch
        {
            >= 80m => 100,
            >= 60m => 85,
            >= 40m => 70,
            >= 20m => 55,
            > 0m => 40,
            _ => 25,
        };

    private static int ScoreLiquidity(decimal netCashFlow, decimal monthlyIncome)
    {
        if (monthlyIncome <= 0)
        {
            return netCashFlow >= 0 ? 60 : 30;
        }

        var ratio = netCashFlow / monthlyIncome;
        return ratio switch
        {
            >= 0.30m => 100,
            >= 0.15m => 85,
            >= 0.05m => 70,
            >= 0m => 55,
            >= -0.10m => 40,
            _ => 20,
        };
    }

    private static int ScoreNetWorth(decimal netWorth, decimal changePercent)
    {
        var baseScore = netWorth >= 0 ? 70 : 30;
        var trendBonus = changePercent switch
        {
            >= 10m => 30,
            >= 5m => 20,
            >= 0m => 10,
            >= -5m => 0,
            _ => -20,
        };

        return Math.Clamp(baseScore + trendBonus, 0, 100);
    }

    private static FinancialHealthGrade ToGrade(int score) =>
        score switch
        {
            >= 95 => FinancialHealthGrade.APlus,
            >= 85 => FinancialHealthGrade.A,
            >= 70 => FinancialHealthGrade.B,
            >= 55 => FinancialHealthGrade.C,
            >= 40 => FinancialHealthGrade.D,
            _ => FinancialHealthGrade.F,
        };

    private static string FormatGrade(FinancialHealthGrade grade) =>
        grade switch
        {
            FinancialHealthGrade.APlus => "A+",
            _ => grade.ToString(),
        };

    private static string BuildSummary(int score, FinancialHealthGrade grade) =>
        $"Financial health score is {score}/100 (grade {FormatGrade(grade)}). " +
        "Score aggregates debt ratio, savings rate, goal progress, liquidity, and net-worth trend.";

    private static ReportFilterResponse? MapFilters(ReportFilterRequest? filters)
    {
        if (filters is null)
        {
            return null;
        }

        return new ReportFilterResponse
        {
            FromDate = filters.FromDate,
            ToDate = filters.ToDate,
            Category = filters.Category,
            Owner = filters.Owner,
            PropertyId = filters.PropertyId,
            InvestmentAccountId = filters.InvestmentAccountId,
            BusinessClientId = filters.BusinessClientId,
            GoalId = filters.GoalId,
            LoanId = filters.LoanId,
            Period = filters.Period,
            PeriodLabel = filters.PeriodLabel,
        };
    }
}

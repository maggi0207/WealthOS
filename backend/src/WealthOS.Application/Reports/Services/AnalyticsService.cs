using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.Interfaces;
using WealthOS.Application.Goals.Interfaces;
using WealthOS.Application.Income.Interfaces;
using WealthOS.Application.Investments.Interfaces;
using WealthOS.Application.Loans.Interfaces;
using WealthOS.Application.Reports.DTOs.Requests;
using WealthOS.Application.Reports.DTOs.Responses;
using WealthOS.Application.Reports.Interfaces;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Domain.Reports.Enums;

namespace WealthOS.Application.Reports.Services;

/// <summary>
/// Builds cross-module analytics KPIs from Application interfaces only.
/// </summary>
public sealed class AnalyticsService : IAnalyticsService
{
    private readonly IDashboardService _dashboardService;
    private readonly IIncomeService _incomeService;
    private readonly IPortfolioService _portfolioService;
    private readonly ILoanService _loanService;
    private readonly IGoalService _goalService;
    private readonly ICurrentUserService _currentUser;

    public AnalyticsService(
        IDashboardService dashboardService,
        IIncomeService incomeService,
        IPortfolioService portfolioService,
        ILoanService loanService,
        IGoalService goalService,
        ICurrentUserService currentUser)
    {
        _dashboardService = dashboardService;
        _incomeService = incomeService;
        _portfolioService = portfolioService;
        _loanService = loanService;
        _goalService = goalService;
        _currentUser = currentUser;
    }

    public async Task<Result<AnalyticsSummaryResponse>> GetSummaryAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<AnalyticsSummaryResponse>(Error.Unauthorized());
        }

        var netWorth = await _dashboardService.GetNetWorthAsync(cancellationToken);
        if (netWorth.IsFailure)
        {
            return Result.Failure<AnalyticsSummaryResponse>(netWorth.Error!);
        }

        var months = filters?.Period == AnalyticsPeriod.Yearly ? 12 : 6;
        var incomeTrend = await _incomeService.GetMonthlyIncomeAsync(months, cancellationToken);
        var cashFlow = await _incomeService.GetCashFlowAsync(filters?.PeriodLabel, cancellationToken);
        var pnl = await _incomeService.GetProfitLossAsync(filters?.PeriodLabel, cancellationToken);
        var portfolio = await _portfolioService.GetPortfolioSummaryAsync(cancellationToken);
        var performance = await _portfolioService.GetPerformanceAsync(
            PerformanceRange.OneYear,
            cancellationToken);
        var loans = await _loanService.GetSummaryAsync(cancellationToken);
        var goals = await _goalService.GetDashboardAsync(cancellationToken);

        var assets = netWorth.Value.AssetValue;
        var debtRatio = assets > 0 && loans.IsSuccess
            ? Math.Round(loans.Value.OutstandingBalance / assets * 100m, 2)
            : 0m;

        var monthlyIncomePoints = incomeTrend.IsSuccess
            ? incomeTrend.Value.Points.Select(point => new ReportTrendPointResponse
            {
                Label = point.Label,
                Value = point.Salary + point.Business,
            }).ToList()
            : new List<ReportTrendPointResponse>();

        var cashFlowTrend = monthlyIncomePoints
            .Select(point => new ReportTrendPointResponse
            {
                Label = point.Label,
                Value = point.Value - (cashFlow.IsSuccess
                    ? (cashFlow.Value.DeveloperPayroll + cashFlow.Value.BusinessExpenses + cashFlow.Value.PersonalOutflow) / Math.Max(months, 1)
                    : 0m),
            })
            .ToList();

        var netWorthTrend = monthlyIncomePoints
            .Select((point, index) => new ReportTrendPointResponse
            {
                Label = point.Label,
                Value = Math.Round(
                    netWorth.Value.NetWorth * (1m + (netWorth.Value.ChangePercent / 100m) * ((index + 1m) / Math.Max(monthlyIncomePoints.Count, 1))),
                    2),
            })
            .ToList();

        if (netWorthTrend.Count == 0)
        {
            netWorthTrend.Add(new ReportTrendPointResponse
            {
                Label = "Current",
                Value = netWorth.Value.NetWorth,
                AsOf = DateTime.UtcNow,
            });
        }

        var response = new AnalyticsSummaryResponse
        {
            ReportType = ReportType.AnalyticsSummary,
            Title = "Analytics Summary",
            GeneratedAt = DateTime.UtcNow,
            CurrencyCode = netWorth.Value.CurrencyCode,
            Filters = MapFilters(filters),
            DataSources = new[] { "Dashboard", "Income", "Investments", "Loans", "Goals" },
            NetWorth = netWorth.Value.NetWorth,
            NetWorthGrowthPercent = netWorth.Value.ChangePercent,
            SavingsRatePercent = pnl.IsSuccess ? pnl.Value.SavingsRatePercent : 0m,
            InvestmentReturnPercent = portfolio.IsSuccess
                ? portfolio.Value.AbsoluteReturnPercent
                : performance.IsSuccess ? performance.Value.AbsoluteReturnPercent : 0m,
            BusinessProfit = pnl.IsSuccess ? pnl.Value.NetProfit : 0m,
            DebtRatioPercent = debtRatio,
            NetWorthTrend = netWorthTrend,
            CashFlowTrend = cashFlowTrend,
            MonthlyIncomeTrend = monthlyIncomePoints,
        };

        _ = goals; // Goals available for future weighted analytics extensions.

        return Result.Success(response);
    }

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

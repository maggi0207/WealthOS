using System.Diagnostics;
using System.Text.Json;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.Interfaces;
using WealthOS.Application.Dashboard.Providers;
using WealthOS.Application.Documents.Interfaces;
using WealthOS.Application.Goals.Interfaces;
using WealthOS.Application.Income.Interfaces;
using WealthOS.Application.Investments.Interfaces;
using WealthOS.Application.Loans.Interfaces;
using WealthOS.Application.Notifications.Interfaces;
using WealthOS.Application.Properties.Interfaces;
using WealthOS.Application.Reports.DTOs.Requests;
using WealthOS.Application.Reports.DTOs.Responses;
using WealthOS.Application.Reports.Interfaces;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Domain.Reports.Entities;
using WealthOS.Domain.Reports.Enums;
using WealthOS.Domain.Reports.Repositories;

namespace WealthOS.Application.Reports.Services;

/// <summary>
/// Aggregates report payloads exclusively via Application-layer module interfaces/providers.
/// Reports owns no business data repositories outside its own metadata (snapshots/executions).
/// </summary>
public sealed class ReportService : IReportService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    private readonly IDashboardService _dashboardService;
    private readonly IPropertyService _propertyService;
    private readonly ILoanService _loanService;
    private readonly IIncomeService _incomeService;
    private readonly IInvestmentService _investmentService;
    private readonly IPortfolioService _portfolioService;
    private readonly IAllocationService _allocationService;
    private readonly IGoalService _goalService;
    private readonly IDocumentSummaryProvider _documentSummaryProvider;
    private readonly IDocumentSearchService _documentSearchService;
    private readonly INotificationService _notificationService;
    private readonly IReportDefinitionRepository _definitionRepository;
    private readonly IReportExecutionRepository _executionRepository;
    private readonly IReportSnapshotRepository _snapshotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public ReportService(
        IDashboardService dashboardService,
        IPropertyService propertyService,
        ILoanService loanService,
        IIncomeService incomeService,
        IInvestmentService investmentService,
        IPortfolioService portfolioService,
        IAllocationService allocationService,
        IGoalService goalService,
        IDocumentSummaryProvider documentSummaryProvider,
        IDocumentSearchService documentSearchService,
        INotificationService notificationService,
        IReportDefinitionRepository definitionRepository,
        IReportExecutionRepository executionRepository,
        IReportSnapshotRepository snapshotRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _dashboardService = dashboardService;
        _propertyService = propertyService;
        _loanService = loanService;
        _incomeService = incomeService;
        _investmentService = investmentService;
        _portfolioService = portfolioService;
        _allocationService = allocationService;
        _goalService = goalService;
        _documentSummaryProvider = documentSummaryProvider;
        _documentSearchService = documentSearchService;
        _notificationService = notificationService;
        _definitionRepository = definitionRepository;
        _executionRepository = executionRepository;
        _snapshotRepository = snapshotRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<NetWorthReportResponse>> GetNetWorthReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<NetWorthReportResponse>(userResult.Error!);
        }

        var stopwatch = Stopwatch.StartNew();
        var netWorth = await _dashboardService.GetNetWorthAsync(cancellationToken);
        if (netWorth.IsFailure)
        {
            return Result.Failure<NetWorthReportResponse>(netWorth.Error!);
        }

        var dashboard = await _dashboardService.GetSummaryAsync(cancellationToken);
        var property = await _propertyService.GetSummaryAsync(cancellationToken);
        var investment = await _investmentService.GetDashboardSummaryAsync(cancellationToken);
        var loan = await _loanService.GetSummaryAsync(cancellationToken);

        var response = new NetWorthReportResponse
        {
            ReportType = ReportType.NetWorth,
            Title = "Net Worth Report",
            GeneratedAt = DateTime.UtcNow,
            CurrencyCode = netWorth.Value.CurrencyCode,
            Filters = MapFilters(filters),
            DataSources = new[] { "Dashboard", "Properties", "Investments", "Loans" },
            NetWorth = netWorth.Value.NetWorth,
            AssetValue = netWorth.Value.AssetValue,
            LiabilityValue = netWorth.Value.LiabilityValue,
            ChangePercent = netWorth.Value.ChangePercent,
            PropertyValue = property.IsSuccess
                ? property.Value.TotalMarketValue
                : dashboard.IsSuccess ? dashboard.Value.PropertyValue : 0m,
            InvestmentValue = investment.IsSuccess
                ? investment.Value.PortfolioValue
                : dashboard.IsSuccess ? dashboard.Value.InvestmentValue : 0m,
            LoanBalance = loan.IsSuccess
                ? loan.Value.OutstandingBalance
                : dashboard.IsSuccess ? dashboard.Value.LoanBalance : 0m,
        };

        await PersistExecutionAsync(
            userResult.Value,
            ReportType.NetWorth,
            filters,
            response,
            stopwatch,
            cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<CashFlowReportResponse>> GetCashFlowReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<CashFlowReportResponse>(userResult.Error!);
        }

        var stopwatch = Stopwatch.StartNew();
        var period = filters?.PeriodLabel;
        var cashFlow = await _incomeService.GetCashFlowAsync(period, cancellationToken);
        if (cashFlow.IsFailure)
        {
            return Result.Failure<CashFlowReportResponse>(cashFlow.Error!);
        }

        var pnl = await _incomeService.GetProfitLossAsync(period, cancellationToken);
        var cf = cashFlow.Value;
        var totalIn = cf.SalaryIncome + cf.BusinessRevenue;
        var totalOut = cf.DeveloperPayroll + cf.BusinessExpenses + cf.PersonalOutflow;

        var response = new CashFlowReportResponse
        {
            ReportType = ReportType.CashFlow,
            Title = "Cash Flow Report",
            GeneratedAt = DateTime.UtcNow,
            CurrencyCode = cf.CurrencyCode,
            Filters = MapFilters(filters),
            DataSources = new[] { "Income" },
            Period = cf.PeriodLabel,
            SalaryIncome = cf.SalaryIncome,
            BusinessRevenue = cf.BusinessRevenue,
            TotalInflow = totalIn,
            DeveloperPayroll = cf.DeveloperPayroll,
            BusinessExpenses = cf.BusinessExpenses,
            PersonalOutflow = cf.PersonalOutflow,
            TotalOutflow = totalOut,
            NetCashFlow = cf.NetCashFlow,
            SavingsRatePercent = pnl.IsSuccess ? pnl.Value.SavingsRatePercent : 0m,
        };

        await PersistExecutionAsync(
            userResult.Value,
            ReportType.CashFlow,
            filters,
            response,
            stopwatch,
            cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<InvestmentReportResponse>> GetInvestmentReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<InvestmentReportResponse>(userResult.Error!);
        }

        var stopwatch = Stopwatch.StartNew();
        var accountId = filters?.InvestmentAccountId;
        var summary = await _portfolioService.GetPortfolioSummaryAsync(cancellationToken);
        if (summary.IsFailure)
        {
            return Result.Failure<InvestmentReportResponse>(summary.Error!);
        }

        var allocation = await _allocationService.GetAllocationAsync(accountId, cancellationToken);
        var performance = await _portfolioService.GetPerformanceAsync(
            PerformanceRange.OneYear,
            cancellationToken);

        var response = new InvestmentReportResponse
        {
            ReportType = ReportType.InvestmentPerformance,
            Title = "Investment Performance Report",
            GeneratedAt = DateTime.UtcNow,
            CurrencyCode = allocation.IsSuccess ? allocation.Value.CurrencyCode : "INR",
            Filters = MapFilters(filters),
            DataSources = new[] { "Investments" },
            PortfolioValue = summary.Value.PortfolioValue,
            InvestedAmount = summary.Value.InvestedAmount,
            TotalReturn = summary.Value.TotalReturn,
            AbsoluteReturnPercent = summary.Value.AbsoluteReturnPercent,
            XirrPercent = performance.IsSuccess ? performance.Value.XirrPercent : null,
            AccountCount = summary.Value.AccountCount,
            HoldingCount = summary.Value.HoldingCount,
            Allocation = allocation.IsSuccess
                ? allocation.Value.Slices.Select(slice => new ReportAllocationSliceResponse
                {
                    Category = slice.CategoryName,
                    Value = slice.Value,
                    Percent = slice.WeightPercent,
                }).ToList()
                : Array.Empty<ReportAllocationSliceResponse>(),
            PerformancePoints = performance.IsSuccess
                ? performance.Value.Points.Select(point => new ReportTrendPointResponse
                {
                    Label = point.Label,
                    Value = point.Value,
                }).ToList()
                : Array.Empty<ReportTrendPointResponse>(),
        };

        await PersistExecutionAsync(
            userResult.Value,
            ReportType.InvestmentPerformance,
            filters,
            response,
            stopwatch,
            cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<LoanReportResponse>> GetLoanReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<LoanReportResponse>(userResult.Error!);
        }

        var stopwatch = Stopwatch.StartNew();
        var loanSummary = await _loanService.GetSummaryAsync(cancellationToken);
        if (loanSummary.IsFailure)
        {
            return Result.Failure<LoanReportResponse>(loanSummary.Error!);
        }

        var netWorth = await _dashboardService.GetNetWorthAsync(cancellationToken);
        var assets = netWorth.IsSuccess && netWorth.Value.AssetValue > 0
            ? netWorth.Value.AssetValue
            : 0m;
        var debtRatio = assets > 0
            ? Math.Round(loanSummary.Value.OutstandingBalance / assets * 100m, 2)
            : 0m;

        var response = new LoanReportResponse
        {
            ReportType = ReportType.LoanAnalysis,
            Title = "Loan Analysis Report",
            GeneratedAt = DateTime.UtcNow,
            CurrencyCode = loanSummary.Value.CurrencyCode,
            Filters = MapFilters(filters),
            DataSources = new[] { "Loans", "Dashboard" },
            LoanCount = loanSummary.Value.LoanCount,
            TotalLoanAmount = loanSummary.Value.TotalLoanAmount,
            OutstandingBalance = loanSummary.Value.OutstandingBalance,
            MonthlyEmi = loanSummary.Value.MonthlyEmi,
            UpcomingEmi = loanSummary.Value.UpcomingEmi,
            DebtRatioPercent = debtRatio,
            ActiveCount = loanSummary.Value.ActiveCount,
            ClosedCount = loanSummary.Value.ClosedCount,
        };

        await PersistExecutionAsync(
            userResult.Value,
            ReportType.LoanAnalysis,
            filters,
            response,
            stopwatch,
            cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<PropertyReportResponse>> GetPropertyReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<PropertyReportResponse>(userResult.Error!);
        }

        var stopwatch = Stopwatch.StartNew();
        var propertySummary = await _propertyService.GetSummaryAsync(cancellationToken);
        if (propertySummary.IsFailure)
        {
            return Result.Failure<PropertyReportResponse>(propertySummary.Error!);
        }

        var response = new PropertyReportResponse
        {
            ReportType = ReportType.PropertyAppreciation,
            Title = "Property Appreciation Report",
            GeneratedAt = DateTime.UtcNow,
            CurrencyCode = propertySummary.Value.CurrencyCode,
            Filters = MapFilters(filters),
            DataSources = new[] { "Properties" },
            PropertyCount = propertySummary.Value.PropertyCount,
            TotalPurchasePrice = propertySummary.Value.TotalPurchasePrice,
            TotalMarketValue = propertySummary.Value.TotalMarketValue,
            TotalAppreciation = propertySummary.Value.TotalAppreciation,
            TotalAppreciationPercent = propertySummary.Value.TotalAppreciationPercent,
            ActiveCount = propertySummary.Value.ActiveCount,
            RentedCount = propertySummary.Value.RentedCount,
        };

        await PersistExecutionAsync(
            userResult.Value,
            ReportType.PropertyAppreciation,
            filters,
            response,
            stopwatch,
            cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<BusinessReportResponse>> GetBusinessReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<BusinessReportResponse>(userResult.Error!);
        }

        var stopwatch = Stopwatch.StartNew();
        var period = filters?.PeriodLabel;
        var pnl = await _incomeService.GetProfitLossAsync(period, cancellationToken);
        if (pnl.IsFailure)
        {
            return Result.Failure<BusinessReportResponse>(pnl.Error!);
        }

        var response = new BusinessReportResponse
        {
            ReportType = ReportType.BusinessProfitAndLoss,
            Title = "Business P&L Report",
            GeneratedAt = DateTime.UtcNow,
            CurrencyCode = "INR",
            Filters = MapFilters(filters),
            DataSources = new[] { "Income", "Business" },
            Period = pnl.Value.Period,
            BusinessRevenue = pnl.Value.BusinessRevenue,
            DeveloperCost = pnl.Value.DeveloperCost,
            BusinessExpenses = pnl.Value.BusinessExpenses,
            GrossProfit = pnl.Value.GrossProfit,
            NetProfit = pnl.Value.NetProfit,
            SalaryIncome = pnl.Value.SalaryIncome,
            TotalIncome = pnl.Value.TotalIncome,
            SavingsRatePercent = pnl.Value.SavingsRatePercent,
        };

        await PersistExecutionAsync(
            userResult.Value,
            ReportType.BusinessProfitAndLoss,
            filters,
            response,
            stopwatch,
            cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<GoalReportResponse>> GetGoalReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<GoalReportResponse>(userResult.Error!);
        }

        var stopwatch = Stopwatch.StartNew();

        if (filters?.GoalId is Guid goalId)
        {
            var progress = await _goalService.GetProgressAsync(goalId, cancellationToken);
            if (progress.IsFailure)
            {
                return Result.Failure<GoalReportResponse>(progress.Error!);
            }

            var goal = await _goalService.GetByIdAsync(goalId, cancellationToken);
            var response = new GoalReportResponse
            {
                ReportType = ReportType.GoalProgress,
                Title = "Goal Progress Report",
                GeneratedAt = DateTime.UtcNow,
                CurrencyCode = goal.IsSuccess ? goal.Value.CurrencyCode : "INR",
                Filters = MapFilters(filters),
                DataSources = new[] { "Goals" },
                ActiveGoals = 1,
                TotalGoalValue = progress.Value.TargetAmount,
                TotalSaved = progress.Value.CurrentAmount,
                OverallProgressPercent = progress.Value.CompletionPercent,
                MonthlyCommitted = progress.Value.MonthlyContribution,
            };

            await PersistExecutionAsync(
                userResult.Value,
                ReportType.GoalProgress,
                filters,
                response,
                stopwatch,
                cancellationToken);

            return Result.Success(response);
        }

        var dashboard = await _goalService.GetDashboardAsync(cancellationToken);
        if (dashboard.IsFailure)
        {
            return Result.Failure<GoalReportResponse>(dashboard.Error!);
        }

        var report = new GoalReportResponse
        {
            ReportType = ReportType.GoalProgress,
            Title = "Goal Progress Report",
            GeneratedAt = DateTime.UtcNow,
            CurrencyCode = dashboard.Value.CurrencyCode,
            Filters = MapFilters(filters),
            DataSources = new[] { "Goals" },
            ActiveGoals = dashboard.Value.ActiveGoals,
            CompletedGoals = dashboard.Value.CompletedGoals,
            PausedGoals = dashboard.Value.PausedGoals,
            TotalGoalValue = dashboard.Value.TotalGoalValue,
            TotalSaved = dashboard.Value.TotalSaved,
            OverallProgressPercent = dashboard.Value.OverallProgressPercent,
            MonthlyCommitted = dashboard.Value.MonthlyCommitted,
        };

        await PersistExecutionAsync(
            userResult.Value,
            ReportType.GoalProgress,
            filters,
            report,
            stopwatch,
            cancellationToken);

        return Result.Success(report);
    }

    public async Task<Result<DocumentReportResponse>> GetDocumentReportAsync(
        ReportFilterRequest? filters,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DocumentReportResponse>(userResult.Error!);
        }

        var stopwatch = Stopwatch.StartNew();
        var summary = await _documentSummaryProvider.GetSummaryAsync(userResult.Value, cancellationToken);
        var recent = await _documentSearchService.GetRecentAsync(10, cancellationToken);
        var expired = await _documentSearchService.GetExpiredAsync(10, cancellationToken);
        var notifications = await _notificationService.GetSummaryAsync(cancellationToken);

        var response = new DocumentReportResponse
        {
            ReportType = ReportType.DocumentSummary,
            Title = "Document Summary Report",
            GeneratedAt = DateTime.UtcNow,
            CurrencyCode = "INR",
            Filters = MapFilters(filters),
            DataSources = new[] { "Documents", "Notifications" },
            DocumentCount = summary.DocumentCount,
            PendingReviewCount = summary.PendingReviewCount,
            RecentCount = recent.IsSuccess ? recent.Value.Items.Count : 0,
            ExpiredCount = expired.IsSuccess ? expired.Value.Items.Count : 0,
            UnreadNotifications = notifications.IsSuccess ? notifications.Value.UnreadCount : 0,
        };

        await PersistExecutionAsync(
            userResult.Value,
            ReportType.DocumentSummary,
            filters,
            response,
            stopwatch,
            cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<ReportSnapshotResponse>> GenerateSnapshotAsync(
        GenerateSnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ReportSnapshotResponse>(userResult.Error!);
        }

        var payloadResult = await BuildPayloadForTypeAsync(
            request.ReportType,
            request.Filters,
            cancellationToken);
        if (payloadResult.IsFailure)
        {
            return Result.Failure<ReportSnapshotResponse>(payloadResult.Error!);
        }

        var (title, currency, payloadJson) = payloadResult.Value;
        var snapshot = new ReportSnapshot
        {
            UserId = userResult.Value,
            ReportType = request.ReportType,
            Title = string.IsNullOrWhiteSpace(request.Title) ? title : request.Title.Trim(),
            PayloadJson = payloadJson,
            FiltersJson = request.Filters is null ? null : JsonSerializer.Serialize(request.Filters, JsonOptions),
            CapturedAt = DateTime.UtcNow,
            CurrencyCode = currency,
        };

        await _snapshotRepository.AddAsync(snapshot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new ReportSnapshotResponse
        {
            Id = snapshot.Id,
            ReportType = snapshot.ReportType,
            Title = snapshot.Title,
            CapturedAt = snapshot.CapturedAt,
            CurrencyCode = snapshot.CurrencyCode,
            PayloadJson = snapshot.PayloadJson,
            Filters = MapFilters(request.Filters),
        });
    }

    private async Task<Result<(string Title, string Currency, string PayloadJson)>> BuildPayloadForTypeAsync(
        ReportType reportType,
        ReportFilterRequest? filters,
        CancellationToken cancellationToken)
    {
        switch (reportType)
        {
            case ReportType.NetWorth:
            {
                var result = await GetNetWorthReportAsync(filters, cancellationToken);
                return result.IsSuccess
                    ? Result.Success((result.Value.Title, result.Value.CurrencyCode, JsonSerializer.Serialize(result.Value, JsonOptions)))
                    : Result.Failure<(string, string, string)>(result.Error!);
            }
            case ReportType.CashFlow:
            {
                var result = await GetCashFlowReportAsync(filters, cancellationToken);
                return result.IsSuccess
                    ? Result.Success((result.Value.Title, result.Value.CurrencyCode, JsonSerializer.Serialize(result.Value, JsonOptions)))
                    : Result.Failure<(string, string, string)>(result.Error!);
            }
            case ReportType.InvestmentPerformance:
            case ReportType.AssetAllocation:
            {
                var result = await GetInvestmentReportAsync(filters, cancellationToken);
                return result.IsSuccess
                    ? Result.Success((result.Value.Title, result.Value.CurrencyCode, JsonSerializer.Serialize(result.Value, JsonOptions)))
                    : Result.Failure<(string, string, string)>(result.Error!);
            }
            case ReportType.LoanAnalysis:
            {
                var result = await GetLoanReportAsync(filters, cancellationToken);
                return result.IsSuccess
                    ? Result.Success((result.Value.Title, result.Value.CurrencyCode, JsonSerializer.Serialize(result.Value, JsonOptions)))
                    : Result.Failure<(string, string, string)>(result.Error!);
            }
            case ReportType.PropertyAppreciation:
            {
                var result = await GetPropertyReportAsync(filters, cancellationToken);
                return result.IsSuccess
                    ? Result.Success((result.Value.Title, result.Value.CurrencyCode, JsonSerializer.Serialize(result.Value, JsonOptions)))
                    : Result.Failure<(string, string, string)>(result.Error!);
            }
            case ReportType.BusinessProfitAndLoss:
            case ReportType.Income:
            {
                var result = await GetBusinessReportAsync(filters, cancellationToken);
                return result.IsSuccess
                    ? Result.Success((result.Value.Title, result.Value.CurrencyCode, JsonSerializer.Serialize(result.Value, JsonOptions)))
                    : Result.Failure<(string, string, string)>(result.Error!);
            }
            case ReportType.GoalProgress:
            {
                var result = await GetGoalReportAsync(filters, cancellationToken);
                return result.IsSuccess
                    ? Result.Success((result.Value.Title, result.Value.CurrencyCode, JsonSerializer.Serialize(result.Value, JsonOptions)))
                    : Result.Failure<(string, string, string)>(result.Error!);
            }
            case ReportType.DocumentSummary:
            {
                var result = await GetDocumentReportAsync(filters, cancellationToken);
                return result.IsSuccess
                    ? Result.Success((result.Value.Title, result.Value.CurrencyCode, JsonSerializer.Serialize(result.Value, JsonOptions)))
                    : Result.Failure<(string, string, string)>(result.Error!);
            }
            default:
                return Result.Failure<(string, string, string)>(
                    Error.Failure("unsupported_report_type", $"Report type '{reportType}' is not supported for snapshots."));
        }
    }

    private async Task PersistExecutionAsync<T>(
        Guid userId,
        ReportType reportType,
        ReportFilterRequest? filters,
        T payload,
        Stopwatch stopwatch,
        CancellationToken cancellationToken)
    {
        stopwatch.Stop();
        var definition = await _definitionRepository.GetByTypeAsync(reportType, cancellationToken);

        var execution = new ReportExecution
        {
            UserId = userId,
            ReportDefinitionId = definition?.Id,
            ReportType = reportType,
            Status = ReportExecutionStatus.Succeeded,
            FiltersJson = filters is null ? null : JsonSerializer.Serialize(filters, JsonOptions),
            ResultSummaryJson = JsonSerializer.Serialize(payload, JsonOptions),
            StartedAt = DateTime.UtcNow.AddMilliseconds(-stopwatch.ElapsedMilliseconds),
            CompletedAt = DateTime.UtcNow,
            DurationMs = (int)stopwatch.ElapsedMilliseconds,
        };

        await _executionRepository.AddAsync(execution, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null || _currentUser.UserId == Guid.Empty)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
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

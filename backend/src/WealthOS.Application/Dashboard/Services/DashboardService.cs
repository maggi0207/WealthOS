using AutoMapper;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.DTOs.Responses;
using WealthOS.Application.Dashboard.Interfaces;
using WealthOS.Application.Dashboard.Providers;
using WealthOS.Domain.Dashboard.Models;

namespace WealthOS.Application.Dashboard.Services;

/// <summary>
/// Aggregates module summary providers into dashboard read models.
/// Does not call other modules' repositories — only provider abstractions.
/// </summary>
public sealed class DashboardService : IDashboardService
{
    private const decimal DemoChangePercent = 3.8m;

    private readonly ICurrentUserService _currentUserService;
    private readonly IPropertySummaryProvider _propertySummaryProvider;
    private readonly ILoanSummaryProvider _loanSummaryProvider;
    private readonly IInvestmentSummaryProvider _investmentSummaryProvider;
    private readonly IIncomeSummaryProvider _incomeSummaryProvider;
    private readonly IDocumentSummaryProvider _documentSummaryProvider;
    private readonly IMapper _mapper;

    public DashboardService(
        ICurrentUserService currentUserService,
        IPropertySummaryProvider propertySummaryProvider,
        ILoanSummaryProvider loanSummaryProvider,
        IInvestmentSummaryProvider investmentSummaryProvider,
        IIncomeSummaryProvider incomeSummaryProvider,
        IDocumentSummaryProvider documentSummaryProvider,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _propertySummaryProvider = propertySummaryProvider;
        _loanSummaryProvider = loanSummaryProvider;
        _investmentSummaryProvider = investmentSummaryProvider;
        _incomeSummaryProvider = incomeSummaryProvider;
        _documentSummaryProvider = documentSummaryProvider;
        _mapper = mapper;
    }

    public async Task<Result<DashboardResponse>> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DashboardResponse>(userResult.Error!);
        }

        var summary = await BuildSummaryAsync(userResult.Value, cancellationToken);
        return Result.Success(_mapper.Map<DashboardResponse>(summary));
    }

    public async Task<Result<NetWorthResponse>> GetNetWorthAsync(CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<NetWorthResponse>(userResult.Error!);
        }

        var financials = await BuildFinancialSummaryAsync(userResult.Value, cancellationToken);
        return Result.Success(_mapper.Map<NetWorthResponse>(financials));
    }

    public async Task<Result<IReadOnlyList<RecentActivityResponse>>> GetRecentActivitiesAsync(
        int limit,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<IReadOnlyList<RecentActivityResponse>>(userResult.Error!);
        }

        // Keep async signature for future module-backed activity feeds.
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();

        var activities = BuildDemoActivities()
            .OrderByDescending(activity => activity.OccurredAt)
            .Take(limit)
            .ToList();

        return Result.Success<IReadOnlyList<RecentActivityResponse>>(
            _mapper.Map<IReadOnlyList<RecentActivityResponse>>(activities));
    }

    public async Task<Result<DashboardHealthResponse>> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DashboardHealthResponse>(userResult.Error!);
        }

        var userId = userResult.Value;
        var providerStatuses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            _ = await _propertySummaryProvider.GetSummaryAsync(userId, cancellationToken);
            providerStatuses["property"] = "Healthy";
        }
        catch
        {
            providerStatuses["property"] = "Unhealthy";
        }

        try
        {
            _ = await _loanSummaryProvider.GetSummaryAsync(userId, cancellationToken);
            providerStatuses["loan"] = "Healthy";
        }
        catch
        {
            providerStatuses["loan"] = "Unhealthy";
        }

        try
        {
            _ = await _investmentSummaryProvider.GetSummaryAsync(userId, cancellationToken);
            providerStatuses["investment"] = "Healthy";
        }
        catch
        {
            providerStatuses["investment"] = "Unhealthy";
        }

        try
        {
            _ = await _incomeSummaryProvider.GetSummaryAsync(userId, cancellationToken);
            providerStatuses["income"] = "Healthy";
        }
        catch
        {
            providerStatuses["income"] = "Unhealthy";
        }

        try
        {
            _ = await _documentSummaryProvider.GetSummaryAsync(userId, cancellationToken);
            providerStatuses["document"] = "Healthy";
        }
        catch
        {
            providerStatuses["document"] = "Unhealthy";
        }

        var providersReady = providerStatuses.Values.All(status => status == "Healthy");

        return Result.Success(new DashboardHealthResponse
        {
            Status = providersReady ? "Healthy" : "Degraded",
            ProvidersReady = providersReady,
            ProviderStatuses = providerStatuses,
            CheckedAt = DateTimeOffset.UtcNow,
        });
    }

    public async Task<Result<DashboardSnapshot>> CreateSnapshotAsync(CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<DashboardSnapshot>(userResult.Error!);
        }

        var summary = await BuildSummaryAsync(userResult.Value, cancellationToken);

        return Result.Success(new DashboardSnapshot
        {
            Id = Guid.NewGuid(),
            UserId = userResult.Value,
            Financials = summary.Financials,
            HealthScore = summary.HealthScore,
            CapturedAt = DateTimeOffset.UtcNow,
        });
    }

    private Result<Guid> RequireUserId()
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUserService.UserId.Value);
    }

    private async Task<DashboardSummary> BuildSummaryAsync(Guid userId, CancellationToken cancellationToken)
    {
        var financials = await BuildFinancialSummaryAsync(userId, cancellationToken);
        var documentSummary = await _documentSummaryProvider.GetSummaryAsync(userId, cancellationToken);

        return new DashboardSummary
        {
            Financials = financials,
            HealthScore = BuildHealthScore(financials, documentSummary),
            RecentActivities = BuildDemoActivities().Take(6).ToList(),
            QuickActions = BuildQuickActions(),
            GeneratedAt = DateTimeOffset.UtcNow,
        };
    }

    private async Task<FinancialSummary> BuildFinancialSummaryAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Providers share a scoped DbContext — query sequentially to avoid concurrent use.
        var property = await _propertySummaryProvider.GetSummaryAsync(userId, cancellationToken);
        var loan = await _loanSummaryProvider.GetSummaryAsync(userId, cancellationToken);
        var investment = await _investmentSummaryProvider.GetSummaryAsync(userId, cancellationToken);
        var income = await _incomeSummaryProvider.GetSummaryAsync(userId, cancellationToken);

        var assetValue = property.TotalValue + investment.TotalValue;
        var liabilityValue = loan.TotalBalance;
        var netWorth = assetValue - liabilityValue;

        return new FinancialSummary
        {
            NetWorth = netWorth,
            AssetValue = assetValue,
            LiabilityValue = liabilityValue,
            MonthlyIncome = income.MonthlyIncome,
            MonthlyExpense = income.MonthlyExpense,
            InvestmentValue = investment.TotalValue,
            PropertyValue = property.TotalValue,
            LoanBalance = loan.TotalBalance,
            CurrencyCode = ResolveCurrencyCode(
                property.CurrencyCode,
                loan.CurrencyCode,
                investment.CurrencyCode,
                income.CurrencyCode),
            ChangePercent = DemoChangePercent,
        };
    }

    private static string ResolveCurrencyCode(params string[] codes)
    {
        foreach (var code in codes)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                return code.Trim().ToUpperInvariant();
            }
        }

        return "INR";
    }

    private static HealthScore BuildHealthScore(FinancialSummary financials, DocumentModuleSummary documents)
    {
        var savingsRateScore = financials.MonthlyIncome <= 0
            ? 50
            : Math.Clamp(
                (int)Math.Round(((financials.MonthlyIncome - financials.MonthlyExpense) / financials.MonthlyIncome) * 100m),
                0,
                100);

        var debtToIncomeScore = financials.MonthlyIncome <= 0
            ? 50
            : Math.Clamp(
                100 - (int)Math.Round((financials.LiabilityValue / (financials.MonthlyIncome * 12m)) * 10m),
                0,
                100);

        var documentScore = documents.DocumentCount == 0
            ? 60
            : Math.Clamp(100 - (documents.PendingReviewCount * 10), 40, 100);

        var factors = new List<HealthScoreFactor>
        {
            new() { Label = "Savings rate", Value = savingsRateScore, Weight = "High" },
            new() { Label = "Debt-to-income", Value = debtToIncomeScore, Weight = "High" },
            new() { Label = "Emergency buffer", Value = 64, Weight = "Medium" },
            new() { Label = "Diversification", Value = 81, Weight = "Medium" },
            new() { Label = "Document readiness", Value = documentScore, Weight = "Low" },
        };

        var score = (int)Math.Round(factors.Average(factor => factor.Value));
        var grade = score switch
        {
            >= 85 => "Excellent",
            >= 70 => "Strong",
            >= 55 => "Fair",
            _ => "Needs attention",
        };

        return new HealthScore
        {
            Score = score,
            Grade = grade,
            ChangePoints = 4,
            Factors = factors,
        };
    }

    private static IReadOnlyList<QuickAction> BuildQuickActions() =>
    [
        new() { Key = "add-expense", Label = "Add expense", Route = "/expenses", Icon = "receipt" },
        new() { Key = "add-asset", Label = "Add asset", Route = "/assets", Icon = "coins" },
        new() { Key = "add-income", Label = "Add income", Route = "/income", Icon = "banknote" },
        new() { Key = "record-payment", Label = "Record payment", Route = "/loans", Icon = "landmark" },
    ];

    private static IReadOnlyList<RecentActivity> BuildDemoActivities()
    {
        var now = DateTimeOffset.UtcNow;

        return
        [
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                Title = "Salary credited",
                Detail = "Meridian Capital · Payroll",
                Amount = 18_400m,
                Direction = "in",
                Category = "Income",
                OccurredAt = now.Date.AddHours(9).AddMinutes(12),
            },
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111102"),
                Title = "SIP executed",
                Detail = "Global Index Fund · monthly",
                Amount = 2_000m,
                Direction = "out",
                Category = "Investment",
                OccurredAt = now.Date.AddHours(6).AddMinutes(30),
            },
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111103"),
                Title = "Mortgage EMI",
                Detail = "Home mortgage · auto-debit",
                Amount = 2_410m,
                Direction = "out",
                Category = "Loan",
                OccurredAt = now.AddDays(-1),
            },
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111104"),
                Title = "Rent received",
                Detail = "Harbour View apartment",
                Amount = 3_150m,
                Direction = "in",
                Category = "Property",
                OccurredAt = now.AddDays(-1).AddHours(-2),
            },
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111105"),
                Title = "Dividend payout",
                Detail = "Blue-chip equity basket",
                Amount = 940m,
                Direction = "in",
                Category = "Investment",
                OccurredAt = now.AddDays(-2),
            },
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111106"),
                Title = "Card settlement",
                Detail = "Travel & dining",
                Amount = 1_265m,
                Direction = "out",
                Category = "Expense",
                OccurredAt = now.AddDays(-3),
            },
        ];
    }
}

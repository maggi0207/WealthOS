using WealthOS.Domain.Dashboard.Models;

namespace WealthOS.Application.Dashboard.DTOs.Responses;

/// <summary>
/// Full dashboard payload returned by <c>GET /api/v1/dashboard</c>.
/// </summary>
public sealed class DashboardResponse
{
    public decimal NetWorth { get; init; }

    public decimal AssetValue { get; init; }

    public decimal LiabilityValue { get; init; }

    public decimal MonthlyIncome { get; init; }

    public decimal MonthlyExpense { get; init; }

    public decimal InvestmentValue { get; init; }

    public decimal PropertyValue { get; init; }

    public decimal LoanBalance { get; init; }

    public decimal ChangePercent { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public HealthScoreResponse HealthScore { get; init; } = new();

    public IReadOnlyList<RecentActivityResponse> RecentActivities { get; init; } = [];

    public IReadOnlyList<QuickActionResponse> QuickActions { get; init; } = [];

    public DateTimeOffset GeneratedAt { get; init; }
}

/// <summary>
/// Net-worth slice returned by <c>GET /api/v1/dashboard/net-worth</c>.
/// </summary>
public sealed class NetWorthResponse
{
    public decimal NetWorth { get; init; }

    public decimal AssetValue { get; init; }

    public decimal LiabilityValue { get; init; }

    public decimal ChangePercent { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

/// <summary>
/// Recent activity item in the dashboard feed.
/// </summary>
public sealed class RecentActivityResponse
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Detail { get; init; } = string.Empty;

    public decimal Amount { get; init; }

    public string Direction { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public DateTimeOffset OccurredAt { get; init; }
}

/// <summary>
/// Portfolio health score DTO.
/// </summary>
public sealed class HealthScoreResponse
{
    public int Score { get; init; }

    public string Grade { get; init; } = string.Empty;

    public int ChangePoints { get; init; }

    public IReadOnlyList<HealthScoreFactorResponse> Factors { get; init; } = [];
}

/// <summary>
/// Individual health factor DTO.
/// </summary>
public sealed class HealthScoreFactorResponse
{
    public string Label { get; init; } = string.Empty;

    public int Value { get; init; }

    public string Weight { get; init; } = string.Empty;
}

/// <summary>
/// Quick-action shortcut DTO.
/// </summary>
public sealed class QuickActionResponse
{
    public string Key { get; init; } = string.Empty;

    public string Label { get; init; } = string.Empty;

    public string Route { get; init; } = string.Empty;

    public string Icon { get; init; } = string.Empty;
}

/// <summary>
/// Dashboard module readiness returned by <c>GET /api/v1/dashboard/health</c>.
/// </summary>
public sealed class DashboardHealthResponse
{
    public string Status { get; init; } = string.Empty;

    public bool ProvidersReady { get; init; }

    public IReadOnlyDictionary<string, string> ProviderStatuses { get; init; } =
        new Dictionary<string, string>();

    public DateTimeOffset CheckedAt { get; init; }
}

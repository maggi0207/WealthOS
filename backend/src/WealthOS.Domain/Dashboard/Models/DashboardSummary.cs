namespace WealthOS.Domain.Dashboard.Models;

/// <summary>
/// Full dashboard aggregation for the authenticated user.
/// Phase 3 read model composed from module summary providers — not mapped to EF.
/// </summary>
public sealed class DashboardSummary
{
    public FinancialSummary Financials { get; init; } = new();

    public HealthScore HealthScore { get; init; } = new();

    public IReadOnlyList<RecentActivity> RecentActivities { get; init; } = [];

    public IReadOnlyList<QuickAction> QuickActions { get; init; } = [];

    public DateTimeOffset GeneratedAt { get; init; }
}

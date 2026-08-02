namespace WealthOS.Domain.Dashboard.Models;

/// <summary>
/// Point-in-time capture of dashboard financials and health.
/// Phase 3 read model reserved for future persistence; not mapped to EF yet.
/// </summary>
public sealed class DashboardSnapshot
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public FinancialSummary Financials { get; init; } = new();

    public HealthScore HealthScore { get; init; } = new();

    public DateTimeOffset CapturedAt { get; init; }
}

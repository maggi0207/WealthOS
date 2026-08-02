namespace WealthOS.Domain.Dashboard.Models;

/// <summary>
/// A recent portfolio activity feed item.
/// Phase 3 read model — not mapped to EF (mock provider supplies data).
/// </summary>
public sealed class RecentActivity
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Detail { get; init; } = string.Empty;

    public decimal Amount { get; init; }

    /// <summary>Cashflow direction: <c>in</c> or <c>out</c>.</summary>
    public string Direction { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public DateTimeOffset OccurredAt { get; init; }
}

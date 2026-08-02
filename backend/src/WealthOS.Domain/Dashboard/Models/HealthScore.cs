namespace WealthOS.Domain.Dashboard.Models;

/// <summary>
/// Portfolio health score and contributing factors.
/// Phase 3 read model / value object — not mapped to EF.
/// </summary>
public sealed class HealthScore
{
    public int Score { get; init; }

    public string Grade { get; init; } = string.Empty;

    public int ChangePoints { get; init; }

    public IReadOnlyList<HealthScoreFactor> Factors { get; init; } = [];
}

/// <summary>
/// A weighted factor contributing to <see cref="HealthScore"/>.
/// </summary>
public sealed class HealthScoreFactor
{
    public string Label { get; init; } = string.Empty;

    public int Value { get; init; }

    public string Weight { get; init; } = string.Empty;
}

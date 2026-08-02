using WealthOS.Domain.AI.Enums;

namespace WealthOS.Domain.AI.Models;

/// <summary>
/// Lightweight conversation summary for history lists.
/// </summary>
public sealed class AIConversationSummary
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public AIConversationStatus Status { get; init; }

    public DateTime? LastMessageAt { get; init; }

    public int MessageCount { get; init; }
}

/// <summary>
/// Aggregated insight counts for the advisor dashboard surface.
/// </summary>
public sealed class AIInsightSummary
{
    public int TotalCount { get; init; }

    public int HighSeverityCount { get; init; }

    public int ActiveRecommendationCount { get; init; }
}

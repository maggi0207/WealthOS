using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.AI.Entities;

/// <summary>
/// AI-generated actionable recommendation for the user.
/// </summary>
public sealed class AIRecommendation : AuditableEntity
{
    public AIRecommendation()
    {
    }

    public AIRecommendation(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid? ConversationId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public string? Category { get; set; }

    public AIRecommendationStatus Status { get; set; } = AIRecommendationStatus.Draft;

    public double Confidence { get; set; }

    public string? PayloadJson { get; set; }

    public DateTime? ExpiresAt { get; set; }
}

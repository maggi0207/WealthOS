using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.AI.Entities;

/// <summary>
/// AI-generated insight derived from financial context and tools.
/// </summary>
public sealed class AIInsight : AuditableEntity
{
    public AIInsight()
    {
    }

    public AIInsight(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid? ConversationId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public string? Module { get; set; }

    public AIInsightSeverity Severity { get; set; } = AIInsightSeverity.Info;

    public string? PayloadJson { get; set; }

    public bool IsDismissed { get; set; }
}

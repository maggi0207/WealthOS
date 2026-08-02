using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.AI.Entities;

/// <summary>
/// Structured memory item for future long-term recall (conversation, preferences, facts).
/// </summary>
public sealed class AIMemory : AuditableEntity
{
    public AIMemory()
    {
    }

    public AIMemory(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid? ConversationId { get; set; }

    public AIMemoryType MemoryType { get; set; } = AIMemoryType.ImportantFact;

    public string Key { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string? MetadataJson { get; set; }

    public double Importance { get; set; } = 0.5;

    public DateTime? ExpiresAt { get; set; }
}

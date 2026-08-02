using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.AI.Entities;

/// <summary>
/// Snapshot of aggregated financial context built for an AI turn.
/// </summary>
public sealed class AIContext : AuditableEntity
{
    public AIContext()
    {
    }

    public AIContext(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid? ConversationId { get; set; }

    public string ContextJson { get; set; } = "{}";

    public string? ModulesIncluded { get; set; }

    public DateTime BuiltAt { get; set; }
}

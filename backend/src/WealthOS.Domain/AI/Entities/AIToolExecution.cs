using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.AI.Entities;

/// <summary>
/// Audit record for a single tool invocation during orchestration.
/// </summary>
public sealed class AIToolExecution : AuditableEntity
{
    public AIToolExecution()
    {
    }

    public AIToolExecution(Guid id)
        : base(id)
    {
    }

    public Guid ConversationId { get; set; }

    public Guid? MessageId { get; set; }

    public string ToolCode { get; set; } = string.Empty;

    public AIToolExecutionStatus Status { get; set; } = AIToolExecutionStatus.Pending;

    public string? InputJson { get; set; }

    public string? OutputJson { get; set; }

    public string? ErrorMessage { get; set; }

    public int DurationMs { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public AIConversation? Conversation { get; set; }
}

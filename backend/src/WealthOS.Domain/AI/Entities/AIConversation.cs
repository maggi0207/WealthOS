using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.AI.Entities;

/// <summary>
/// Aggregate root for an AI advisor conversation thread.
/// </summary>
public sealed class AIConversation : AuditableEntity
{
    public AIConversation()
    {
    }

    public AIConversation(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public string Title { get; set; } = "New conversation";

    public AIConversationStatus Status { get; set; } = AIConversationStatus.Active;

    public AIProviderKind ProviderKind { get; set; } = AIProviderKind.OpenAI;

    public string? Summary { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public ICollection<AIMessage> Messages { get; set; } = new List<AIMessage>();

    public ICollection<ConversationSession> Sessions { get; set; } = new List<ConversationSession>();

    public ICollection<AIToolExecution> ToolExecutions { get; set; } = new List<AIToolExecution>();
}

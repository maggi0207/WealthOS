using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.AI.Entities;

/// <summary>
/// A single message within an AI conversation.
/// </summary>
public sealed class AIMessage : AuditableEntity
{
    public AIMessage()
    {
    }

    public AIMessage(Guid id)
        : base(id)
    {
    }

    public Guid ConversationId { get; set; }

    public AIMessageRole Role { get; set; } = AIMessageRole.User;

    public string Content { get; set; } = string.Empty;

    public string? MetadataJson { get; set; }

    public int Sequence { get; set; }

    public AIConversation? Conversation { get; set; }
}

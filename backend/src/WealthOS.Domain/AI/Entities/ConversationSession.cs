using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.AI.Entities;

/// <summary>
/// Tracks an active interactive session bound to a conversation.
/// </summary>
public sealed class ConversationSession : AuditableEntity
{
    public ConversationSession()
    {
    }

    public ConversationSession(Guid id)
        : base(id)
    {
    }

    public Guid ConversationId { get; set; }

    public Guid UserId { get; set; }

    public string SessionKey { get; set; } = string.Empty;

    public ConversationSessionStatus Status { get; set; } = ConversationSessionStatus.Open;

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public AIConversation? Conversation { get; set; }
}

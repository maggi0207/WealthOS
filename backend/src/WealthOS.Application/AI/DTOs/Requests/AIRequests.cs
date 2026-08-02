using WealthOS.Domain.AI.Enums;

namespace WealthOS.Application.AI.DTOs.Requests;

/// <summary>Starts a new AI conversation (optional title).</summary>
public sealed class StartConversationRequest
{
    public string? Title { get; init; }

    public AIProviderKind? PreferredProvider { get; init; }
}

/// <summary>Sends a user message to the AI advisor.</summary>
public sealed class SendMessageRequest
{
    public Guid? ConversationId { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? PromptTemplateCode { get; init; }
}

/// <summary>Persists a memory item for future context.</summary>
public sealed class SaveMemoryRequest
{
    public Guid? ConversationId { get; init; }

    public AIMemoryType MemoryType { get; init; } = AIMemoryType.ImportantFact;

    public string Key { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public string? MetadataJson { get; init; }

    public double Importance { get; init; } = 0.5;

    public DateTime? ExpiresAt { get; init; }
}

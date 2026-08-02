using WealthOS.Domain.AI.Enums;

namespace WealthOS.Application.AI.DTOs.Responses;

/// <summary>Chat turn response from the AI orchestration platform.</summary>
public sealed class AIChatResponse
{
    public Guid ConversationId { get; init; }

    public Guid? SessionId { get; init; }

    public Guid UserMessageId { get; init; }

    public Guid AssistantMessageId { get; init; }

    public string Reply { get; init; } = string.Empty;

    public string Provider { get; init; } = string.Empty;

    public bool IsPlaceholder { get; init; } = true;

    public IReadOnlyList<AIToolExecutionResponse> ToolExecutions { get; init; } =
        Array.Empty<AIToolExecutionResponse>();

    public IReadOnlyList<string> ModulesUsed { get; init; } = Array.Empty<string>();
}

/// <summary>Full conversation with messages.</summary>
public sealed class AIConversationResponse
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public AIConversationStatus Status { get; init; }

    public AIProviderKind ProviderKind { get; init; }

    public string? Summary { get; init; }

    public DateTime? LastMessageAt { get; init; }

    public DateTime CreatedAt { get; init; }

    public IReadOnlyList<AIMessageResponse> Messages { get; init; } = Array.Empty<AIMessageResponse>();
}

/// <summary>Single conversation message.</summary>
public sealed class AIMessageResponse
{
    public Guid Id { get; init; }

    public AIMessageRole Role { get; init; }

    public string Content { get; init; } = string.Empty;

    public int Sequence { get; init; }

    public DateTime CreatedAt { get; init; }
}

/// <summary>Paginated conversation history.</summary>
public sealed class AIConversationHistoryResponse
{
    public IReadOnlyList<AIConversationSummaryResponse> Items { get; init; } =
        Array.Empty<AIConversationSummaryResponse>();

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }
}

/// <summary>Conversation list item.</summary>
public sealed class AIConversationSummaryResponse
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public AIConversationStatus Status { get; init; }

    public DateTime? LastMessageAt { get; init; }

    public int MessageCount { get; init; }
}

/// <summary>Suggested prompts for the advisor UI.</summary>
public sealed class AISuggestionsResponse
{
    public IReadOnlyList<string> Suggestions { get; init; } = Array.Empty<string>();
}

/// <summary>Insights and recommendation bundle.</summary>
public sealed class AIInsightsResponse
{
    public int TotalInsightCount { get; init; }

    public int HighSeverityCount { get; init; }

    public int ActiveRecommendationCount { get; init; }

    public IReadOnlyList<AIInsightItemResponse> Insights { get; init; } =
        Array.Empty<AIInsightItemResponse>();

    public IReadOnlyList<AIRecommendationResponse> Recommendations { get; init; } =
        Array.Empty<AIRecommendationResponse>();
}

/// <summary>Single insight item.</summary>
public sealed class AIInsightItemResponse
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public string? Module { get; init; }

    public AIInsightSeverity Severity { get; init; }

    public DateTime CreatedAt { get; init; }
}

/// <summary>Recommendation item.</summary>
public sealed class AIRecommendationResponse
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public string? Category { get; init; }

    public AIRecommendationStatus Status { get; init; }

    public double Confidence { get; init; }
}

/// <summary>Tool execution audit item.</summary>
public sealed class AIToolExecutionResponse
{
    public string ToolCode { get; init; } = string.Empty;

    public string ToolName { get; init; } = string.Empty;

    public AIToolExecutionStatus Status { get; init; }

    public string Summary { get; init; } = string.Empty;

    public int DurationMs { get; init; }
}

/// <summary>Memory item response.</summary>
public sealed class AIMemoryResponse
{
    public Guid Id { get; init; }

    public AIMemoryType MemoryType { get; init; }

    public string Key { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public double Importance { get; init; }

    public DateTime CreatedAt { get; init; }
}

/// <summary>Paginated memory list.</summary>
public sealed class AIMemoryListResponse
{
    public IReadOnlyList<AIMemoryResponse> Items { get; init; } = Array.Empty<AIMemoryResponse>();

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }
}

/// <summary>Prompt template response.</summary>
public sealed class PromptTemplateResponse
{
    public Guid Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public PromptTemplateCategory Category { get; init; }

    public string SystemPrompt { get; init; } = string.Empty;

    public string UserPromptTemplate { get; init; } = string.Empty;

    public int Version { get; init; }
}

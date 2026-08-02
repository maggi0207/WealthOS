using WealthOS.Application.Common.Models;
using WealthOS.Application.AI.DTOs.Requests;
using WealthOS.Application.AI.DTOs.Responses;
using WealthOS.Domain.AI.Enums;

namespace WealthOS.Application.AI.Interfaces;

/// <summary>
/// Primary AI advisor orchestration service (chat, history, suggestions, insights).
/// </summary>
public interface IAIService
{
    Task<Result<AIChatResponse>> StartConversationAsync(
        StartConversationRequest? request,
        CancellationToken cancellationToken = default);

    Task<Result<AIChatResponse>> SendMessageAsync(
        SendMessageRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> ClearConversationAsync(CancellationToken cancellationToken = default);

    Task<Result<AIConversationResponse>> GetConversationAsync(
        Guid? conversationId,
        CancellationToken cancellationToken = default);

    Task<Result<AIConversationHistoryResponse>> GetHistoryAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<AISuggestionsResponse>> GetSuggestionsAsync(
        CancellationToken cancellationToken = default);

    Task<Result<AIInsightsResponse>> GetInsightsAsync(
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Placeholder LLM / orchestration provider. No external API calls in Phase 11.
/// </summary>
public interface IAIProvider
{
    AIProviderKind Kind { get; }

    Task<Result<string>> GenerateResponseAsync(
        AIProviderRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<string>> GenerateStructuredResponseAsync(
        AIProviderRequest request,
        string schemaHint,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<float>>> GenerateEmbeddingsAsync(
        string text,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Request payload passed to an <see cref="IAIProvider"/>.
/// </summary>
public sealed class AIProviderRequest
{
    public string SystemPrompt { get; init; } = string.Empty;

    public string UserPrompt { get; init; } = string.Empty;

    public string? ContextJson { get; init; }

    public IReadOnlyList<AIToolResultDto>? ToolResults { get; init; }
}

/// <summary>
/// Extensible AI tool contract. New tools register via DI without modifying existing tools.
/// </summary>
public interface IAITool
{
    string Code { get; }

    string Name { get; }

    string Description { get; }

    AIToolCategory Category { get; }

    Task<Result<AIToolResultDto>> ExecuteAsync(
        AIToolExecutionContext context,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Runtime context supplied to tool executions.
/// </summary>
public sealed class AIToolExecutionContext
{
    public Guid UserId { get; init; }

    public Guid? ConversationId { get; init; }

    public string UserMessage { get; init; } = string.Empty;

    public string? ArgumentsJson { get; init; }
}

/// <summary>
/// Tool registry for discovery and invocation.
/// </summary>
public interface IAIToolRegistry
{
    IReadOnlyList<IAITool> GetAll();

    IAITool? GetByCode(string code);

    Task<IReadOnlyList<AIToolResultDto>> ExecuteRelevantAsync(
        AIToolExecutionContext context,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Conversation and long-term memory operations.
/// </summary>
public interface IAIMemoryService
{
    Task<Result<AIMemoryResponse>> SaveAsync(
        SaveMemoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AIMemoryListResponse>> ListAsync(
        AIMemoryType? memoryType,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Prompt template resolution and rendering (placeholder substitution only).
/// </summary>
public interface IAIPromptService
{
    Task<Result<PromptTemplateResponse>> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<Result<string>> RenderAsync(
        string code,
        IReadOnlyDictionary<string, string> variables,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Aggregates financial module summaries into an AI context via application interfaces only.
/// </summary>
public interface IAIContextBuilder
{
    Task<Result<AIContextSnapshot>> BuildAsync(
        Guid userId,
        Guid? conversationId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Generates placeholder recommendations from context (no real LLM).
/// </summary>
public interface IAIRecommendationService
{
    Task<Result<IReadOnlyList<AIRecommendationResponse>>> GetActiveAsync(
        int take = 10,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<AIRecommendationResponse>>> GeneratePlaceholderAsync(
        AIContextSnapshot context,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Tool execution result DTO shared across orchestration.
/// </summary>
public sealed class AIToolResultDto
{
    public string ToolCode { get; init; } = string.Empty;

    public string ToolName { get; init; } = string.Empty;

    public bool Succeeded { get; init; }

    public string Summary { get; init; } = string.Empty;

    public string? PayloadJson { get; init; }

    public string? Error { get; init; }
}

/// <summary>
/// In-memory / persisted context snapshot returned by the context builder.
/// </summary>
public sealed class AIContextSnapshot
{
    public Guid UserId { get; init; }

    public Guid? ConversationId { get; init; }

    public DateTime BuiltAt { get; init; }

    public string ContextJson { get; init; } = "{}";

    public IReadOnlyList<string> ModulesIncluded { get; init; } = Array.Empty<string>();

    public IReadOnlyDictionary<string, object?> Sections { get; init; } =
        new Dictionary<string, object?>();
}

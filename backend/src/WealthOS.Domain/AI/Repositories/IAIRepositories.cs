using WealthOS.Domain.AI.Entities;
using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.AI.Models;
using WealthOS.Domain.Common.Abstractions.Repositories;

namespace WealthOS.Domain.AI.Repositories;

/// <summary>Persistence for AI conversations.</summary>
public interface IAIConversationRepository : IRepository<AIConversation>
{
    Task<AIConversation?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<AIConversation?> GetActiveForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<AIConversationSummary> Items, int TotalCount)> ListSummariesForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task ClearActiveForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>Persistence for AI messages.</summary>
public interface IAIMessageRepository : IRepository<AIMessage>
{
    Task<IReadOnlyList<AIMessage>> ListForConversationAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default);

    Task<int> GetNextSequenceAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default);
}

/// <summary>Persistence for conversation sessions.</summary>
public interface IConversationSessionRepository : IRepository<ConversationSession>
{
    Task<ConversationSession?> GetOpenSessionAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>Persistence for AI tool catalog entries.</summary>
public interface IAIToolRepository : IRepository<AITool>
{
    Task<AITool?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AITool>> ListEnabledAsync(CancellationToken cancellationToken = default);
}

/// <summary>Persistence for tool execution audits.</summary>
public interface IAIToolExecutionRepository : IRepository<AIToolExecution>;

/// <summary>Persistence for AI context snapshots.</summary>
public interface IAIContextRepository : IRepository<AIContext>
{
    Task<AIContext?> GetLatestForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>Persistence for AI memory items.</summary>
public interface IAIMemoryRepository : IRepository<AIMemory>
{
    Task<(IReadOnlyList<AIMemory> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        AIMemoryType? memoryType,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<AIMemory?> GetByKeyAsync(
        Guid userId,
        string key,
        CancellationToken cancellationToken = default);
}

/// <summary>Persistence for AI recommendations.</summary>
public interface IAIRecommendationRepository : IRepository<AIRecommendation>
{
    Task<IReadOnlyList<AIRecommendation>> ListActiveForUserAsync(
        Guid userId,
        int take,
        CancellationToken cancellationToken = default);
}

/// <summary>Persistence for AI insights.</summary>
public interface IAIInsightRepository : IRepository<AIInsight>
{
    Task<IReadOnlyList<AIInsight>> ListForUserAsync(
        Guid userId,
        int take,
        CancellationToken cancellationToken = default);

    Task<AIInsightSummary> GetSummaryForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>Persistence for prompt templates.</summary>
public interface IPromptTemplateRepository : IRepository<PromptTemplate>
{
    Task<PromptTemplate?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PromptTemplate>> ListActiveAsync(CancellationToken cancellationToken = default);
}

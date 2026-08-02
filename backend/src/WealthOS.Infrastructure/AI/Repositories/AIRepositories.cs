using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.AI.Entities;
using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.AI.Models;
using WealthOS.Domain.AI.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Repositories;

namespace WealthOS.Infrastructure.AI.Repositories;

public sealed class AIConversationRepository : Repository<AIConversation>, IAIConversationRepository
{
    public AIConversationRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<AIConversation?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            conversation => conversation.Id == id && conversation.UserId == userId,
            cancellationToken);

    public async Task<AIConversation?> GetActiveForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(conversation =>
                conversation.UserId == userId &&
                conversation.Status == AIConversationStatus.Active)
            .OrderByDescending(conversation => conversation.LastMessageAt ?? conversation.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<(IReadOnlyList<AIConversationSummary> Items, int TotalCount)> ListSummariesForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking()
            .Where(conversation =>
                conversation.UserId == userId &&
                conversation.Status != AIConversationStatus.Cleared);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(conversation => conversation.LastMessageAt ?? conversation.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(conversation => new AIConversationSummary
            {
                Id = conversation.Id,
                Title = conversation.Title,
                Status = conversation.Status,
                LastMessageAt = conversation.LastMessageAt,
                MessageCount = conversation.Messages.Count,
            })
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task ClearActiveForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var conversations = await DbSet
            .Where(conversation =>
                conversation.UserId == userId &&
                conversation.Status == AIConversationStatus.Active)
            .ToListAsync(cancellationToken);

        foreach (var conversation in conversations)
        {
            conversation.Status = AIConversationStatus.Cleared;
            conversation.Summary = "Cleared by user.";
        }
    }
}

public sealed class AIMessageRepository : Repository<AIMessage>, IAIMessageRepository
{
    public AIMessageRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<AIMessage>> ListForConversationAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Where(message => message.ConversationId == conversationId)
            .OrderBy(message => message.Sequence)
            .ToListAsync(cancellationToken);

    public async Task<int> GetNextSequenceAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var max = await DbSet
            .Where(message => message.ConversationId == conversationId)
            .Select(message => (int?)message.Sequence)
            .MaxAsync(cancellationToken);

        return (max ?? 0) + 1;
    }
}

public sealed class ConversationSessionRepository
    : Repository<ConversationSession>, IConversationSessionRepository
{
    public ConversationSessionRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<ConversationSession?> GetOpenSessionAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(session =>
                session.UserId == userId &&
                session.Status == ConversationSessionStatus.Open)
            .OrderByDescending(session => session.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);
}

public sealed class AIToolRepository : Repository<AITool>, IAIToolRepository
{
    public AIToolRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<AITool?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            tool => tool.Code == code,
            cancellationToken);

    public async Task<IReadOnlyList<AITool>> ListEnabledAsync(
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Where(tool => tool.IsEnabled)
            .OrderBy(tool => tool.SortOrder)
            .ToListAsync(cancellationToken);
}

public sealed class AIToolExecutionRepository
    : Repository<AIToolExecution>, IAIToolExecutionRepository
{
    public AIToolExecutionRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}

public sealed class AIContextRepository : Repository<AIContext>, IAIContextRepository
{
    public AIContextRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<AIContext?> GetLatestForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Where(context => context.UserId == userId)
            .OrderByDescending(context => context.BuiltAt)
            .FirstOrDefaultAsync(cancellationToken);
}

public sealed class AIMemoryRepository : Repository<AIMemory>, IAIMemoryRepository
{
    public AIMemoryRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<(IReadOnlyList<AIMemory> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        AIMemoryType? memoryType,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(memory => memory.UserId == userId);
        if (memoryType.HasValue)
        {
            query = query.Where(memory => memory.MemoryType == memoryType.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(memory => memory.Importance)
            .ThenByDescending(memory => memory.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<AIMemory?> GetByKeyAsync(
        Guid userId,
        string key,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            memory => memory.UserId == userId && memory.Key == key,
            cancellationToken);
}

public sealed class AIRecommendationRepository
    : Repository<AIRecommendation>, IAIRecommendationRepository
{
    public AIRecommendationRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<AIRecommendation>> ListActiveForUserAsync(
        Guid userId,
        int take,
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Where(recommendation =>
                recommendation.UserId == userId &&
                recommendation.Status == AIRecommendationStatus.Active)
            .OrderByDescending(recommendation => recommendation.Confidence)
            .Take(take)
            .ToListAsync(cancellationToken);
}

public sealed class AIInsightRepository : Repository<AIInsight>, IAIInsightRepository
{
    public AIInsightRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<AIInsight>> ListForUserAsync(
        Guid userId,
        int take,
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Where(insight => insight.UserId == userId && !insight.IsDismissed)
            .OrderByDescending(insight => insight.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);

    public async Task<AIInsightSummary> GetSummaryForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var insights = DbSet.AsNoTracking().Where(insight =>
            insight.UserId == userId && !insight.IsDismissed);

        var total = await insights.CountAsync(cancellationToken);
        var high = await insights.CountAsync(
            insight =>
                insight.Severity == AIInsightSeverity.High ||
                insight.Severity == AIInsightSeverity.Critical,
            cancellationToken);

        var activeRecommendations = await Context.Set<AIRecommendation>()
            .AsNoTracking()
            .CountAsync(
                recommendation =>
                    recommendation.UserId == userId &&
                    recommendation.Status == AIRecommendationStatus.Active,
                cancellationToken);

        return new AIInsightSummary
        {
            TotalCount = total,
            HighSeverityCount = high,
            ActiveRecommendationCount = activeRecommendations,
        };
    }
}

public sealed class PromptTemplateRepository : Repository<PromptTemplate>, IPromptTemplateRepository
{
    public PromptTemplateRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<PromptTemplate?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(template => template.Code == code, cancellationToken);

    public async Task<IReadOnlyList<PromptTemplate>> ListActiveAsync(
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Where(template => template.IsActive)
            .OrderBy(template => template.Category)
            .ToListAsync(cancellationToken);
}

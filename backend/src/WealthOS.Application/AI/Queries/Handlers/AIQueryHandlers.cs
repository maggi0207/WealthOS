using WealthOS.Application.AI.DTOs.Responses;
using WealthOS.Application.AI.Interfaces;
using WealthOS.Application.AI.Queries;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;

namespace WealthOS.Application.AI.Queries.Handlers;

public sealed class GetConversationQueryHandler
    : IQueryHandler<GetConversationQuery, AIConversationResponse>
{
    private readonly IAIService _aiService;

    public GetConversationQueryHandler(IAIService aiService) => _aiService = aiService;

    public Task<Result<AIConversationResponse>> HandleAsync(
        GetConversationQuery query,
        CancellationToken cancellationToken = default) =>
        _aiService.GetConversationAsync(query.ConversationId, cancellationToken);
}

public sealed class GetConversationHistoryQueryHandler
    : IQueryHandler<GetConversationHistoryQuery, AIConversationHistoryResponse>
{
    private readonly IAIService _aiService;

    public GetConversationHistoryQueryHandler(IAIService aiService) => _aiService = aiService;

    public Task<Result<AIConversationHistoryResponse>> HandleAsync(
        GetConversationHistoryQuery query,
        CancellationToken cancellationToken = default) =>
        _aiService.GetHistoryAsync(query.Page, query.PageSize, cancellationToken);
}

public sealed class GetSuggestionsQueryHandler
    : IQueryHandler<GetSuggestionsQuery, AISuggestionsResponse>
{
    private readonly IAIService _aiService;

    public GetSuggestionsQueryHandler(IAIService aiService) => _aiService = aiService;

    public Task<Result<AISuggestionsResponse>> HandleAsync(
        GetSuggestionsQuery query,
        CancellationToken cancellationToken = default) =>
        _aiService.GetSuggestionsAsync(cancellationToken);
}

public sealed class GetInsightsQueryHandler
    : IQueryHandler<GetInsightsQuery, AIInsightsResponse>
{
    private readonly IAIService _aiService;

    public GetInsightsQueryHandler(IAIService aiService) => _aiService = aiService;

    public Task<Result<AIInsightsResponse>> HandleAsync(
        GetInsightsQuery query,
        CancellationToken cancellationToken = default) =>
        _aiService.GetInsightsAsync(cancellationToken);
}

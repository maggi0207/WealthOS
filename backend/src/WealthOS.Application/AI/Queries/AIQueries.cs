using WealthOS.Application.Common.Abstractions;

namespace WealthOS.Application.AI.Queries;

/// <summary>Gets a conversation (active or by id) with messages.</summary>
public sealed class GetConversationQuery : IQuery
{
    public Guid? ConversationId { get; init; }
}

/// <summary>Lists conversation history for the authenticated user.</summary>
public sealed class GetConversationHistoryQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}

/// <summary>Returns suggested advisor prompts.</summary>
public sealed class GetSuggestionsQuery : IQuery;

/// <summary>Returns insights and recommendations.</summary>
public sealed class GetInsightsQuery : IQuery;

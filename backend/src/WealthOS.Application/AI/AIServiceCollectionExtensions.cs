using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.AI.Commands;
using WealthOS.Application.AI.Commands.Handlers;
using WealthOS.Application.AI.DTOs.Responses;
using WealthOS.Application.AI.Interfaces;
using WealthOS.Application.AI.Options;
using WealthOS.Application.AI.Queries;
using WealthOS.Application.AI.Queries.Handlers;
using WealthOS.Application.AI.Services;
using WealthOS.Application.AI.Tools;
using WealthOS.Application.Common.Abstractions;

namespace WealthOS.Application.AI;

/// <summary>
/// Registers AI Advisor application services, tools, and CQRS handlers.
/// Providers are registered in Infrastructure.
/// </summary>
public static class AIServiceCollectionExtensions
{
    public static IServiceCollection AddAIApplication(this IServiceCollection services)
    {
        services.AddOptions<AIOptions>()
            .BindConfiguration(AIOptions.SectionName);

        services.AddScoped<IAIService, AIService>();
        services.AddScoped<IAIMemoryService, AIMemoryService>();
        services.AddScoped<IAIPromptService, AIPromptService>();
        services.AddScoped<IAIContextBuilder, AIContextBuilder>();
        services.AddScoped<IAIRecommendationService, AIRecommendationService>();
        services.AddScoped<IAIToolRegistry, AIToolRegistry>();

        services.AddScoped<IAITool, GetDashboardSummaryTool>();
        services.AddScoped<IAITool, GetPropertySummaryTool>();
        services.AddScoped<IAITool, GetLoanSummaryTool>();
        services.AddScoped<IAITool, GetInvestmentSummaryTool>();
        services.AddScoped<IAITool, GetIncomeSummaryTool>();
        services.AddScoped<IAITool, GetGoalSummaryTool>();
        services.AddScoped<IAITool, SearchDocumentsTool>();
        services.AddScoped<IAITool, GetNotificationsTool>();

        services.AddScoped<
            ICommandHandler<StartConversationCommand, AIChatResponse>,
            StartConversationCommandHandler>();
        services.AddScoped<
            ICommandHandler<SendMessageCommand, AIChatResponse>,
            SendMessageCommandHandler>();
        services.AddScoped<
            ICommandHandler<ClearConversationCommand>,
            ClearConversationCommandHandler>();
        services.AddScoped<
            ICommandHandler<SaveMemoryCommand, AIMemoryResponse>,
            SaveMemoryCommandHandler>();

        services.AddScoped<
            IQueryHandler<GetConversationQuery, AIConversationResponse>,
            GetConversationQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetConversationHistoryQuery, AIConversationHistoryResponse>,
            GetConversationHistoryQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetSuggestionsQuery, AISuggestionsResponse>,
            GetSuggestionsQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetInsightsQuery, AIInsightsResponse>,
            GetInsightsQueryHandler>();

        return services;
    }
}

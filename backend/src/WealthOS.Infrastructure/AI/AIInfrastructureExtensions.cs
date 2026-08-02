using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WealthOS.Application.AI.Interfaces;
using WealthOS.Domain.AI.Entities;
using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.AI.Repositories;
using WealthOS.Infrastructure.AI.Providers;
using WealthOS.Infrastructure.AI.Repositories;
using WealthOS.Infrastructure.Persistence;

namespace WealthOS.Infrastructure.AI;

/// <summary>
/// Registers AI Advisor infrastructure: repositories, provider stubs, and seed hooks.
/// </summary>
public static class AIInfrastructureExtensions
{
    public static IServiceCollection AddAIInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IAIConversationRepository, AIConversationRepository>();
        services.AddScoped<IAIMessageRepository, AIMessageRepository>();
        services.AddScoped<IConversationSessionRepository, ConversationSessionRepository>();
        services.AddScoped<IAIToolRepository, AIToolRepository>();
        services.AddScoped<IAIToolExecutionRepository, AIToolExecutionRepository>();
        services.AddScoped<IAIContextRepository, AIContextRepository>();
        services.AddScoped<IAIMemoryRepository, AIMemoryRepository>();
        services.AddScoped<IAIRecommendationRepository, AIRecommendationRepository>();
        services.AddScoped<IAIInsightRepository, AIInsightRepository>();
        services.AddScoped<IPromptTemplateRepository, PromptTemplateRepository>();

        services.AddScoped<IAIProvider, OpenAIProvider>();
        services.AddScoped<IAIProvider, AzureOpenAIProvider>();
        services.AddScoped<IAIProvider, AnthropicProvider>();
        services.AddScoped<IAIProvider, GeminiProvider>();
        services.AddScoped<IAIProvider, MCPProvider>();

        return services;
    }
}

/// <summary>
/// Seeds prompt templates and AI tool catalog entries (framework placeholders).
/// </summary>
public static class AIDataSeeder
{
    public static readonly Guid FinancialSummaryPromptId =
        Guid.Parse("b1000001-0000-4000-8000-000000000001");

    public static readonly Guid InvestmentAnalysisPromptId =
        Guid.Parse("b1000001-0000-4000-8000-000000000002");

    public static readonly Guid LoanAnalysisPromptId =
        Guid.Parse("b1000001-0000-4000-8000-000000000003");

    public static readonly Guid CashFlowAnalysisPromptId =
        Guid.Parse("b1000001-0000-4000-8000-000000000004");

    public static readonly Guid GoalPlanningPromptId =
        Guid.Parse("b1000001-0000-4000-8000-000000000005");

    public static readonly Guid BusinessAnalysisPromptId =
        Guid.Parse("b1000001-0000-4000-8000-000000000006");

    public static readonly Guid DocumentSearchPromptId =
        Guid.Parse("b1000001-0000-4000-8000-000000000007");

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(AIDataSeeder));

        if (!dbContext.PromptTemplates.Any())
        {
            dbContext.PromptTemplates.AddRange(
                CreatePrompt(
                    FinancialSummaryPromptId,
                    "financial-summary",
                    "Financial Summary",
                    PromptTemplateCategory.FinancialSummary,
                    "You are a wealth advisor summarizing overall finances.",
                    "Summarize the user's financial position. Question: {{message}}"),
                CreatePrompt(
                    InvestmentAnalysisPromptId,
                    "investment-analysis",
                    "Investment Analysis",
                    PromptTemplateCategory.InvestmentAnalysis,
                    "You analyze investment portfolios conservatively.",
                    "Analyze investments for: {{message}}"),
                CreatePrompt(
                    LoanAnalysisPromptId,
                    "loan-analysis",
                    "Loan Analysis",
                    PromptTemplateCategory.LoanAnalysis,
                    "You analyze loans, EMIs, and debt risk.",
                    "Analyze loans for: {{message}}"),
                CreatePrompt(
                    CashFlowAnalysisPromptId,
                    "cash-flow-analysis",
                    "Cash Flow Analysis",
                    PromptTemplateCategory.CashFlowAnalysis,
                    "You analyze income vs expenses and cash flow.",
                    "Analyze cash flow for: {{message}}"),
                CreatePrompt(
                    GoalPlanningPromptId,
                    "goal-planning",
                    "Goal Planning",
                    PromptTemplateCategory.GoalPlanning,
                    "You help plan and prioritize financial goals.",
                    "Plan goals for: {{message}}"),
                CreatePrompt(
                    BusinessAnalysisPromptId,
                    "business-analysis",
                    "Business Analysis",
                    PromptTemplateCategory.BusinessAnalysis,
                    "You analyze business income and client projects.",
                    "Analyze business finances for: {{message}}"),
                CreatePrompt(
                    DocumentSearchPromptId,
                    "document-search",
                    "Document Search",
                    PromptTemplateCategory.DocumentSearch,
                    "You help locate and explain document findings.",
                    "Search documents related to: {{message}}"));

            logger.LogInformation("Seeded {Count} AI prompt templates", 7);
        }

        if (!dbContext.AITools.Any())
        {
            dbContext.AITools.AddRange(
                CreateTool("get_dashboard_summary", "Get Dashboard Summary", AIToolCategory.Dashboard, 1),
                CreateTool("get_property_summary", "Get Property Summary", AIToolCategory.Property, 2),
                CreateTool("get_loan_summary", "Get Loan Summary", AIToolCategory.Loan, 3),
                CreateTool("get_investment_summary", "Get Investment Summary", AIToolCategory.Investment, 4),
                CreateTool("get_income_summary", "Get Income Summary", AIToolCategory.Income, 5),
                CreateTool("get_goal_summary", "Get Goal Summary", AIToolCategory.Goal, 6),
                CreateTool("search_documents", "Search Documents", AIToolCategory.Document, 7),
                CreateTool("get_notifications", "Get Notifications", AIToolCategory.Notification, 8));

            logger.LogInformation("Seeded {Count} AI tool catalog entries", 8);
        }

        await dbContext.SaveChangesAsync();
    }

    private static PromptTemplate CreatePrompt(
        Guid id,
        string code,
        string name,
        PromptTemplateCategory category,
        string systemPrompt,
        string userTemplate) =>
        new(id)
        {
            Code = code,
            Name = name,
            Category = category,
            SystemPrompt = systemPrompt,
            UserPromptTemplate = userTemplate,
            Description = $"{name} template (architecture placeholder).",
            IsActive = true,
            Version = 1,
        };

    private static AITool CreateTool(
        string code,
        string name,
        AIToolCategory category,
        int sortOrder) =>
        new()
        {
            Code = code,
            Name = name,
            Description = $"{name} tool catalog entry.",
            Category = category,
            IsEnabled = true,
            SortOrder = sortOrder,
        };
}

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WealthOS.Application.AI.DTOs.Requests;
using WealthOS.Application.AI.DTOs.Responses;
using WealthOS.Application.AI.Interfaces;
using WealthOS.Application.AI.Options;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Domain.AI.Entities;
using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.AI.Repositories;
using WealthOS.Domain.Common.Abstractions.Repositories;

namespace WealthOS.Application.AI.Services;

/// <summary>
/// Orchestrates the AI advisor platform: context → tools → provider stub → persistence.
/// Does not call external LLM APIs; returns structured placeholder responses.
/// </summary>
public sealed class AIService : IAIService
{
    private readonly IAIConversationRepository _conversationRepository;
    private readonly IAIMessageRepository _messageRepository;
    private readonly IConversationSessionRepository _sessionRepository;
    private readonly IAIToolExecutionRepository _toolExecutionRepository;
    private readonly IAIInsightRepository _insightRepository;
    private readonly IAIContextBuilder _contextBuilder;
    private readonly IAIToolRegistry _toolRegistry;
    private readonly IAIPromptService _promptService;
    private readonly IAIRecommendationService _recommendationService;
    private readonly IEnumerable<IAIProvider> _providers;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly AIOptions _options;

    public AIService(
        IAIConversationRepository conversationRepository,
        IAIMessageRepository messageRepository,
        IConversationSessionRepository sessionRepository,
        IAIToolExecutionRepository toolExecutionRepository,
        IAIInsightRepository insightRepository,
        IAIContextBuilder contextBuilder,
        IAIToolRegistry toolRegistry,
        IAIPromptService promptService,
        IAIRecommendationService recommendationService,
        IEnumerable<IAIProvider> providers,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IOptions<AIOptions> options)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _sessionRepository = sessionRepository;
        _toolExecutionRepository = toolExecutionRepository;
        _insightRepository = insightRepository;
        _contextBuilder = contextBuilder;
        _toolRegistry = toolRegistry;
        _promptService = promptService;
        _recommendationService = recommendationService;
        _providers = providers;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _options = options.Value;
    }

    public async Task<Result<AIChatResponse>> StartConversationAsync(
        StartConversationRequest? request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<AIChatResponse>(userResult.Error!);
        }

        var providerKind = request?.PreferredProvider ?? ParseProviderKind(_options.DefaultProvider);
        var conversation = new AIConversation
        {
            UserId = userResult.Value,
            Title = string.IsNullOrWhiteSpace(request?.Title) ? "New conversation" : request!.Title!.Trim(),
            Status = AIConversationStatus.Active,
            ProviderKind = providerKind,
            LastMessageAt = DateTime.UtcNow,
        };

        var session = new ConversationSession
        {
            ConversationId = conversation.Id,
            UserId = userResult.Value,
            SessionKey = Guid.NewGuid().ToString("N"),
            Status = ConversationSessionStatus.Open,
            StartedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(12),
        };

        var systemMessage = new AIMessage
        {
            ConversationId = conversation.Id,
            Role = AIMessageRole.System,
            Content = "WealthOS AI Financial Advisor (architecture placeholder).",
            Sequence = 1,
        };

        await _conversationRepository.AddAsync(conversation, cancellationToken);
        await _sessionRepository.AddAsync(session, cancellationToken);
        await _messageRepository.AddAsync(systemMessage, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AIChatResponse
        {
            ConversationId = conversation.Id,
            SessionId = session.Id,
            UserMessageId = Guid.Empty,
            AssistantMessageId = systemMessage.Id,
            Reply = "Conversation started. Ask about your dashboard, loans, investments, goals, or documents.",
            Provider = providerKind.ToString(),
            IsPlaceholder = true,
            ModulesUsed = Array.Empty<string>(),
        });
    }

    public async Task<Result<AIChatResponse>> SendMessageAsync(
        SendMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<AIChatResponse>(userResult.Error!);
        }

        var conversationResult = await ResolveConversationAsync(
            userResult.Value,
            request.ConversationId,
            cancellationToken);
        if (conversationResult.IsFailure)
        {
            return Result.Failure<AIChatResponse>(conversationResult.Error!);
        }

        var conversation = conversationResult.Value;
        var session = await EnsureOpenSessionAsync(conversation, userResult.Value, cancellationToken);

        var contextResult = await _contextBuilder.BuildAsync(
            userResult.Value,
            conversation.Id,
            cancellationToken);
        if (contextResult.IsFailure)
        {
            return Result.Failure<AIChatResponse>(contextResult.Error!);
        }

        var context = contextResult.Value;
        var toolContext = new AIToolExecutionContext
        {
            UserId = userResult.Value,
            ConversationId = conversation.Id,
            UserMessage = request.Message,
        };

        var toolResults = await _toolRegistry.ExecuteRelevantAsync(toolContext, cancellationToken);
        await PersistToolExecutionsAsync(conversation.Id, toolResults, cancellationToken);

        var systemPrompt = "You are the WealthOS AI Financial Advisor orchestration layer.";
        var userPrompt = request.Message;
        if (!string.IsNullOrWhiteSpace(request.PromptTemplateCode))
        {
            var rendered = await _promptService.RenderAsync(
                request.PromptTemplateCode,
                new Dictionary<string, string> { ["message"] = request.Message },
                cancellationToken);
            if (rendered.IsSuccess)
            {
                userPrompt = rendered.Value;
            }

            var template = await _promptService.GetByCodeAsync(
                request.PromptTemplateCode,
                cancellationToken);
            if (template.IsSuccess)
            {
                systemPrompt = template.Value.SystemPrompt;
            }
        }

        var provider = ResolveProvider(conversation.ProviderKind);
        var providerResult = await provider.GenerateResponseAsync(
            new AIProviderRequest
            {
                SystemPrompt = systemPrompt,
                UserPrompt = userPrompt,
                ContextJson = context.ContextJson,
                ToolResults = toolResults,
            },
            cancellationToken);

        var reply = providerResult.IsSuccess
            ? providerResult.Value
            : BuildFallbackReply(request.Message, context, toolResults);

        var sequence = await _messageRepository.GetNextSequenceAsync(conversation.Id, cancellationToken);
        var userMessage = new AIMessage
        {
            ConversationId = conversation.Id,
            Role = AIMessageRole.User,
            Content = request.Message,
            Sequence = sequence,
        };
        var assistantMessage = new AIMessage
        {
            ConversationId = conversation.Id,
            Role = AIMessageRole.Assistant,
            Content = reply,
            Sequence = sequence + 1,
            MetadataJson = JsonSerializer.Serialize(new
            {
                provider = provider.Kind.ToString(),
                isPlaceholder = true,
                tools = toolResults.Select(t => t.ToolCode).ToList(),
            }),
        };

        conversation.LastMessageAt = DateTime.UtcNow;
        if (conversation.Title == "New conversation")
        {
            conversation.Title = Truncate(request.Message, 80);
        }

        await _messageRepository.AddAsync(userMessage, cancellationToken);
        await _messageRepository.AddAsync(assistantMessage, cancellationToken);
        _conversationRepository.Update(conversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AIChatResponse
        {
            ConversationId = conversation.Id,
            SessionId = session.Id,
            UserMessageId = userMessage.Id,
            AssistantMessageId = assistantMessage.Id,
            Reply = reply,
            Provider = provider.Kind.ToString(),
            IsPlaceholder = true,
            ToolExecutions = toolResults.Select(t => new AIToolExecutionResponse
            {
                ToolCode = t.ToolCode,
                ToolName = t.ToolName,
                Status = t.Succeeded ? AIToolExecutionStatus.Succeeded : AIToolExecutionStatus.Failed,
                Summary = t.Summary,
                DurationMs = 0,
            }).ToList(),
            ModulesUsed = context.ModulesIncluded,
        });
    }

    public async Task<Result> ClearConversationAsync(CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error!);
        }

        await _conversationRepository.ClearActiveForUserAsync(userResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<AIConversationResponse>> GetConversationAsync(
        Guid? conversationId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<AIConversationResponse>(userResult.Error!);
        }

        AIConversation? conversation;
        if (conversationId.HasValue)
        {
            conversation = await _conversationRepository.GetByIdForUserAsync(
                conversationId.Value,
                userResult.Value,
                cancellationToken);
        }
        else
        {
            conversation = await _conversationRepository.GetActiveForUserAsync(
                userResult.Value,
                cancellationToken);
        }

        if (conversation is null)
        {
            return Result.Failure<AIConversationResponse>(
                Error.NotFound(nameof(AIConversation), conversationId ?? Guid.Empty));
        }

        var messages = await _messageRepository.ListForConversationAsync(
            conversation.Id,
            cancellationToken);

        return Result.Success(new AIConversationResponse
        {
            Id = conversation.Id,
            Title = conversation.Title,
            Status = conversation.Status,
            ProviderKind = conversation.ProviderKind,
            Summary = conversation.Summary,
            LastMessageAt = conversation.LastMessageAt,
            CreatedAt = conversation.CreatedAt,
            Messages = messages.Select(m => new AIMessageResponse
            {
                Id = m.Id,
                Role = m.Role,
                Content = m.Content,
                Sequence = m.Sequence,
                CreatedAt = m.CreatedAt,
            }).ToList(),
        });
    }

    public async Task<Result<AIConversationHistoryResponse>> GetHistoryAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<AIConversationHistoryResponse>(userResult.Error!);
        }

        var (items, total) = await _conversationRepository.ListSummariesForUserAsync(
            userResult.Value,
            page,
            pageSize,
            cancellationToken);

        return Result.Success(new AIConversationHistoryResponse
        {
            Items = items.Select(i => new AIConversationSummaryResponse
            {
                Id = i.Id,
                Title = i.Title,
                Status = i.Status,
                LastMessageAt = i.LastMessageAt,
                MessageCount = i.MessageCount,
            }).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
        });
    }

    public Task<Result<AISuggestionsResponse>> GetSuggestionsAsync(
        CancellationToken cancellationToken = default)
    {
        var suggestions = new[]
        {
            "Summarize my net worth and dashboard",
            "How are my loans and EMIs looking?",
            "Analyze my investment portfolio",
            "What is my monthly cash flow?",
            "Show progress on my financial goals",
            "Find recent or expiring documents",
            "Any important notifications I should know about?",
        };

        return Task.FromResult(Result.Success(new AISuggestionsResponse
        {
            Suggestions = suggestions,
        }));
    }

    public async Task<Result<AIInsightsResponse>> GetInsightsAsync(
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<AIInsightsResponse>(userResult.Error!);
        }

        var summary = await _insightRepository.GetSummaryForUserAsync(
            userResult.Value,
            cancellationToken);

        if (summary.TotalCount == 0 && summary.ActiveRecommendationCount == 0)
        {
            var contextResult = await _contextBuilder.BuildAsync(
                userResult.Value,
                null,
                cancellationToken);
            if (contextResult.IsSuccess)
            {
                _ = await _recommendationService.GeneratePlaceholderAsync(
                    contextResult.Value,
                    cancellationToken);
                summary = await _insightRepository.GetSummaryForUserAsync(
                    userResult.Value,
                    cancellationToken);
            }
        }

        var insights = await _insightRepository.ListForUserAsync(
            userResult.Value,
            20,
            cancellationToken);
        var recommendations = await _recommendationService.GetActiveAsync(10, cancellationToken);

        return Result.Success(new AIInsightsResponse
        {
            TotalInsightCount = summary.TotalCount,
            HighSeverityCount = summary.HighSeverityCount,
            ActiveRecommendationCount = summary.ActiveRecommendationCount,
            Insights = insights.Select(i => new AIInsightItemResponse
            {
                Id = i.Id,
                Title = i.Title,
                Body = i.Body,
                Module = i.Module,
                Severity = i.Severity,
                CreatedAt = i.CreatedAt,
            }).ToList(),
            Recommendations = recommendations.IsSuccess
                ? recommendations.Value
                : Array.Empty<AIRecommendationResponse>(),
        });
    }

    private async Task<Result<AIConversation>> ResolveConversationAsync(
        Guid userId,
        Guid? conversationId,
        CancellationToken cancellationToken)
    {
        if (conversationId.HasValue)
        {
            var existing = await _conversationRepository.GetByIdForUserAsync(
                conversationId.Value,
                userId,
                cancellationToken);
            if (existing is null || existing.Status == AIConversationStatus.Cleared)
            {
                return Result.Failure<AIConversation>(
                    Error.NotFound(nameof(AIConversation), conversationId.Value));
            }

            return Result.Success(existing);
        }

        var active = await _conversationRepository.GetActiveForUserAsync(userId, cancellationToken);
        if (active is not null)
        {
            return Result.Success(active);
        }

        var started = await StartConversationAsync(new StartConversationRequest(), cancellationToken);
        if (started.IsFailure)
        {
            return Result.Failure<AIConversation>(started.Error!);
        }

        var created = await _conversationRepository.GetByIdForUserAsync(
            started.Value.ConversationId,
            userId,
            cancellationToken);

        return created is null
            ? Result.Failure<AIConversation>(Error.Failure("ai_conversation", "Failed to create conversation."))
            : Result.Success(created);
    }

    private async Task<ConversationSession> EnsureOpenSessionAsync(
        AIConversation conversation,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var open = await _sessionRepository.GetOpenSessionAsync(userId, cancellationToken);
        if (open is not null && open.ConversationId == conversation.Id)
        {
            return open;
        }

        if (open is not null)
        {
            open.Status = ConversationSessionStatus.Closed;
            open.EndedAt = DateTime.UtcNow;
            _sessionRepository.Update(open);
        }

        var session = new ConversationSession
        {
            ConversationId = conversation.Id,
            UserId = userId,
            SessionKey = Guid.NewGuid().ToString("N"),
            Status = ConversationSessionStatus.Open,
            StartedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(12),
        };

        await _sessionRepository.AddAsync(session, cancellationToken);
        return session;
    }

    private async Task PersistToolExecutionsAsync(
        Guid conversationId,
        IReadOnlyList<AIToolResultDto> toolResults,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        foreach (var tool in toolResults)
        {
            await _toolExecutionRepository.AddAsync(
                new AIToolExecution
                {
                    ConversationId = conversationId,
                    ToolCode = tool.ToolCode,
                    Status = tool.Succeeded
                        ? AIToolExecutionStatus.Succeeded
                        : AIToolExecutionStatus.Failed,
                    InputJson = null,
                    OutputJson = tool.PayloadJson,
                    ErrorMessage = tool.Error,
                    DurationMs = 0,
                    StartedAt = now,
                    CompletedAt = now,
                },
                cancellationToken);
        }
    }

    private IAIProvider ResolveProvider(AIProviderKind kind)
    {
        var match = _providers.FirstOrDefault(p => p.Kind == kind);
        if (match is not null)
        {
            return match;
        }

        var defaultKind = ParseProviderKind(_options.DefaultProvider);
        return _providers.FirstOrDefault(p => p.Kind == defaultKind)
            ?? _providers.First();
    }

    private static AIProviderKind ParseProviderKind(string? value) =>
        Enum.TryParse<AIProviderKind>(value, ignoreCase: true, out var kind)
            ? kind
            : AIProviderKind.OpenAI;

    private static string BuildFallbackReply(
        string message,
        AIContextSnapshot context,
        IReadOnlyList<AIToolResultDto> toolResults)
    {
        var sb = new StringBuilder();
        sb.AppendLine("[WealthOS AI Placeholder]");
        sb.AppendLine($"You asked: {message}");
        sb.AppendLine($"Context modules: {string.Join(", ", context.ModulesIncluded)}");
        if (toolResults.Count > 0)
        {
            sb.AppendLine("Tool results:");
            foreach (var tool in toolResults)
            {
                sb.AppendLine($"- {tool.ToolName}: {tool.Summary}");
            }
        }

        sb.Append("No external LLM was called. This is an orchestration architecture response.");
        return sb.ToString();
    }

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }

    private static string Truncate(string value, int max) =>
        value.Length <= max ? value : value[..max];
}

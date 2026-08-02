using WealthOS.Application.AI.DTOs.Responses;
using WealthOS.Application.AI.Interfaces;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Domain.AI.Entities;
using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.AI.Repositories;
using WealthOS.Domain.Common.Abstractions.Repositories;

namespace WealthOS.Application.AI.Services;

/// <summary>
/// Generates and lists placeholder recommendations from context (no LLM).
/// </summary>
public sealed class AIRecommendationService : IAIRecommendationService
{
    private readonly IAIRecommendationRepository _recommendationRepository;
    private readonly IAIInsightRepository _insightRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public AIRecommendationService(
        IAIRecommendationRepository recommendationRepository,
        IAIInsightRepository insightRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _recommendationRepository = recommendationRepository;
        _insightRepository = insightRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<AIRecommendationResponse>>> GetActiveAsync(
        int take = 10,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<IReadOnlyList<AIRecommendationResponse>>(userResult.Error!);
        }

        var items = await _recommendationRepository.ListActiveForUserAsync(
            userResult.Value,
            take,
            cancellationToken);

        return Result.Success<IReadOnlyList<AIRecommendationResponse>>(
            items.Select(Map).ToList());
    }

    public async Task<Result<IReadOnlyList<AIRecommendationResponse>>> GeneratePlaceholderAsync(
        AIContextSnapshot context,
        CancellationToken cancellationToken = default)
    {
        var recommendations = new List<AIRecommendation>
        {
            new()
            {
                UserId = context.UserId,
                ConversationId = context.ConversationId,
                Title = "Review net worth trend",
                Body = "Placeholder: review your dashboard net worth and cash flow this month.",
                Category = "Dashboard",
                Status = AIRecommendationStatus.Active,
                Confidence = 0.55,
            },
            new()
            {
                UserId = context.UserId,
                ConversationId = context.ConversationId,
                Title = "Check upcoming obligations",
                Body = "Placeholder: verify loan EMIs and document expiry reminders.",
                Category = "Planning",
                Status = AIRecommendationStatus.Active,
                Confidence = 0.5,
            },
        };

        var insight = new AIInsight
        {
            UserId = context.UserId,
            ConversationId = context.ConversationId,
            Title = "AI platform context ready",
            Body =
                $"Placeholder insight built from modules: {string.Join(", ", context.ModulesIncluded)}.",
            Module = "AI",
            Severity = AIInsightSeverity.Info,
        };

        foreach (var recommendation in recommendations)
        {
            await _recommendationRepository.AddAsync(recommendation, cancellationToken);
        }

        await _insightRepository.AddAsync(insight, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success<IReadOnlyList<AIRecommendationResponse>>(
            recommendations.Select(Map).ToList());
    }

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }

    private static AIRecommendationResponse Map(AIRecommendation recommendation) =>
        new()
        {
            Id = recommendation.Id,
            Title = recommendation.Title,
            Body = recommendation.Body,
            Category = recommendation.Category,
            Status = recommendation.Status,
            Confidence = recommendation.Confidence,
        };
}

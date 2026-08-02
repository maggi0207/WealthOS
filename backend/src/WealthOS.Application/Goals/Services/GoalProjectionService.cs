using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Goals.Calculations;
using WealthOS.Application.Goals.DTOs.Responses;
using WealthOS.Application.Goals.Interfaces;
using WealthOS.Domain.Goals.Entities;
using WealthOS.Domain.Goals.Repositories;

namespace WealthOS.Application.Goals.Services;

/// <summary>
/// Builds goal projections and placeholder recommendations (no AI in Phase 8).
/// </summary>
public sealed class GoalProjectionService : IGoalProjectionService
{
    private readonly IFinancialGoalRepository _goalRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IGoalCalculationService _calculator;

    public GoalProjectionService(
        IFinancialGoalRepository goalRepository,
        ICurrentUserService currentUser,
        IGoalCalculationService calculator)
    {
        _goalRepository = goalRepository;
        _currentUser = currentUser;
        _calculator = calculator;
    }

    public async Task<Result<GoalProjectionResponse>> GetProjectionAsync(
        Guid goalId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<GoalProjectionResponse>(userResult.Error!);
        }

        var goal = await _goalRepository.GetByIdForUserAsync(goalId, userResult.Value, cancellationToken);
        if (goal is null)
        {
            return Result.Failure<GoalProjectionResponse>(Error.NotFound(nameof(FinancialGoal), goalId));
        }

        var projection = _calculator.BuildProjection(
            goal.Id,
            goal.Name,
            goal.TargetAmount,
            goal.CurrentAmount,
            goal.MonthlyContribution,
            goal.TargetDate,
            DateOnly.FromDateTime(DateTime.UtcNow),
            goal.CurrencyCode);

        return Result.Success(new GoalProjectionResponse
        {
            GoalId = projection.GoalId,
            GoalName = projection.GoalName,
            TargetAmount = projection.TargetAmount,
            CurrentAmount = projection.CurrentAmount,
            MonthlyContribution = projection.MonthlyContribution,
            TargetDate = projection.TargetDate,
            EstimatedCompletionDate = projection.EstimatedCompletionDate,
            MonthlyRequiredContribution = projection.MonthlyRequiredContribution,
            ProjectedAmountAtTargetDate = projection.ProjectedAmountAtTargetDate,
            ShortfallAtTargetDate = projection.ShortfallAtTargetDate,
            IsOnTrack = projection.IsOnTrack,
            Points = projection.Points.Select(point => new GoalProjectionPointResponse
            {
                AsOf = point.AsOf,
                Label = point.Label,
                ProjectedAmount = point.ProjectedAmount,
            }).ToList(),
            CurrencyCode = projection.CurrencyCode,
        });
    }

    /// <inheritdoc />
    /// <remarks>
    /// Phase 8 intentionally returns an empty list. Future AI / rule engines can plug in here.
    /// </remarks>
    public Task<Result<IReadOnlyList<GoalRecommendationResponse>>> GetRecommendationsAsync(
        Guid? goalId = null,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Task.FromResult(
                Result.Failure<IReadOnlyList<GoalRecommendationResponse>>(userResult.Error!));
        }

        // Extension point: return empty stubs (no AI / notifications in Phase 8).
        IReadOnlyList<GoalRecommendationResponse> empty = Array.Empty<GoalRecommendationResponse>();
        return Task.FromResult(Result.Success(empty));
    }

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }
}

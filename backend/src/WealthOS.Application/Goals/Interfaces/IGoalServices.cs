using WealthOS.Application.Common.Models;
using WealthOS.Application.Goals.DTOs.Requests;
using WealthOS.Application.Goals.DTOs.Responses;
using WealthOS.Domain.Goals.Enums;

namespace WealthOS.Application.Goals.Interfaces;

/// <summary>
/// Application service for goal CRUD, contributions, milestones, and dashboard.
/// </summary>
public interface IGoalService
{
    Task<Result<GoalResponse>> CreateAsync(
        CreateGoalRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<GoalResponse>> UpdateAsync(
        Guid goalId,
        UpdateGoalRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Guid goalId, CancellationToken cancellationToken = default);

    Task<Result<GoalContributionResponse>> RecordContributionAsync(
        Guid goalId,
        RecordGoalContributionRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<GoalMilestoneResponse>> CompleteMilestoneAsync(
        Guid goalId,
        Guid milestoneId,
        CompleteMilestoneRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<GoalResponse>> GetByIdAsync(
        Guid goalId,
        CancellationToken cancellationToken = default);

    Task<Result<GoalListResponse>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        GoalStatus? status,
        GoalCategory? category,
        GoalPriority? priority,
        CancellationToken cancellationToken = default);

    Task<Result<GoalProgressResponse>> GetProgressAsync(
        Guid goalId,
        CancellationToken cancellationToken = default);

    Task<Result<GoalDashboardResponse>> GetDashboardAsync(
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Projection and recommendation extension-point service.
/// </summary>
public interface IGoalProjectionService
{
    Task<Result<GoalProjectionResponse>> GetProjectionAsync(
        Guid goalId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns empty / static stub recommendations (Phase 8 — no AI).
    /// </summary>
    Task<Result<IReadOnlyList<GoalRecommendationResponse>>> GetRecommendationsAsync(
        Guid? goalId = null,
        CancellationToken cancellationToken = default);
}

using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Goals.DTOs.Responses;
using WealthOS.Application.Goals.Interfaces;
using WealthOS.Application.Goals.Queries;

namespace WealthOS.Application.Goals.Queries.Handlers;

public sealed class GetGoalsQueryHandler : IQueryHandler<GetGoalsQuery, GoalListResponse>
{
    private readonly IGoalService _goalService;

    public GetGoalsQueryHandler(IGoalService goalService)
    {
        _goalService = goalService;
    }

    public Task<Result<GoalListResponse>> HandleAsync(
        GetGoalsQuery query,
        CancellationToken cancellationToken = default) =>
        _goalService.GetAllAsync(
            query.Page,
            query.PageSize,
            query.Search,
            query.Status,
            query.Category,
            query.Priority,
            cancellationToken);
}

public sealed class GetGoalByIdQueryHandler : IQueryHandler<GetGoalByIdQuery, GoalResponse>
{
    private readonly IGoalService _goalService;

    public GetGoalByIdQueryHandler(IGoalService goalService)
    {
        _goalService = goalService;
    }

    public Task<Result<GoalResponse>> HandleAsync(
        GetGoalByIdQuery query,
        CancellationToken cancellationToken = default) =>
        _goalService.GetByIdAsync(query.GoalId, cancellationToken);
}

public sealed class GetGoalProgressQueryHandler
    : IQueryHandler<GetGoalProgressQuery, GoalProgressResponse>
{
    private readonly IGoalService _goalService;

    public GetGoalProgressQueryHandler(IGoalService goalService)
    {
        _goalService = goalService;
    }

    public Task<Result<GoalProgressResponse>> HandleAsync(
        GetGoalProgressQuery query,
        CancellationToken cancellationToken = default) =>
        _goalService.GetProgressAsync(query.GoalId, cancellationToken);
}

public sealed class GetGoalDashboardQueryHandler
    : IQueryHandler<GetGoalDashboardQuery, GoalDashboardResponse>
{
    private readonly IGoalService _goalService;

    public GetGoalDashboardQueryHandler(IGoalService goalService)
    {
        _goalService = goalService;
    }

    public Task<Result<GoalDashboardResponse>> HandleAsync(
        GetGoalDashboardQuery query,
        CancellationToken cancellationToken = default) =>
        _goalService.GetDashboardAsync(cancellationToken);
}

public sealed class GetGoalProjectionQueryHandler
    : IQueryHandler<GetGoalProjectionQuery, GoalProjectionResponse>
{
    private readonly IGoalProjectionService _projectionService;

    public GetGoalProjectionQueryHandler(IGoalProjectionService projectionService)
    {
        _projectionService = projectionService;
    }

    public Task<Result<GoalProjectionResponse>> HandleAsync(
        GetGoalProjectionQuery query,
        CancellationToken cancellationToken = default) =>
        _projectionService.GetProjectionAsync(query.GoalId, cancellationToken);
}

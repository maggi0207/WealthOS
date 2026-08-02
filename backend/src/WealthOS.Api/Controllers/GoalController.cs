using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Goals.Commands;
using WealthOS.Application.Goals.DTOs.Requests;
using WealthOS.Application.Goals.DTOs.Responses;
using WealthOS.Application.Goals.Queries;
using WealthOS.Domain.Goals.Enums;

namespace WealthOS.Api.Controllers;

/// <summary>
/// Goal planning endpoints (CRUD, contributions, milestones, progress, projection, dashboard).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/goals")]
public sealed class GoalController : ControllerBase
{
    private readonly ICommandHandler<CreateGoalCommand, GoalResponse> _createHandler;
    private readonly ICommandHandler<UpdateGoalCommand, GoalResponse> _updateHandler;
    private readonly ICommandHandler<DeleteGoalCommand> _deleteHandler;
    private readonly ICommandHandler<RecordContributionCommand, GoalContributionResponse> _recordContributionHandler;
    private readonly ICommandHandler<CompleteMilestoneCommand, GoalMilestoneResponse> _completeMilestoneHandler;
    private readonly IQueryHandler<GetGoalsQuery, GoalListResponse> _getAllHandler;
    private readonly IQueryHandler<GetGoalByIdQuery, GoalResponse> _getByIdHandler;
    private readonly IQueryHandler<GetGoalProgressQuery, GoalProgressResponse> _progressHandler;
    private readonly IQueryHandler<GetGoalDashboardQuery, GoalDashboardResponse> _dashboardHandler;
    private readonly IQueryHandler<GetGoalProjectionQuery, GoalProjectionResponse> _projectionHandler;

    /// <summary>
    /// Creates a new <see cref="GoalController"/>.
    /// </summary>
    public GoalController(
        ICommandHandler<CreateGoalCommand, GoalResponse> createHandler,
        ICommandHandler<UpdateGoalCommand, GoalResponse> updateHandler,
        ICommandHandler<DeleteGoalCommand> deleteHandler,
        ICommandHandler<RecordContributionCommand, GoalContributionResponse> recordContributionHandler,
        ICommandHandler<CompleteMilestoneCommand, GoalMilestoneResponse> completeMilestoneHandler,
        IQueryHandler<GetGoalsQuery, GoalListResponse> getAllHandler,
        IQueryHandler<GetGoalByIdQuery, GoalResponse> getByIdHandler,
        IQueryHandler<GetGoalProgressQuery, GoalProgressResponse> progressHandler,
        IQueryHandler<GetGoalDashboardQuery, GoalDashboardResponse> dashboardHandler,
        IQueryHandler<GetGoalProjectionQuery, GoalProjectionResponse> projectionHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _recordContributionHandler = recordContributionHandler;
        _completeMilestoneHandler = completeMilestoneHandler;
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _progressHandler = progressHandler;
        _dashboardHandler = dashboardHandler;
        _projectionHandler = projectionHandler;
    }

    /// <summary>
    /// Returns a paginated list of goals for the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GoalListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] GoalStatus? status = null,
        [FromQuery] GoalCategory? category = null,
        [FromQuery] GoalPriority? priority = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _getAllHandler.HandleAsync(
            new GetGoalsQuery
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                Status = status,
                Category = category,
                Priority = priority,
            },
            cancellationToken);

        return ToActionResult(result, "Goals retrieved.");
    }

    /// <summary>
    /// Returns goals module dashboard summary (active/completed counts, overall progress, upcoming milestones).
    /// Registered before <c>{id}</c> to avoid route conflicts.
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ApiResponse<GoalDashboardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var result = await _dashboardHandler.HandleAsync(new GetGoalDashboardQuery(), cancellationToken);
        return ToActionResult(result, "Goal dashboard retrieved.");
    }

    /// <summary>
    /// Returns a single goal by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GoalResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.HandleAsync(
            new GetGoalByIdQuery { GoalId = id },
            cancellationToken);

        return ToActionResult(result, "Goal retrieved.");
    }

    /// <summary>
    /// Returns computed progress for a goal (completion %, remaining, trend, required contribution).
    /// </summary>
    [HttpGet("{id:guid}/progress")]
    [ProducesResponseType(typeof(ApiResponse<GoalProgressResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProgress(Guid id, CancellationToken cancellationToken)
    {
        var result = await _progressHandler.HandleAsync(
            new GetGoalProgressQuery { GoalId = id },
            cancellationToken);

        return ToActionResult(result, "Goal progress retrieved.");
    }

    /// <summary>
    /// Returns a contribution projection series for a goal.
    /// </summary>
    [HttpGet("{id:guid}/projection")]
    [ProducesResponseType(typeof(ApiResponse<GoalProjectionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjection(Guid id, CancellationToken cancellationToken)
    {
        var result = await _projectionHandler.HandleAsync(
            new GetGoalProjectionQuery { GoalId = id },
            cancellationToken);

        return ToActionResult(result, "Goal projection retrieved.");
    }

    /// <summary>
    /// Creates a new financial goal.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<GoalResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateGoalRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreateGoalCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<GoalResponse>.Ok(result.Value, "Goal created."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Updates an existing financial goal.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GoalResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateGoalRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateHandler.HandleAsync(
            new UpdateGoalCommand { GoalId = id, Request = request },
            cancellationToken);

        return ToActionResult(result, "Goal updated.");
    }

    /// <summary>
    /// Soft-deletes a financial goal.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _deleteHandler.HandleAsync(
            new DeleteGoalCommand { GoalId = id },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object>.Ok(null!, "Goal deleted."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Records a contribution against a goal and updates current amount.
    /// </summary>
    [HttpPost("{id:guid}/contributions")]
    [ProducesResponseType(typeof(ApiResponse<GoalContributionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecordContribution(
        Guid id,
        [FromBody] RecordGoalContributionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _recordContributionHandler.HandleAsync(
            new RecordContributionCommand { GoalId = id, Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<GoalContributionResponse>.Ok(result.Value, "Contribution recorded."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Marks a milestone as completed.
    /// </summary>
    [HttpPost("{id:guid}/milestones/{milestoneId:guid}/complete")]
    [ProducesResponseType(typeof(ApiResponse<GoalMilestoneResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CompleteMilestone(
        Guid id,
        Guid milestoneId,
        [FromBody] CompleteMilestoneRequest? request,
        CancellationToken cancellationToken)
    {
        var result = await _completeMilestoneHandler.HandleAsync(
            new CompleteMilestoneCommand
            {
                GoalId = id,
                MilestoneId = milestoneId,
                Request = request ?? new CompleteMilestoneRequest(),
            },
            cancellationToken);

        return ToActionResult(result, "Milestone completed.");
    }

    private IActionResult ToActionResult<T>(Result<T> result, string successMessage)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.Ok(result.Value, successMessage));
        }

        return ToFailureResult(result.Error!);
    }

    private IActionResult ToFailureResult(Error error)
    {
        var errors = new List<ApiErrorDetail>();

        if (error.ValidationErrors is not null)
        {
            foreach (var (field, messages) in error.ValidationErrors)
            {
                errors.AddRange(messages.Select(message => new ApiErrorDetail
                {
                    Code = error.Code,
                    Message = message,
                    Field = field,
                }));
            }
        }
        else
        {
            errors.Add(new ApiErrorDetail
            {
                Code = error.Code,
                Message = error.Message,
            });
        }

        var payload = ApiResponse<object>.Fail(error.Message, errors);

        return error.Code switch
        {
            "unauthorized" => Unauthorized(payload),
            "forbidden" => StatusCode(StatusCodes.Status403Forbidden, payload),
            "not_found" => NotFound(payload),
            "conflict" => Conflict(payload),
            "validation_error" => UnprocessableEntity(payload),
            _ => BadRequest(payload),
        };
    }
}

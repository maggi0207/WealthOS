using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.DTOs.Responses;
using WealthOS.Application.Dashboard.Queries;

namespace WealthOS.Api.Controllers;

/// <summary>
/// Aggregated portfolio dashboard endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IQueryHandler<GetDashboardSummaryQuery, DashboardResponse> _summaryHandler;
    private readonly IQueryHandler<GetNetWorthQuery, NetWorthResponse> _netWorthHandler;
    private readonly IQueryHandler<GetRecentActivitiesQuery, IReadOnlyList<RecentActivityResponse>> _activitiesHandler;
    private readonly IQueryHandler<GetDashboardHealthQuery, DashboardHealthResponse> _healthHandler;

    /// <summary>
    /// Creates a new <see cref="DashboardController"/>.
    /// </summary>
    public DashboardController(
        IQueryHandler<GetDashboardSummaryQuery, DashboardResponse> summaryHandler,
        IQueryHandler<GetNetWorthQuery, NetWorthResponse> netWorthHandler,
        IQueryHandler<GetRecentActivitiesQuery, IReadOnlyList<RecentActivityResponse>> activitiesHandler,
        IQueryHandler<GetDashboardHealthQuery, DashboardHealthResponse> healthHandler)
    {
        _summaryHandler = summaryHandler;
        _netWorthHandler = netWorthHandler;
        _activitiesHandler = activitiesHandler;
        _healthHandler = healthHandler;
    }

    /// <summary>
    /// Returns the full dashboard summary for the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<DashboardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await _summaryHandler.HandleAsync(new GetDashboardSummaryQuery(), cancellationToken);
        return ToActionResult(result, "Dashboard summary retrieved.");
    }

    /// <summary>
    /// Returns net-worth, asset, and liability totals.
    /// </summary>
    [HttpGet("net-worth")]
    [ProducesResponseType(typeof(ApiResponse<NetWorthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNetWorth(CancellationToken cancellationToken)
    {
        var result = await _netWorthHandler.HandleAsync(new GetNetWorthQuery(), cancellationToken);
        return ToActionResult(result, "Net worth retrieved.");
    }

    /// <summary>
    /// Returns recent portfolio activities.
    /// </summary>
    /// <param name="limit">Maximum number of activities to return (1–50).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("activities")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RecentActivityResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetActivities(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _activitiesHandler.HandleAsync(
            new GetRecentActivitiesQuery { Limit = limit },
            cancellationToken);

        return ToActionResult(result, "Recent activities retrieved.");
    }

    /// <summary>
    /// Returns dashboard module readiness (provider health).
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(typeof(ApiResponse<DashboardHealthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetHealth(CancellationToken cancellationToken)
    {
        var result = await _healthHandler.HandleAsync(new GetDashboardHealthQuery(), cancellationToken);
        return ToActionResult(result, "Dashboard health retrieved.");
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

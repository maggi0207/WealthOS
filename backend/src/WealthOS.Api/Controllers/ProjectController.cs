using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Income.Commands;
using WealthOS.Application.Income.DTOs.Requests;
using WealthOS.Application.Income.DTOs.Responses;
using WealthOS.Application.Income.Queries;
using WealthOS.Domain.Income.Enums;

namespace WealthOS.Api.Controllers;

/// <summary>
/// Business project management and developer assignment.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/projects")]
public sealed class ProjectController : ControllerBase
{
    private readonly ICommandHandler<CreateProjectCommand, ProjectResponse> _createHandler;
    private readonly ICommandHandler<AssignDeveloperCommand, ProjectResponse> _assignHandler;
    private readonly IQueryHandler<GetProjectsQuery, ProjectListResponse> _listHandler;

    /// <summary>
    /// Creates a new <see cref="ProjectController"/>.
    /// </summary>
    public ProjectController(
        ICommandHandler<CreateProjectCommand, ProjectResponse> createHandler,
        ICommandHandler<AssignDeveloperCommand, ProjectResponse> assignHandler,
        IQueryHandler<GetProjectsQuery, ProjectListResponse> listHandler)
    {
        _createHandler = createHandler;
        _assignHandler = assignHandler;
        _listHandler = listHandler;
    }

    /// <summary>
    /// Returns a paginated list of projects.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<ProjectListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? clientId = null,
        [FromQuery] ProjectStatus? status = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _listHandler.HandleAsync(
            new GetProjectsQuery
            {
                Page = page,
                PageSize = pageSize,
                ClientId = clientId,
                Status = status,
                Search = search,
            },
            cancellationToken);

        return ToActionResult(result, "Projects retrieved.");
    }

    /// <summary>
    /// Creates a project under a client.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreateProjectCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<ProjectResponse>.Ok(result.Value, "Project created."));
        }

        return IncomeControllerHelpers.ToFailureResult(this, result.Error!);
    }

    /// <summary>
    /// Assigns a developer to a project.
    /// </summary>
    [HttpPost("assign-developer")]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AssignDeveloper(
        [FromBody] AssignDeveloperRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _assignHandler.HandleAsync(
            new AssignDeveloperCommand { Request = request },
            cancellationToken);

        return ToActionResult(result, "Developer assigned.");
    }

    private IActionResult ToActionResult<T>(Result<T> result, string successMessage)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.Ok(result.Value, successMessage));
        }

        return IncomeControllerHelpers.ToFailureResult(this, result.Error!);
    }
}

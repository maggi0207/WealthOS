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
/// Developer roster management.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/developers")]
public sealed class DeveloperController : ControllerBase
{
    private readonly ICommandHandler<CreateDeveloperCommand, DeveloperResponse> _createHandler;
    private readonly IQueryHandler<GetDevelopersQuery, DeveloperListResponse> _listHandler;

    /// <summary>
    /// Creates a new <see cref="DeveloperController"/>.
    /// </summary>
    public DeveloperController(
        ICommandHandler<CreateDeveloperCommand, DeveloperResponse> createHandler,
        IQueryHandler<GetDevelopersQuery, DeveloperListResponse> listHandler)
    {
        _createHandler = createHandler;
        _listHandler = listHandler;
    }

    /// <summary>
    /// Returns a paginated list of developers.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<DeveloperListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _listHandler.HandleAsync(
            new GetDevelopersQuery
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                IsActive = isActive,
            },
            cancellationToken);

        return ToActionResult(result, "Developers retrieved.");
    }

    /// <summary>
    /// Creates a developer.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DeveloperResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateDeveloperRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreateDeveloperCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<DeveloperResponse>.Ok(result.Value, "Developer created."));
        }

        return IncomeControllerHelpers.ToFailureResult(this, result.Error!);
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

/// <summary>
/// Developer payroll records.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/payroll")]
public sealed class PayrollController : ControllerBase
{
    private readonly ICommandHandler<CreatePayrollCommand, PayrollResponse> _createHandler;
    private readonly IQueryHandler<GetPayrollQuery, PayrollListResponse> _listHandler;

    /// <summary>
    /// Creates a new <see cref="PayrollController"/>.
    /// </summary>
    public PayrollController(
        ICommandHandler<CreatePayrollCommand, PayrollResponse> createHandler,
        IQueryHandler<GetPayrollQuery, PayrollListResponse> listHandler)
    {
        _createHandler = createHandler;
        _listHandler = listHandler;
    }

    /// <summary>
    /// Returns a paginated list of payroll records.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PayrollListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? period = null,
        [FromQuery] PayrollStatus? status = null,
        [FromQuery] Guid? developerId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _listHandler.HandleAsync(
            new GetPayrollQuery
            {
                Page = page,
                PageSize = pageSize,
                Period = period,
                Status = status,
                DeveloperId = developerId,
            },
            cancellationToken);

        return ToActionResult(result, "Payroll retrieved.");
    }

    /// <summary>
    /// Creates a payroll record for a developer.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PayrollResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePayrollRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreatePayrollCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<PayrollResponse>.Ok(result.Value, "Payroll recorded."));
        }

        return IncomeControllerHelpers.ToFailureResult(this, result.Error!);
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

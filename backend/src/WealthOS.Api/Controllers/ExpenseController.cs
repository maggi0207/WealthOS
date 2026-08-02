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

namespace WealthOS.Api.Controllers;

/// <summary>
/// Business expense management.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/expenses")]
public sealed class ExpenseController : ControllerBase
{
    private readonly ICommandHandler<CreateExpenseCommand, ExpenseResponse> _createHandler;
    private readonly IQueryHandler<GetExpensesQuery, ExpenseListResponse> _listHandler;

    /// <summary>
    /// Creates a new <see cref="ExpenseController"/>.
    /// </summary>
    public ExpenseController(
        ICommandHandler<CreateExpenseCommand, ExpenseResponse> createHandler,
        IQueryHandler<GetExpensesQuery, ExpenseListResponse> listHandler)
    {
        _createHandler = createHandler;
        _listHandler = listHandler;
    }

    /// <summary>
    /// Returns a paginated list of business expenses.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<ExpenseListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? period = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _listHandler.HandleAsync(
            new GetExpensesQuery
            {
                Page = page,
                PageSize = pageSize,
                CategoryId = categoryId,
                Period = period,
            },
            cancellationToken);

        return ToActionResult(result, "Expenses retrieved.");
    }

    /// <summary>
    /// Creates a business expense (creates category by name when CategoryId is omitted).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ExpenseResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreateExpenseCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<ExpenseResponse>.Ok(result.Value, "Expense created."));
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

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
/// Income dashboard, cash flow, P&amp;L, monthly trend, and salary recording.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/income")]
public sealed class IncomeController : ControllerBase
{
    private readonly IQueryHandler<GetIncomeDashboardQuery, IncomeDashboardResponse> _dashboardHandler;
    private readonly IQueryHandler<GetCashFlowQuery, CashFlowResponse> _cashFlowHandler;
    private readonly IQueryHandler<GetProfitLossQuery, ProfitLossResponse> _profitLossHandler;
    private readonly IQueryHandler<GetMonthlyIncomeQuery, MonthlyIncomeTrendResponse> _monthlyHandler;
    private readonly ICommandHandler<RecordSalaryCommand, SalaryResponse> _recordSalaryHandler;

    /// <summary>
    /// Creates a new <see cref="IncomeController"/>.
    /// </summary>
    public IncomeController(
        IQueryHandler<GetIncomeDashboardQuery, IncomeDashboardResponse> dashboardHandler,
        IQueryHandler<GetCashFlowQuery, CashFlowResponse> cashFlowHandler,
        IQueryHandler<GetProfitLossQuery, ProfitLossResponse> profitLossHandler,
        IQueryHandler<GetMonthlyIncomeQuery, MonthlyIncomeTrendResponse> monthlyHandler,
        ICommandHandler<RecordSalaryCommand, SalaryResponse> recordSalaryHandler)
    {
        _dashboardHandler = dashboardHandler;
        _cashFlowHandler = cashFlowHandler;
        _profitLossHandler = profitLossHandler;
        _monthlyHandler = monthlyHandler;
        _recordSalaryHandler = recordSalaryHandler;
    }

    /// <summary>
    /// Returns income &amp; business dashboard KPIs for a period (default: current UTC month).
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ApiResponse<IncomeDashboardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] string? period = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _dashboardHandler.HandleAsync(
            new GetIncomeDashboardQuery { Period = period },
            cancellationToken);

        return ToActionResult(result, "Income dashboard retrieved.");
    }

    /// <summary>
    /// Returns cash-flow summary for a period.
    /// </summary>
    [HttpGet("cashflow")]
    [ProducesResponseType(typeof(ApiResponse<CashFlowResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCashFlow(
        [FromQuery] string? period = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _cashFlowHandler.HandleAsync(
            new GetCashFlowQuery { Period = period },
            cancellationToken);

        return ToActionResult(result, "Cash flow retrieved.");
    }

    /// <summary>
    /// Returns profit &amp; loss for a period.
    /// </summary>
    [HttpGet("profit-loss")]
    [ProducesResponseType(typeof(ApiResponse<ProfitLossResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfitLoss(
        [FromQuery] string? period = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _profitLossHandler.HandleAsync(
            new GetProfitLossQuery { Period = period },
            cancellationToken);

        return ToActionResult(result, "Profit and loss retrieved.");
    }

    /// <summary>
    /// Returns monthly salary vs business revenue trend points.
    /// </summary>
    [HttpGet("monthly")]
    [ProducesResponseType(typeof(ApiResponse<MonthlyIncomeTrendResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMonthlyIncome(
        [FromQuery] int months = 6,
        CancellationToken cancellationToken = default)
    {
        var result = await _monthlyHandler.HandleAsync(
            new GetMonthlyIncomeQuery { Months = months },
            cancellationToken);

        return ToActionResult(result, "Monthly income trend retrieved.");
    }

    /// <summary>
    /// Records a salary credit (creates or updates the salary definition and adds a payment).
    /// </summary>
    [HttpPost("salary")]
    [ProducesResponseType(typeof(ApiResponse<SalaryResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecordSalary(
        [FromBody] RecordSalaryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _recordSalaryHandler.HandleAsync(
            new RecordSalaryCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<SalaryResponse>.Ok(result.Value, "Salary recorded."));
        }

        return ToFailureResult(result.Error!);
    }

    private IActionResult ToActionResult<T>(Result<T> result, string successMessage)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.Ok(result.Value, successMessage));
        }

        return ToFailureResult(result.Error!);
    }

    private IActionResult ToFailureResult(Error error) =>
        IncomeControllerHelpers.ToFailureResult(this, error);
}

internal static class IncomeControllerHelpers
{
    public static IActionResult ToFailureResult(ControllerBase controller, Error error)
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
            "unauthorized" => controller.Unauthorized(payload),
            "forbidden" => controller.StatusCode(StatusCodes.Status403Forbidden, payload),
            "not_found" => controller.NotFound(payload),
            "conflict" => controller.Conflict(payload),
            "validation_error" => controller.UnprocessableEntity(payload),
            _ => controller.BadRequest(payload),
        };
    }
}

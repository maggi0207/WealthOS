using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Loans.Commands;
using WealthOS.Application.Loans.DTOs.Requests;
using WealthOS.Application.Loans.DTOs.Responses;
using WealthOS.Application.Loans.Queries;
using WealthOS.Domain.Loans.Enums;

namespace WealthOS.Api.Controllers;

/// <summary>
/// Loan management endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/loans")]
public sealed class LoanController : ControllerBase
{
    private readonly ICommandHandler<CreateLoanCommand, LoanResponse> _createHandler;
    private readonly ICommandHandler<UpdateLoanCommand, LoanResponse> _updateHandler;
    private readonly ICommandHandler<DeleteLoanCommand> _deleteHandler;
    private readonly ICommandHandler<RecordPaymentCommand, LoanPaymentResponse> _recordPaymentHandler;
    private readonly IQueryHandler<GetLoanQuery, LoanResponse> _getByIdHandler;
    private readonly IQueryHandler<GetLoansQuery, LoanListResponse> _getAllHandler;
    private readonly IQueryHandler<GetLoanSummaryQuery, LoanSummaryResponse> _summaryHandler;
    private readonly IQueryHandler<GetUpcomingPaymentsQuery, UpcomingPaymentsResponse> _upcomingHandler;
    private readonly IQueryHandler<GetLoanDashboardQuery, LoanDashboardResponse> _dashboardHandler;

    /// <summary>
    /// Creates a new <see cref="LoanController"/>.
    /// </summary>
    public LoanController(
        ICommandHandler<CreateLoanCommand, LoanResponse> createHandler,
        ICommandHandler<UpdateLoanCommand, LoanResponse> updateHandler,
        ICommandHandler<DeleteLoanCommand> deleteHandler,
        ICommandHandler<RecordPaymentCommand, LoanPaymentResponse> recordPaymentHandler,
        IQueryHandler<GetLoanQuery, LoanResponse> getByIdHandler,
        IQueryHandler<GetLoansQuery, LoanListResponse> getAllHandler,
        IQueryHandler<GetLoanSummaryQuery, LoanSummaryResponse> summaryHandler,
        IQueryHandler<GetUpcomingPaymentsQuery, UpcomingPaymentsResponse> upcomingHandler,
        IQueryHandler<GetLoanDashboardQuery, LoanDashboardResponse> dashboardHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _recordPaymentHandler = recordPaymentHandler;
        _getByIdHandler = getByIdHandler;
        _getAllHandler = getAllHandler;
        _summaryHandler = summaryHandler;
        _upcomingHandler = upcomingHandler;
        _dashboardHandler = dashboardHandler;
    }

    /// <summary>
    /// Returns a paginated list of loans for the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<LoanListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] LoanStatus? status = null,
        [FromQuery] LoanType? type = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _getAllHandler.HandleAsync(
            new GetLoansQuery
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                Status = status,
                Type = type,
            },
            cancellationToken);

        return ToActionResult(result, "Loans retrieved.");
    }

    /// <summary>
    /// Returns portfolio-level loan summary totals
    /// (total loan amount, outstanding balance, monthly EMI, upcoming EMI, loan count).
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<LoanSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await _summaryHandler.HandleAsync(new GetLoanSummaryQuery(), cancellationToken);
        return ToActionResult(result, "Loan summary retrieved.");
    }

    /// <summary>
    /// Returns upcoming EMI reminders for the authenticated user.
    /// </summary>
    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(ApiResponse<UpcomingPaymentsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUpcoming(
        [FromQuery] int daysAhead = 45,
        [FromQuery] int take = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _upcomingHandler.HandleAsync(
            new GetUpcomingPaymentsQuery { DaysAhead = daysAhead, Take = take },
            cancellationToken);

        return ToActionResult(result, "Upcoming payments retrieved.");
    }

    /// <summary>
    /// Returns a single loan by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LoanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.HandleAsync(
            new GetLoanQuery { LoanId = id },
            cancellationToken);

        return ToActionResult(result, "Loan retrieved.");
    }

    /// <summary>
    /// Returns a per-loan dashboard snapshot (detail, progress, payment counts, sample prepayment).
    /// Choice: nested under <c>/api/v1/loans/{id}/dashboard</c> (same pattern as properties).
    /// </summary>
    [HttpGet("{id:guid}/dashboard")]
    [ProducesResponseType(typeof(ApiResponse<LoanDashboardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDashboard(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dashboardHandler.HandleAsync(
            new GetLoanDashboardQuery { LoanId = id },
            cancellationToken);

        return ToActionResult(result, "Loan dashboard retrieved.");
    }

    /// <summary>
    /// Creates a new loan for the authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<LoanResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateLoanRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreateLoanCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value.Id, version = "1.0" },
                ApiResponse<LoanResponse>.Ok(result.Value, "Loan created."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Updates an existing loan owned by the authenticated user.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LoanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateLoanRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateHandler.HandleAsync(
            new UpdateLoanCommand { LoanId = id, Request = request },
            cancellationToken);

        return ToActionResult(result, "Loan updated.");
    }

    /// <summary>
    /// Soft-deletes a loan owned by the authenticated user.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _deleteHandler.HandleAsync(
            new DeleteLoanCommand { LoanId = id },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object?>.Ok(null, "Loan deleted."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Records a payment against a loan (optionally reducing outstanding balance).
    /// </summary>
    [HttpPost("{id:guid}/payments")]
    [ProducesResponseType(typeof(ApiResponse<LoanPaymentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecordPayment(
        Guid id,
        [FromBody] RecordLoanPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _recordPaymentHandler.HandleAsync(
            new RecordPaymentCommand { LoanId = id, Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<LoanPaymentResponse>.Ok(result.Value, "Payment recorded."));
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

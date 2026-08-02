using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Reports.Commands;
using WealthOS.Application.Reports.DTOs.Requests;
using WealthOS.Application.Reports.DTOs.Responses;
using WealthOS.Application.Reports.Queries;

namespace WealthOS.Api.Controllers;

/// <summary>
/// Reports &amp; Analytics aggregation endpoints. Aggregates module data via Application interfaces only.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly IQueryHandler<GetNetWorthReportQuery, NetWorthReportResponse> _netWorthHandler;
    private readonly IQueryHandler<GetCashFlowReportQuery, CashFlowReportResponse> _cashFlowHandler;
    private readonly IQueryHandler<GetInvestmentReportQuery, InvestmentReportResponse> _investmentHandler;
    private readonly IQueryHandler<GetLoanReportQuery, LoanReportResponse> _loanHandler;
    private readonly IQueryHandler<GetPropertyReportQuery, PropertyReportResponse> _propertyHandler;
    private readonly IQueryHandler<GetBusinessReportQuery, BusinessReportResponse> _businessHandler;
    private readonly IQueryHandler<GetGoalReportQuery, GoalReportResponse> _goalHandler;
    private readonly IQueryHandler<GetDocumentReportQuery, DocumentReportResponse> _documentHandler;
    private readonly IQueryHandler<GetFinancialHealthQuery, FinancialHealthResponse> _healthHandler;
    private readonly IQueryHandler<GetAnalyticsSummaryQuery, AnalyticsSummaryResponse> _summaryHandler;
    private readonly ICommandHandler<GenerateSnapshotCommand, ReportSnapshotResponse> _snapshotHandler;
    private readonly ICommandHandler<ExportReportCommand, ReportExportResponse> _exportHandler;

    /// <summary>
    /// Creates a new <see cref="ReportsController"/>.
    /// </summary>
    public ReportsController(
        IQueryHandler<GetNetWorthReportQuery, NetWorthReportResponse> netWorthHandler,
        IQueryHandler<GetCashFlowReportQuery, CashFlowReportResponse> cashFlowHandler,
        IQueryHandler<GetInvestmentReportQuery, InvestmentReportResponse> investmentHandler,
        IQueryHandler<GetLoanReportQuery, LoanReportResponse> loanHandler,
        IQueryHandler<GetPropertyReportQuery, PropertyReportResponse> propertyHandler,
        IQueryHandler<GetBusinessReportQuery, BusinessReportResponse> businessHandler,
        IQueryHandler<GetGoalReportQuery, GoalReportResponse> goalHandler,
        IQueryHandler<GetDocumentReportQuery, DocumentReportResponse> documentHandler,
        IQueryHandler<GetFinancialHealthQuery, FinancialHealthResponse> healthHandler,
        IQueryHandler<GetAnalyticsSummaryQuery, AnalyticsSummaryResponse> summaryHandler,
        ICommandHandler<GenerateSnapshotCommand, ReportSnapshotResponse> snapshotHandler,
        ICommandHandler<ExportReportCommand, ReportExportResponse> exportHandler)
    {
        _netWorthHandler = netWorthHandler;
        _cashFlowHandler = cashFlowHandler;
        _investmentHandler = investmentHandler;
        _loanHandler = loanHandler;
        _propertyHandler = propertyHandler;
        _businessHandler = businessHandler;
        _goalHandler = goalHandler;
        _documentHandler = documentHandler;
        _healthHandler = healthHandler;
        _summaryHandler = summaryHandler;
        _snapshotHandler = snapshotHandler;
        _exportHandler = exportHandler;
    }

    /// <summary>Returns the net worth report.</summary>
    [HttpGet("networth")]
    [ProducesResponseType(typeof(ApiResponse<NetWorthReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNetWorth(
        [FromQuery] ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var result = await _netWorthHandler.HandleAsync(
            new GetNetWorthReportQuery { Filters = filters },
            cancellationToken);

        return ToActionResult(result, "Net worth report retrieved.");
    }

    /// <summary>Returns the cash flow report.</summary>
    [HttpGet("cashflow")]
    [ProducesResponseType(typeof(ApiResponse<CashFlowReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCashFlow(
        [FromQuery] ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var result = await _cashFlowHandler.HandleAsync(
            new GetCashFlowReportQuery { Filters = filters },
            cancellationToken);

        return ToActionResult(result, "Cash flow report retrieved.");
    }

    /// <summary>Returns the investment performance / allocation report.</summary>
    [HttpGet("investments")]
    [ProducesResponseType(typeof(ApiResponse<InvestmentReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetInvestments(
        [FromQuery] ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var result = await _investmentHandler.HandleAsync(
            new GetInvestmentReportQuery { Filters = filters },
            cancellationToken);

        return ToActionResult(result, "Investment report retrieved.");
    }

    /// <summary>Returns the loan analysis report.</summary>
    [HttpGet("loans")]
    [ProducesResponseType(typeof(ApiResponse<LoanReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLoans(
        [FromQuery] ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var result = await _loanHandler.HandleAsync(
            new GetLoanReportQuery { Filters = filters },
            cancellationToken);

        return ToActionResult(result, "Loan report retrieved.");
    }

    /// <summary>Returns the property appreciation report.</summary>
    [HttpGet("properties")]
    [ProducesResponseType(typeof(ApiResponse<PropertyReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProperties(
        [FromQuery] ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var result = await _propertyHandler.HandleAsync(
            new GetPropertyReportQuery { Filters = filters },
            cancellationToken);

        return ToActionResult(result, "Property report retrieved.");
    }

    /// <summary>Returns the business P&amp;L report.</summary>
    [HttpGet("business")]
    [ProducesResponseType(typeof(ApiResponse<BusinessReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetBusiness(
        [FromQuery] ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var result = await _businessHandler.HandleAsync(
            new GetBusinessReportQuery { Filters = filters },
            cancellationToken);

        return ToActionResult(result, "Business report retrieved.");
    }

    /// <summary>Returns the goal progress report.</summary>
    [HttpGet("goals")]
    [ProducesResponseType(typeof(ApiResponse<GoalReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetGoals(
        [FromQuery] ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var result = await _goalHandler.HandleAsync(
            new GetGoalReportQuery { Filters = filters },
            cancellationToken);

        return ToActionResult(result, "Goal report retrieved.");
    }

    /// <summary>Returns the document summary report.</summary>
    [HttpGet("documents")]
    [ProducesResponseType(typeof(ApiResponse<DocumentReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDocuments(
        [FromQuery] ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var result = await _documentHandler.HandleAsync(
            new GetDocumentReportQuery { Filters = filters },
            cancellationToken);

        return ToActionResult(result, "Document report retrieved.");
    }

    /// <summary>Returns the composite financial health score.</summary>
    [HttpGet("financial-health")]
    [ProducesResponseType(typeof(ApiResponse<FinancialHealthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFinancialHealth(
        [FromQuery] ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var result = await _healthHandler.HandleAsync(
            new GetFinancialHealthQuery { Filters = filters },
            cancellationToken);

        return ToActionResult(result, "Financial health report retrieved.");
    }

    /// <summary>Returns the cross-module analytics summary.</summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<AnalyticsSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSummary(
        [FromQuery] ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var result = await _summaryHandler.HandleAsync(
            new GetAnalyticsSummaryQuery { Filters = filters },
            cancellationToken);

        return ToActionResult(result, "Analytics summary retrieved.");
    }

    /// <summary>
    /// Captures a point-in-time report snapshot (JSON payload persisted as metadata).
    /// </summary>
    [HttpPost("snapshots")]
    [ProducesResponseType(typeof(ApiResponse<ReportSnapshotResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GenerateSnapshot(
        [FromBody] GenerateSnapshotRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _snapshotHandler.HandleAsync(
            new GenerateSnapshotCommand { Request = request },
            cancellationToken);

        return ToActionResult(result, "Report snapshot captured.");
    }

    /// <summary>
    /// Export architecture placeholder. Returns NotImplemented metadata — no CSV/Excel/PDF generation.
    /// </summary>
    [HttpPost("exports")]
    [ProducesResponseType(typeof(ApiResponse<ReportExportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Export(
        [FromBody] ExportReportRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _exportHandler.HandleAsync(
            new ExportReportCommand { Request = request },
            cancellationToken);

        return ToActionResult(result, "Export request recorded (not implemented).");
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

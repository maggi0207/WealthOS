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
/// Invoice management.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/invoices")]
public sealed class InvoiceController : ControllerBase
{
    private readonly ICommandHandler<CreateInvoiceCommand, InvoiceResponse> _createHandler;
    private readonly IQueryHandler<GetInvoicesQuery, InvoiceListResponse> _listHandler;

    /// <summary>
    /// Creates a new <see cref="InvoiceController"/>.
    /// </summary>
    public InvoiceController(
        ICommandHandler<CreateInvoiceCommand, InvoiceResponse> createHandler,
        IQueryHandler<GetInvoicesQuery, InvoiceListResponse> listHandler)
    {
        _createHandler = createHandler;
        _listHandler = listHandler;
    }

    /// <summary>
    /// Returns a paginated list of invoices.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<InvoiceListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? clientId = null,
        [FromQuery] InvoiceStatus? status = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _listHandler.HandleAsync(
            new GetInvoicesQuery
            {
                Page = page,
                PageSize = pageSize,
                ClientId = clientId,
                Status = status,
                Search = search,
            },
            cancellationToken);

        return ToActionResult(result, "Invoices retrieved.");
    }

    /// <summary>
    /// Creates an invoice with line items.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<InvoiceResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreateInvoiceCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<InvoiceResponse>.Ok(result.Value, "Invoice created."));
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
/// Invoice payment recording (flat resource).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/payments")]
public sealed class PaymentController : ControllerBase
{
    private readonly ICommandHandler<RecordInvoicePaymentCommand, InvoicePaymentResponse> _recordHandler;

    /// <summary>
    /// Creates a new <see cref="PaymentController"/>.
    /// </summary>
    public PaymentController(
        ICommandHandler<RecordInvoicePaymentCommand, InvoicePaymentResponse> recordHandler)
    {
        _recordHandler = recordHandler;
    }

    /// <summary>
    /// Records a payment against an invoice.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<InvoicePaymentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Record(
        [FromBody] RecordInvoicePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _recordHandler.HandleAsync(
            new RecordInvoicePaymentCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<InvoicePaymentResponse>.Ok(result.Value, "Payment recorded."));
        }

        return IncomeControllerHelpers.ToFailureResult(this, result.Error!);
    }
}

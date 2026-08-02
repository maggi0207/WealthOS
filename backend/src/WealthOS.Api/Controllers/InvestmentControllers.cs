using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Investments.Commands;
using WealthOS.Application.Investments.DTOs.Requests;
using WealthOS.Application.Investments.DTOs.Responses;
using WealthOS.Application.Investments.Queries;
using WealthOS.Domain.Investments.Enums;

namespace WealthOS.Api.Controllers;

/// <summary>
/// Investment accounts, dashboard summary, performance, and allocation.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/investments")]
public sealed class InvestmentController : ControllerBase
{
    private readonly ICommandHandler<CreateInvestmentAccountCommand, InvestmentAccountResponse> _createAccountHandler;
    private readonly ICommandHandler<UpdateInvestmentAccountCommand, InvestmentAccountResponse> _updateAccountHandler;
    private readonly ICommandHandler<DeleteInvestmentAccountCommand> _deleteAccountHandler;
    private readonly IQueryHandler<GetAccountsQuery, InvestmentAccountListResponse> _accountsHandler;
    private readonly IQueryHandler<GetAccountByIdQuery, InvestmentAccountResponse> _accountByIdHandler;
    private readonly IQueryHandler<GetInvestmentDashboardSummaryQuery, InvestmentDashboardResponse> _dashboardHandler;
    private readonly IQueryHandler<GetPerformanceQuery, InvestmentPerformanceResponse> _performanceHandler;
    private readonly IQueryHandler<GetAllocationQuery, AssetAllocationResponse> _allocationHandler;

    /// <summary>
    /// Creates a new <see cref="InvestmentController"/>.
    /// </summary>
    public InvestmentController(
        ICommandHandler<CreateInvestmentAccountCommand, InvestmentAccountResponse> createAccountHandler,
        ICommandHandler<UpdateInvestmentAccountCommand, InvestmentAccountResponse> updateAccountHandler,
        ICommandHandler<DeleteInvestmentAccountCommand> deleteAccountHandler,
        IQueryHandler<GetAccountsQuery, InvestmentAccountListResponse> accountsHandler,
        IQueryHandler<GetAccountByIdQuery, InvestmentAccountResponse> accountByIdHandler,
        IQueryHandler<GetInvestmentDashboardSummaryQuery, InvestmentDashboardResponse> dashboardHandler,
        IQueryHandler<GetPerformanceQuery, InvestmentPerformanceResponse> performanceHandler,
        IQueryHandler<GetAllocationQuery, AssetAllocationResponse> allocationHandler)
    {
        _createAccountHandler = createAccountHandler;
        _updateAccountHandler = updateAccountHandler;
        _deleteAccountHandler = deleteAccountHandler;
        _accountsHandler = accountsHandler;
        _accountByIdHandler = accountByIdHandler;
        _dashboardHandler = dashboardHandler;
        _performanceHandler = performanceHandler;
        _allocationHandler = allocationHandler;
    }

    /// <summary>
    /// Returns paginated investment accounts for the current user.
    /// </summary>
    [HttpGet("accounts")]
    [ProducesResponseType(typeof(ApiResponse<InvestmentAccountListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAccounts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] InvestmentAccountStatus? status = null,
        [FromQuery] Guid? providerId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _accountsHandler.HandleAsync(
            new GetAccountsQuery
            {
                Page = page,
                PageSize = pageSize,
                Status = status,
                ProviderId = providerId,
            },
            cancellationToken);

        return ToActionResult(result, "Investment accounts retrieved.");
    }

    /// <summary>
    /// Creates an investment account.
    /// </summary>
    [HttpPost("accounts")]
    [ProducesResponseType(typeof(ApiResponse<InvestmentAccountResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateAccount(
        [FromBody] CreateInvestmentAccountRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createAccountHandler.HandleAsync(
            new CreateInvestmentAccountCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<InvestmentAccountResponse>.Ok(result.Value, "Investment account created."));
        }

        return InvestmentControllerHelpers.ToFailureResult(this, result.Error!);
    }

    /// <summary>
    /// Returns a single investment account.
    /// </summary>
    [HttpGet("accounts/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<InvestmentAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _accountByIdHandler.HandleAsync(
            new GetAccountByIdQuery { AccountId = id },
            cancellationToken);

        return ToActionResult(result, "Investment account retrieved.");
    }

    /// <summary>
    /// Updates an investment account.
    /// </summary>
    [HttpPut("accounts/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<InvestmentAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateAccount(
        Guid id,
        [FromBody] UpdateInvestmentAccountRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateAccountHandler.HandleAsync(
            new UpdateInvestmentAccountCommand { AccountId = id, Request = request },
            cancellationToken);

        return ToActionResult(result, "Investment account updated.");
    }

    /// <summary>
    /// Soft-deletes an investment account.
    /// </summary>
    [HttpDelete("accounts/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccount(Guid id, CancellationToken cancellationToken)
    {
        var result = await _deleteAccountHandler.HandleAsync(
            new DeleteInvestmentAccountCommand { AccountId = id },
            cancellationToken);

        return InvestmentControllerHelpers.ToEmptyActionResult(this, result, "Investment account deleted.");
    }

    /// <summary>
    /// Returns investments dashboard summary KPIs.
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ApiResponse<InvestmentDashboardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var result = await _dashboardHandler.HandleAsync(
            new GetInvestmentDashboardSummaryQuery(),
            cancellationToken);

        return ToActionResult(result, "Investment dashboard retrieved.");
    }

    /// <summary>
    /// Returns portfolio performance series for a range (XIRR is a placeholder).
    /// </summary>
    [HttpGet("performance")]
    [ProducesResponseType(typeof(ApiResponse<InvestmentPerformanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPerformance(
        [FromQuery] PerformanceRange range = PerformanceRange.OneYear,
        CancellationToken cancellationToken = default)
    {
        var result = await _performanceHandler.HandleAsync(
            new GetPerformanceQuery { Range = range },
            cancellationToken);

        return ToActionResult(result, "Investment performance retrieved.");
    }

    /// <summary>
    /// Returns asset allocation by category.
    /// </summary>
    [HttpGet("allocation")]
    [ProducesResponseType(typeof(ApiResponse<AssetAllocationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllocation(
        [FromQuery] Guid? accountId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _allocationHandler.HandleAsync(
            new GetAllocationQuery { AccountId = accountId },
            cancellationToken);

        return ToActionResult(result, "Asset allocation retrieved.");
    }

    private IActionResult ToActionResult<T>(Result<T> result, string successMessage)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.Ok(result.Value, successMessage));
        }

        return InvestmentControllerHelpers.ToFailureResult(this, result.Error!);
    }
}

/// <summary>
/// Portfolio aggregate views.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/investments")]
public sealed class PortfolioController : ControllerBase
{
    private readonly IQueryHandler<GetPortfolioQuery, PortfolioResponse> _portfolioHandler;
    private readonly IQueryHandler<GetPortfolioSummaryQuery, PortfolioSummaryResponse> _summaryHandler;

    /// <summary>
    /// Creates a new <see cref="PortfolioController"/>.
    /// </summary>
    public PortfolioController(
        IQueryHandler<GetPortfolioQuery, PortfolioResponse> portfolioHandler,
        IQueryHandler<GetPortfolioSummaryQuery, PortfolioSummaryResponse> summaryHandler)
    {
        _portfolioHandler = portfolioHandler;
        _summaryHandler = summaryHandler;
    }

    /// <summary>
    /// Returns aggregated portfolio value, gains, and counts.
    /// </summary>
    [HttpGet("portfolio")]
    [ProducesResponseType(typeof(ApiResponse<PortfolioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPortfolio(
        [FromQuery] Guid? accountId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _portfolioHandler.HandleAsync(
            new GetPortfolioQuery { AccountId = accountId },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<PortfolioResponse>.Ok(result.Value, "Portfolio retrieved."));
        }

        return InvestmentControllerHelpers.ToFailureResult(this, result.Error!);
    }

    /// <summary>
    /// Returns compact portfolio summary KPIs.
    /// </summary>
    [HttpGet("portfolio/summary")]
    [ProducesResponseType(typeof(ApiResponse<PortfolioSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPortfolioSummary(CancellationToken cancellationToken)
    {
        var result = await _summaryHandler.HandleAsync(new GetPortfolioSummaryQuery(), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<PortfolioSummaryResponse>.Ok(result.Value, "Portfolio summary retrieved."));
        }

        return InvestmentControllerHelpers.ToFailureResult(this, result.Error!);
    }
}

/// <summary>
/// Holdings and investment transactions.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/investments")]
public sealed class HoldingController : ControllerBase
{
    private readonly ICommandHandler<AddManualHoldingCommand, HoldingResponse> _addHoldingHandler;
    private readonly ICommandHandler<UpdateHoldingCommand, HoldingResponse> _updateHoldingHandler;
    private readonly ICommandHandler<DeleteHoldingCommand> _deleteHoldingHandler;
    private readonly ICommandHandler<RecordTransactionCommand, InvestmentTransactionResponse> _recordTransactionHandler;
    private readonly IQueryHandler<GetHoldingsQuery, HoldingListResponse> _holdingsHandler;
    private readonly IQueryHandler<GetTransactionsQuery, InvestmentTransactionListResponse> _transactionsHandler;

    /// <summary>
    /// Creates a new <see cref="HoldingController"/>.
    /// </summary>
    public HoldingController(
        ICommandHandler<AddManualHoldingCommand, HoldingResponse> addHoldingHandler,
        ICommandHandler<UpdateHoldingCommand, HoldingResponse> updateHoldingHandler,
        ICommandHandler<DeleteHoldingCommand> deleteHoldingHandler,
        ICommandHandler<RecordTransactionCommand, InvestmentTransactionResponse> recordTransactionHandler,
        IQueryHandler<GetHoldingsQuery, HoldingListResponse> holdingsHandler,
        IQueryHandler<GetTransactionsQuery, InvestmentTransactionListResponse> transactionsHandler)
    {
        _addHoldingHandler = addHoldingHandler;
        _updateHoldingHandler = updateHoldingHandler;
        _deleteHoldingHandler = deleteHoldingHandler;
        _recordTransactionHandler = recordTransactionHandler;
        _holdingsHandler = holdingsHandler;
        _transactionsHandler = transactionsHandler;
    }

    /// <summary>
    /// Returns paginated holdings.
    /// </summary>
    [HttpGet("holdings")]
    [ProducesResponseType(typeof(ApiResponse<HoldingListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetHoldings(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] Guid? accountId = null,
        [FromQuery] InvestmentCategory? category = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _holdingsHandler.HandleAsync(
            new GetHoldingsQuery
            {
                Page = page,
                PageSize = pageSize,
                AccountId = accountId,
                Category = category,
                Search = search,
            },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<HoldingListResponse>.Ok(result.Value, "Holdings retrieved."));
        }

        return InvestmentControllerHelpers.ToFailureResult(this, result.Error!);
    }

    /// <summary>
    /// Adds a manually tracked holding.
    /// </summary>
    [HttpPost("manual-holding")]
    [ProducesResponseType(typeof(ApiResponse<HoldingResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AddManualHolding(
        [FromBody] AddManualHoldingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _addHoldingHandler.HandleAsync(
            new AddManualHoldingCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<HoldingResponse>.Ok(result.Value, "Manual holding added."));
        }

        return InvestmentControllerHelpers.ToFailureResult(this, result.Error!);
    }

    /// <summary>
    /// Updates a holding.
    /// </summary>
    [HttpPut("holdings/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<HoldingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateHolding(
        Guid id,
        [FromBody] UpdateHoldingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateHoldingHandler.HandleAsync(
            new UpdateHoldingCommand { HoldingId = id, Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<HoldingResponse>.Ok(result.Value, "Holding updated."));
        }

        return InvestmentControllerHelpers.ToFailureResult(this, result.Error!);
    }

    /// <summary>
    /// Soft-deletes a holding.
    /// </summary>
    [HttpDelete("holdings/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHolding(Guid id, CancellationToken cancellationToken)
    {
        var result = await _deleteHoldingHandler.HandleAsync(
            new DeleteHoldingCommand { HoldingId = id },
            cancellationToken);

        return InvestmentControllerHelpers.ToEmptyActionResult(this, result, "Holding deleted.");
    }

    /// <summary>
    /// Returns paginated investment transactions.
    /// </summary>
    [HttpGet("transactions")]
    [ProducesResponseType(typeof(ApiResponse<InvestmentTransactionListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] Guid? accountId = null,
        [FromQuery] Guid? holdingId = null,
        [FromQuery] InvestmentTransactionType? type = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _transactionsHandler.HandleAsync(
            new GetTransactionsQuery
            {
                Page = page,
                PageSize = pageSize,
                AccountId = accountId,
                HoldingId = holdingId,
                Type = type,
            },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<InvestmentTransactionListResponse>.Ok(result.Value, "Transactions retrieved."));
        }

        return InvestmentControllerHelpers.ToFailureResult(this, result.Error!);
    }

    /// <summary>
    /// Records an investment transaction.
    /// </summary>
    [HttpPost("transactions")]
    [ProducesResponseType(typeof(ApiResponse<InvestmentTransactionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecordTransaction(
        [FromBody] RecordTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _recordTransactionHandler.HandleAsync(
            new RecordTransactionCommand { Request = request },
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<InvestmentTransactionResponse>.Ok(result.Value, "Transaction recorded."));
        }

        return InvestmentControllerHelpers.ToFailureResult(this, result.Error!);
    }
}

/// <summary>
/// Investment providers and sync stubs.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/investments/providers")]
public sealed class ProviderController : ControllerBase
{
    private readonly IQueryHandler<GetProvidersQuery, InvestmentProviderListResponse> _providersHandler;
    private readonly ICommandHandler<ConnectProviderCommand> _connectHandler;
    private readonly ICommandHandler<SyncProviderCommand> _syncHandler;
    private readonly ICommandHandler<DisconnectProviderCommand> _disconnectHandler;

    /// <summary>
    /// Creates a new <see cref="ProviderController"/>.
    /// </summary>
    public ProviderController(
        IQueryHandler<GetProvidersQuery, InvestmentProviderListResponse> providersHandler,
        ICommandHandler<ConnectProviderCommand> connectHandler,
        ICommandHandler<SyncProviderCommand> syncHandler,
        ICommandHandler<DisconnectProviderCommand> disconnectHandler)
    {
        _providersHandler = providersHandler;
        _connectHandler = connectHandler;
        _syncHandler = syncHandler;
        _disconnectHandler = disconnectHandler;
    }

    /// <summary>
    /// Lists catalog providers (Manual, Angel One, IndiaBonds, future stubs).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<InvestmentProviderListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProviders(CancellationToken cancellationToken)
    {
        var result = await _providersHandler.HandleAsync(new GetProvidersQuery(), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<InvestmentProviderListResponse>.Ok(result.Value, "Providers retrieved."));
        }

        return InvestmentControllerHelpers.ToFailureResult(this, result.Error!);
    }

    /// <summary>
    /// Connects a provider for an account (manual succeeds; broker adapters are placeholders).
    /// </summary>
    [HttpPost("{accountId:guid}/connect")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Connect(Guid accountId, CancellationToken cancellationToken)
    {
        var result = await _connectHandler.HandleAsync(
            new ConnectProviderCommand { AccountId = accountId },
            cancellationToken);

        return InvestmentControllerHelpers.ToEmptyActionResult(this, result, "Provider connect completed.");
    }

    /// <summary>
    /// Syncs portfolio / holdings / transactions for an account (no external API calls).
    /// </summary>
    [HttpPost("{accountId:guid}/sync")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Sync(
        Guid accountId,
        [FromQuery] string target = "portfolio",
        CancellationToken cancellationToken = default)
    {
        var result = await _syncHandler.HandleAsync(
            new SyncProviderCommand { AccountId = accountId, SyncTarget = target },
            cancellationToken);

        return InvestmentControllerHelpers.ToEmptyActionResult(this, result, "Provider sync completed.");
    }

    /// <summary>
    /// Disconnects a provider from an account.
    /// </summary>
    [HttpPost("{accountId:guid}/disconnect")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Disconnect(Guid accountId, CancellationToken cancellationToken)
    {
        var result = await _disconnectHandler.HandleAsync(
            new DisconnectProviderCommand { AccountId = accountId },
            cancellationToken);

        return InvestmentControllerHelpers.ToEmptyActionResult(this, result, "Provider disconnected.");
    }
}

internal static class InvestmentControllerHelpers
{
    public static IActionResult ToEmptyActionResult(ControllerBase controller, Result result, string successMessage)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(ApiResponse<object?>.Ok(null, successMessage));
        }

        return ToFailureResult(controller, result.Error!);
    }

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
            "provider_not_implemented" => controller.BadRequest(payload),
            "provider_unavailable" => controller.BadRequest(payload),
            _ => controller.BadRequest(payload),
        };
    }
}

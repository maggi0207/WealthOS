using WealthOS.Application.Common.Models;
using WealthOS.Application.Investments.DTOs.Requests;
using WealthOS.Application.Investments.DTOs.Responses;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Domain.Investments.Models;

namespace WealthOS.Application.Investments.Interfaces;

/// <summary>
/// Investment account and holding CRUD / listing.
/// </summary>
public interface IInvestmentService
{
    Task<Result<InvestmentAccountResponse>> CreateAccountAsync(
        CreateInvestmentAccountRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<InvestmentAccountResponse>> UpdateAccountAsync(
        Guid accountId,
        UpdateInvestmentAccountRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAccountAsync(Guid accountId, CancellationToken cancellationToken = default);

    Task<Result<InvestmentAccountListResponse>> GetAccountsAsync(
        int page,
        int pageSize,
        InvestmentAccountStatus? status,
        Guid? providerId,
        CancellationToken cancellationToken = default);

    Task<Result<InvestmentAccountResponse>> GetAccountByIdAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    Task<Result<HoldingResponse>> AddManualHoldingAsync(
        AddManualHoldingRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<HoldingResponse>> UpdateHoldingAsync(
        Guid holdingId,
        UpdateHoldingRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteHoldingAsync(Guid holdingId, CancellationToken cancellationToken = default);

    Task<Result<HoldingListResponse>> GetHoldingsAsync(
        int page,
        int pageSize,
        Guid? accountId,
        InvestmentCategory? category,
        string? search,
        CancellationToken cancellationToken = default);

    Task<Result<InvestmentTransactionResponse>> RecordTransactionAsync(
        RecordTransactionRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<InvestmentTransactionListResponse>> GetTransactionsAsync(
        int page,
        int pageSize,
        Guid? accountId,
        Guid? holdingId,
        InvestmentTransactionType? type,
        CancellationToken cancellationToken = default);

    Task<Result<InvestmentProviderListResponse>> GetProvidersAsync(
        CancellationToken cancellationToken = default);

    Task<Result<InvestmentDashboardResponse>> GetDashboardSummaryAsync(
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Portfolio aggregation queries.
/// </summary>
public interface IPortfolioService
{
    Task<Result<PortfolioResponse>> GetPortfolioAsync(
        Guid? accountId = null,
        CancellationToken cancellationToken = default);

    Task<Result<PortfolioSummaryResponse>> GetPortfolioSummaryAsync(
        CancellationToken cancellationToken = default);

    Task<Result<InvestmentPerformanceResponse>> GetPerformanceAsync(
        PerformanceRange range = PerformanceRange.OneYear,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Asset allocation calculations.
/// </summary>
public interface IAllocationService
{
    Task<Result<AssetAllocationResponse>> GetAllocationAsync(
        Guid? accountId = null,
        CancellationToken cancellationToken = default);

    AssetAllocation BuildAllocation(IEnumerable<(InvestmentCategory Category, decimal Value)> holdings);
}

/// <summary>
/// Orchestrates provider connect / sync / disconnect without calling external APIs.
/// </summary>
public interface IProviderSyncService
{
    Task<Result> ConnectAsync(Guid accountId, CancellationToken cancellationToken = default);

    Task<Result> SyncPortfolioAsync(Guid accountId, CancellationToken cancellationToken = default);

    Task<Result> SyncHoldingsAsync(Guid accountId, CancellationToken cancellationToken = default);

    Task<Result> SyncTransactionsAsync(Guid accountId, CancellationToken cancellationToken = default);

    Task<Result> DisconnectAsync(Guid accountId, CancellationToken cancellationToken = default);
}

using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Investments.DTOs.Responses;
using WealthOS.Application.Investments.Interfaces;
using WealthOS.Application.Investments.Queries;

namespace WealthOS.Application.Investments.Queries.Handlers;

public sealed class GetPortfolioQueryHandler : IQueryHandler<GetPortfolioQuery, PortfolioResponse>
{
    private readonly IPortfolioService _portfolioService;

    public GetPortfolioQueryHandler(IPortfolioService portfolioService) => _portfolioService = portfolioService;

    public Task<Result<PortfolioResponse>> HandleAsync(
        GetPortfolioQuery query,
        CancellationToken cancellationToken = default) =>
        _portfolioService.GetPortfolioAsync(query.AccountId, cancellationToken);
}

public sealed class GetPortfolioSummaryQueryHandler
    : IQueryHandler<GetPortfolioSummaryQuery, PortfolioSummaryResponse>
{
    private readonly IPortfolioService _portfolioService;

    public GetPortfolioSummaryQueryHandler(IPortfolioService portfolioService) =>
        _portfolioService = portfolioService;

    public Task<Result<PortfolioSummaryResponse>> HandleAsync(
        GetPortfolioSummaryQuery query,
        CancellationToken cancellationToken = default) =>
        _portfolioService.GetPortfolioSummaryAsync(cancellationToken);
}

public sealed class GetHoldingsQueryHandler : IQueryHandler<GetHoldingsQuery, HoldingListResponse>
{
    private readonly IInvestmentService _investmentService;

    public GetHoldingsQueryHandler(IInvestmentService investmentService) =>
        _investmentService = investmentService;

    public Task<Result<HoldingListResponse>> HandleAsync(
        GetHoldingsQuery query,
        CancellationToken cancellationToken = default) =>
        _investmentService.GetHoldingsAsync(
            query.Page,
            query.PageSize,
            query.AccountId,
            query.Category,
            query.Search,
            cancellationToken);
}

public sealed class GetTransactionsQueryHandler
    : IQueryHandler<GetTransactionsQuery, InvestmentTransactionListResponse>
{
    private readonly IInvestmentService _investmentService;

    public GetTransactionsQueryHandler(IInvestmentService investmentService) =>
        _investmentService = investmentService;

    public Task<Result<InvestmentTransactionListResponse>> HandleAsync(
        GetTransactionsQuery query,
        CancellationToken cancellationToken = default) =>
        _investmentService.GetTransactionsAsync(
            query.Page,
            query.PageSize,
            query.AccountId,
            query.HoldingId,
            query.Type,
            cancellationToken);
}

public sealed class GetAllocationQueryHandler : IQueryHandler<GetAllocationQuery, AssetAllocationResponse>
{
    private readonly IAllocationService _allocationService;

    public GetAllocationQueryHandler(IAllocationService allocationService) =>
        _allocationService = allocationService;

    public Task<Result<AssetAllocationResponse>> HandleAsync(
        GetAllocationQuery query,
        CancellationToken cancellationToken = default) =>
        _allocationService.GetAllocationAsync(query.AccountId, cancellationToken);
}

public sealed class GetPerformanceQueryHandler
    : IQueryHandler<GetPerformanceQuery, InvestmentPerformanceResponse>
{
    private readonly IPortfolioService _portfolioService;

    public GetPerformanceQueryHandler(IPortfolioService portfolioService) =>
        _portfolioService = portfolioService;

    public Task<Result<InvestmentPerformanceResponse>> HandleAsync(
        GetPerformanceQuery query,
        CancellationToken cancellationToken = default) =>
        _portfolioService.GetPerformanceAsync(query.Range, cancellationToken);
}

public sealed class GetInvestmentDashboardSummaryQueryHandler
    : IQueryHandler<GetInvestmentDashboardSummaryQuery, InvestmentDashboardResponse>
{
    private readonly IInvestmentService _investmentService;

    public GetInvestmentDashboardSummaryQueryHandler(IInvestmentService investmentService) =>
        _investmentService = investmentService;

    public Task<Result<InvestmentDashboardResponse>> HandleAsync(
        GetInvestmentDashboardSummaryQuery query,
        CancellationToken cancellationToken = default) =>
        _investmentService.GetDashboardSummaryAsync(cancellationToken);
}

public sealed class GetAccountsQueryHandler : IQueryHandler<GetAccountsQuery, InvestmentAccountListResponse>
{
    private readonly IInvestmentService _investmentService;

    public GetAccountsQueryHandler(IInvestmentService investmentService) =>
        _investmentService = investmentService;

    public Task<Result<InvestmentAccountListResponse>> HandleAsync(
        GetAccountsQuery query,
        CancellationToken cancellationToken = default) =>
        _investmentService.GetAccountsAsync(
            query.Page,
            query.PageSize,
            query.Status,
            query.ProviderId,
            cancellationToken);
}

public sealed class GetAccountByIdQueryHandler : IQueryHandler<GetAccountByIdQuery, InvestmentAccountResponse>
{
    private readonly IInvestmentService _investmentService;

    public GetAccountByIdQueryHandler(IInvestmentService investmentService) =>
        _investmentService = investmentService;

    public Task<Result<InvestmentAccountResponse>> HandleAsync(
        GetAccountByIdQuery query,
        CancellationToken cancellationToken = default) =>
        _investmentService.GetAccountByIdAsync(query.AccountId, cancellationToken);
}

public sealed class GetProvidersQueryHandler : IQueryHandler<GetProvidersQuery, InvestmentProviderListResponse>
{
    private readonly IInvestmentService _investmentService;

    public GetProvidersQueryHandler(IInvestmentService investmentService) =>
        _investmentService = investmentService;

    public Task<Result<InvestmentProviderListResponse>> HandleAsync(
        GetProvidersQuery query,
        CancellationToken cancellationToken = default) =>
        _investmentService.GetProvidersAsync(cancellationToken);
}

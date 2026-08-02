using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Loans.DTOs.Responses;
using WealthOS.Application.Loans.Interfaces;
using WealthOS.Application.Loans.Queries;

namespace WealthOS.Application.Loans.Queries.Handlers;

public sealed class GetLoanQueryHandler
    : IQueryHandler<GetLoanQuery, LoanResponse>
{
    private readonly ILoanService _loanService;

    public GetLoanQueryHandler(ILoanService loanService)
    {
        _loanService = loanService;
    }

    public Task<Result<LoanResponse>> HandleAsync(
        GetLoanQuery query,
        CancellationToken cancellationToken = default) =>
        _loanService.GetByIdAsync(query.LoanId, cancellationToken);
}

public sealed class GetLoansQueryHandler
    : IQueryHandler<GetLoansQuery, LoanListResponse>
{
    private readonly ILoanService _loanService;

    public GetLoansQueryHandler(ILoanService loanService)
    {
        _loanService = loanService;
    }

    public Task<Result<LoanListResponse>> HandleAsync(
        GetLoansQuery query,
        CancellationToken cancellationToken = default) =>
        _loanService.GetAllAsync(
            query.Page,
            query.PageSize,
            query.Search,
            query.Status,
            query.Type,
            cancellationToken);
}

public sealed class GetLoanSummaryQueryHandler
    : IQueryHandler<GetLoanSummaryQuery, LoanSummaryResponse>
{
    private readonly ILoanService _loanService;

    public GetLoanSummaryQueryHandler(ILoanService loanService)
    {
        _loanService = loanService;
    }

    public Task<Result<LoanSummaryResponse>> HandleAsync(
        GetLoanSummaryQuery query,
        CancellationToken cancellationToken = default) =>
        _loanService.GetSummaryAsync(cancellationToken);
}

public sealed class GetUpcomingPaymentsQueryHandler
    : IQueryHandler<GetUpcomingPaymentsQuery, UpcomingPaymentsResponse>
{
    private readonly ILoanService _loanService;

    public GetUpcomingPaymentsQueryHandler(ILoanService loanService)
    {
        _loanService = loanService;
    }

    public Task<Result<UpcomingPaymentsResponse>> HandleAsync(
        GetUpcomingPaymentsQuery query,
        CancellationToken cancellationToken = default) =>
        _loanService.GetUpcomingPaymentsAsync(query.DaysAhead, query.Take, cancellationToken);
}

public sealed class GetLoanDashboardQueryHandler
    : IQueryHandler<GetLoanDashboardQuery, LoanDashboardResponse>
{
    private readonly ILoanService _loanService;

    public GetLoanDashboardQueryHandler(ILoanService loanService)
    {
        _loanService = loanService;
    }

    public Task<Result<LoanDashboardResponse>> HandleAsync(
        GetLoanDashboardQuery query,
        CancellationToken cancellationToken = default) =>
        _loanService.GetDashboardAsync(query.LoanId, cancellationToken);
}

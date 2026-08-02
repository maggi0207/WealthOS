using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Income.DTOs.Responses;
using WealthOS.Application.Income.Interfaces;
using WealthOS.Application.Income.Queries;

namespace WealthOS.Application.Income.Queries.Handlers;

public sealed class GetIncomeDashboardQueryHandler
    : IQueryHandler<GetIncomeDashboardQuery, IncomeDashboardResponse>
{
    private readonly IIncomeService _incomeService;

    public GetIncomeDashboardQueryHandler(IIncomeService incomeService) => _incomeService = incomeService;

    public Task<Result<IncomeDashboardResponse>> HandleAsync(
        GetIncomeDashboardQuery query,
        CancellationToken cancellationToken = default) =>
        _incomeService.GetDashboardAsync(query.Period, cancellationToken);
}

public sealed class GetClientsQueryHandler : IQueryHandler<GetClientsQuery, ClientListResponse>
{
    private readonly IBusinessService _businessService;

    public GetClientsQueryHandler(IBusinessService businessService) => _businessService = businessService;

    public Task<Result<ClientListResponse>> HandleAsync(
        GetClientsQuery query,
        CancellationToken cancellationToken = default) =>
        _businessService.GetClientsAsync(
            query.Page,
            query.PageSize,
            query.Search,
            query.Status,
            cancellationToken);
}

public sealed class GetProjectsQueryHandler : IQueryHandler<GetProjectsQuery, ProjectListResponse>
{
    private readonly IBusinessService _businessService;

    public GetProjectsQueryHandler(IBusinessService businessService) => _businessService = businessService;

    public Task<Result<ProjectListResponse>> HandleAsync(
        GetProjectsQuery query,
        CancellationToken cancellationToken = default) =>
        _businessService.GetProjectsAsync(
            query.Page,
            query.PageSize,
            query.ClientId,
            query.Status,
            query.Search,
            cancellationToken);
}

public sealed class GetInvoicesQueryHandler : IQueryHandler<GetInvoicesQuery, InvoiceListResponse>
{
    private readonly IInvoiceService _invoiceService;

    public GetInvoicesQueryHandler(IInvoiceService invoiceService) => _invoiceService = invoiceService;

    public Task<Result<InvoiceListResponse>> HandleAsync(
        GetInvoicesQuery query,
        CancellationToken cancellationToken = default) =>
        _invoiceService.GetInvoicesAsync(
            query.Page,
            query.PageSize,
            query.ClientId,
            query.Status,
            query.Search,
            cancellationToken);
}

public sealed class GetDevelopersQueryHandler : IQueryHandler<GetDevelopersQuery, DeveloperListResponse>
{
    private readonly IPayrollService _payrollService;

    public GetDevelopersQueryHandler(IPayrollService payrollService) => _payrollService = payrollService;

    public Task<Result<DeveloperListResponse>> HandleAsync(
        GetDevelopersQuery query,
        CancellationToken cancellationToken = default) =>
        _payrollService.GetDevelopersAsync(
            query.Page,
            query.PageSize,
            query.Search,
            query.IsActive,
            cancellationToken);
}

public sealed class GetPayrollQueryHandler : IQueryHandler<GetPayrollQuery, PayrollListResponse>
{
    private readonly IPayrollService _payrollService;

    public GetPayrollQueryHandler(IPayrollService payrollService) => _payrollService = payrollService;

    public Task<Result<PayrollListResponse>> HandleAsync(
        GetPayrollQuery query,
        CancellationToken cancellationToken = default) =>
        _payrollService.GetPayrollAsync(
            query.Page,
            query.PageSize,
            query.Period,
            query.Status,
            query.DeveloperId,
            cancellationToken);
}

public sealed class GetCashFlowQueryHandler : IQueryHandler<GetCashFlowQuery, CashFlowResponse>
{
    private readonly IIncomeService _incomeService;

    public GetCashFlowQueryHandler(IIncomeService incomeService) => _incomeService = incomeService;

    public Task<Result<CashFlowResponse>> HandleAsync(
        GetCashFlowQuery query,
        CancellationToken cancellationToken = default) =>
        _incomeService.GetCashFlowAsync(query.Period, cancellationToken);
}

public sealed class GetProfitLossQueryHandler : IQueryHandler<GetProfitLossQuery, ProfitLossResponse>
{
    private readonly IIncomeService _incomeService;

    public GetProfitLossQueryHandler(IIncomeService incomeService) => _incomeService = incomeService;

    public Task<Result<ProfitLossResponse>> HandleAsync(
        GetProfitLossQuery query,
        CancellationToken cancellationToken = default) =>
        _incomeService.GetProfitLossAsync(query.Period, cancellationToken);
}

public sealed class GetMonthlyIncomeQueryHandler
    : IQueryHandler<GetMonthlyIncomeQuery, MonthlyIncomeTrendResponse>
{
    private readonly IIncomeService _incomeService;

    public GetMonthlyIncomeQueryHandler(IIncomeService incomeService) => _incomeService = incomeService;

    public Task<Result<MonthlyIncomeTrendResponse>> HandleAsync(
        GetMonthlyIncomeQuery query,
        CancellationToken cancellationToken = default) =>
        _incomeService.GetMonthlyIncomeAsync(query.Months, cancellationToken);
}

public sealed class GetExpensesQueryHandler : IQueryHandler<GetExpensesQuery, ExpenseListResponse>
{
    private readonly IBusinessService _businessService;

    public GetExpensesQueryHandler(IBusinessService businessService) => _businessService = businessService;

    public Task<Result<ExpenseListResponse>> HandleAsync(
        GetExpensesQuery query,
        CancellationToken cancellationToken = default) =>
        _businessService.GetExpensesAsync(
            query.Page,
            query.PageSize,
            query.CategoryId,
            query.Period,
            cancellationToken);
}

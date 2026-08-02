using WealthOS.Application.Common.Models;
using WealthOS.Application.Income.DTOs.Requests;
using WealthOS.Application.Income.DTOs.Responses;
using WealthOS.Domain.Income.Enums;

namespace WealthOS.Application.Income.Interfaces;

/// <summary>
/// Income dashboard, cash flow, P&amp;L, and monthly trend queries.
/// </summary>
public interface IIncomeService
{
    Task<Result<IncomeDashboardResponse>> GetDashboardAsync(
        string? period,
        CancellationToken cancellationToken = default);

    Task<Result<CashFlowResponse>> GetCashFlowAsync(
        string? period,
        CancellationToken cancellationToken = default);

    Task<Result<ProfitLossResponse>> GetProfitLossAsync(
        string? period,
        CancellationToken cancellationToken = default);

    Task<Result<MonthlyIncomeTrendResponse>> GetMonthlyIncomeAsync(
        int months = 6,
        CancellationToken cancellationToken = default);

    Task<Result<SalaryResponse>> RecordSalaryAsync(
        RecordSalaryRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Clients, projects, and business expenses.
/// </summary>
public interface IBusinessService
{
    Task<Result<ClientResponse>> CreateClientAsync(
        CreateClientRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<ClientResponse>> UpdateClientAsync(
        Guid clientId,
        UpdateClientRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteClientAsync(Guid clientId, CancellationToken cancellationToken = default);

    Task<Result<ClientListResponse>> GetClientsAsync(
        int page,
        int pageSize,
        string? search,
        ClientStatus? status,
        CancellationToken cancellationToken = default);

    Task<Result<ProjectResponse>> CreateProjectAsync(
        CreateProjectRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<ProjectResponse>> AssignDeveloperAsync(
        AssignDeveloperRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<ProjectListResponse>> GetProjectsAsync(
        int page,
        int pageSize,
        Guid? clientId,
        ProjectStatus? status,
        string? search,
        CancellationToken cancellationToken = default);

    Task<Result<ExpenseResponse>> CreateExpenseAsync(
        CreateExpenseRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<ExpenseListResponse>> GetExpensesAsync(
        int page,
        int pageSize,
        Guid? categoryId,
        string? period,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Developers and payroll.
/// </summary>
public interface IPayrollService
{
    Task<Result<DeveloperResponse>> CreateDeveloperAsync(
        CreateDeveloperRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<DeveloperListResponse>> GetDevelopersAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<Result<PayrollResponse>> CreatePayrollAsync(
        CreatePayrollRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<PayrollListResponse>> GetPayrollAsync(
        int page,
        int pageSize,
        string? period,
        PayrollStatus? status,
        Guid? developerId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Invoices and invoice payments.
/// </summary>
public interface IInvoiceService
{
    Task<Result<InvoiceResponse>> CreateInvoiceAsync(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<InvoicePaymentResponse>> RecordPaymentAsync(
        RecordInvoicePaymentRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<InvoiceListResponse>> GetInvoicesAsync(
        int page,
        int pageSize,
        Guid? clientId,
        InvoiceStatus? status,
        string? search,
        CancellationToken cancellationToken = default);

    Task<Result<InvoiceResponse>> GetInvoiceByIdAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default);
}

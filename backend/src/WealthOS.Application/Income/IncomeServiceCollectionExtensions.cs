using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Income.Calculations;
using WealthOS.Application.Income.Commands;
using WealthOS.Application.Income.Commands.Handlers;
using WealthOS.Application.Income.DTOs.Responses;
using WealthOS.Application.Income.Interfaces;
using WealthOS.Application.Income.Queries;
using WealthOS.Application.Income.Queries.Handlers;
using WealthOS.Application.Income.Services;

namespace WealthOS.Application.Income;

/// <summary>
/// Registers Income &amp; Business application services and CQRS handlers.
/// </summary>
public static class IncomeServiceCollectionExtensions
{
    public static IServiceCollection AddIncomeApplication(this IServiceCollection services)
    {
        services.AddScoped<IIncomeCalculationService, IncomeCalculationService>();
        services.AddScoped<IIncomeService, IncomeService>();
        services.AddScoped<IBusinessService, BusinessService>();
        services.AddScoped<IPayrollService, PayrollService>();
        services.AddScoped<IInvoiceService, InvoiceService>();

        services.AddScoped<ICommandHandler<CreateClientCommand, ClientResponse>, CreateClientCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateClientCommand, ClientResponse>, UpdateClientCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteClientCommand>, DeleteClientCommandHandler>();
        services.AddScoped<ICommandHandler<CreateProjectCommand, ProjectResponse>, CreateProjectCommandHandler>();
        services.AddScoped<ICommandHandler<AssignDeveloperCommand, ProjectResponse>, AssignDeveloperCommandHandler>();
        services.AddScoped<ICommandHandler<CreateInvoiceCommand, InvoiceResponse>, CreateInvoiceCommandHandler>();
        services.AddScoped<
            ICommandHandler<RecordInvoicePaymentCommand, InvoicePaymentResponse>,
            RecordInvoicePaymentCommandHandler>();
        services.AddScoped<ICommandHandler<CreateExpenseCommand, ExpenseResponse>, CreateExpenseCommandHandler>();
        services.AddScoped<ICommandHandler<RecordSalaryCommand, SalaryResponse>, RecordSalaryCommandHandler>();
        services.AddScoped<ICommandHandler<CreateDeveloperCommand, DeveloperResponse>, CreateDeveloperCommandHandler>();
        services.AddScoped<ICommandHandler<CreatePayrollCommand, PayrollResponse>, CreatePayrollCommandHandler>();

        services.AddScoped<
            IQueryHandler<GetIncomeDashboardQuery, IncomeDashboardResponse>,
            GetIncomeDashboardQueryHandler>();
        services.AddScoped<IQueryHandler<GetClientsQuery, ClientListResponse>, GetClientsQueryHandler>();
        services.AddScoped<IQueryHandler<GetProjectsQuery, ProjectListResponse>, GetProjectsQueryHandler>();
        services.AddScoped<IQueryHandler<GetInvoicesQuery, InvoiceListResponse>, GetInvoicesQueryHandler>();
        services.AddScoped<IQueryHandler<GetDevelopersQuery, DeveloperListResponse>, GetDevelopersQueryHandler>();
        services.AddScoped<IQueryHandler<GetPayrollQuery, PayrollListResponse>, GetPayrollQueryHandler>();
        services.AddScoped<IQueryHandler<GetCashFlowQuery, CashFlowResponse>, GetCashFlowQueryHandler>();
        services.AddScoped<IQueryHandler<GetProfitLossQuery, ProfitLossResponse>, GetProfitLossQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetMonthlyIncomeQuery, MonthlyIncomeTrendResponse>,
            GetMonthlyIncomeQueryHandler>();
        services.AddScoped<IQueryHandler<GetExpensesQuery, ExpenseListResponse>, GetExpensesQueryHandler>();

        return services;
    }
}

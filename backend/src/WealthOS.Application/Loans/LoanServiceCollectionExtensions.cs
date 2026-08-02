using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Loans.Calculations;
using WealthOS.Application.Loans.Commands;
using WealthOS.Application.Loans.Commands.Handlers;
using WealthOS.Application.Loans.DTOs.Responses;
using WealthOS.Application.Loans.Interfaces;
using WealthOS.Application.Loans.Queries;
using WealthOS.Application.Loans.Queries.Handlers;
using WealthOS.Application.Loans.Services;

namespace WealthOS.Application.Loans;

/// <summary>
/// Registers Loans application services and CQRS handlers.
/// </summary>
public static class LoanServiceCollectionExtensions
{
    public static IServiceCollection AddLoansApplication(this IServiceCollection services)
    {
        services.AddScoped<ILoanCalculationService, LoanCalculationService>();
        services.AddScoped<ILoanService, LoanService>();

        services.AddScoped<
            ICommandHandler<CreateLoanCommand, LoanResponse>,
            CreateLoanCommandHandler>();

        services.AddScoped<
            ICommandHandler<UpdateLoanCommand, LoanResponse>,
            UpdateLoanCommandHandler>();

        services.AddScoped<
            ICommandHandler<DeleteLoanCommand>,
            DeleteLoanCommandHandler>();

        services.AddScoped<
            ICommandHandler<RecordPaymentCommand, LoanPaymentResponse>,
            RecordPaymentCommandHandler>();

        services.AddScoped<
            IQueryHandler<GetLoanQuery, LoanResponse>,
            GetLoanQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetLoansQuery, LoanListResponse>,
            GetLoansQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetLoanSummaryQuery, LoanSummaryResponse>,
            GetLoanSummaryQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetUpcomingPaymentsQuery, UpcomingPaymentsResponse>,
            GetUpcomingPaymentsQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetLoanDashboardQuery, LoanDashboardResponse>,
            GetLoanDashboardQueryHandler>();

        return services;
    }
}

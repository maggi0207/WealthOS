using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Investments.Calculations;
using WealthOS.Application.Investments.Commands;
using WealthOS.Application.Investments.Commands.Handlers;
using WealthOS.Application.Investments.DTOs.Responses;
using WealthOS.Application.Investments.Interfaces;
using WealthOS.Application.Investments.Queries;
using WealthOS.Application.Investments.Queries.Handlers;
using WealthOS.Application.Investments.Services;

namespace WealthOS.Application.Investments;

/// <summary>
/// Registers Investments application services and CQRS handlers.
/// </summary>
public static class InvestmentServiceCollectionExtensions
{
    public static IServiceCollection AddInvestmentsApplication(this IServiceCollection services)
    {
        services.AddScoped<IInvestmentCalculationService, InvestmentCalculationService>();
        services.AddScoped<IInvestmentService, InvestmentService>();
        services.AddScoped<IPortfolioService, PortfolioService>();
        services.AddScoped<IAllocationService, AllocationService>();
        services.AddScoped<IProviderSyncService, ProviderSyncService>();

        services.AddScoped<
            ICommandHandler<CreateInvestmentAccountCommand, InvestmentAccountResponse>,
            CreateInvestmentAccountCommandHandler>();
        services.AddScoped<
            ICommandHandler<UpdateInvestmentAccountCommand, InvestmentAccountResponse>,
            UpdateInvestmentAccountCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteInvestmentAccountCommand>, DeleteInvestmentAccountCommandHandler>();
        services.AddScoped<ICommandHandler<AddManualHoldingCommand, HoldingResponse>, AddManualHoldingCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateHoldingCommand, HoldingResponse>, UpdateHoldingCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteHoldingCommand>, DeleteHoldingCommandHandler>();
        services.AddScoped<
            ICommandHandler<RecordTransactionCommand, InvestmentTransactionResponse>,
            RecordTransactionCommandHandler>();
        services.AddScoped<ICommandHandler<ConnectProviderCommand>, ConnectProviderCommandHandler>();
        services.AddScoped<ICommandHandler<SyncProviderCommand>, SyncProviderCommandHandler>();
        services.AddScoped<ICommandHandler<DisconnectProviderCommand>, DisconnectProviderCommandHandler>();

        services.AddScoped<IQueryHandler<GetPortfolioQuery, PortfolioResponse>, GetPortfolioQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetPortfolioSummaryQuery, PortfolioSummaryResponse>,
            GetPortfolioSummaryQueryHandler>();
        services.AddScoped<IQueryHandler<GetHoldingsQuery, HoldingListResponse>, GetHoldingsQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetTransactionsQuery, InvestmentTransactionListResponse>,
            GetTransactionsQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllocationQuery, AssetAllocationResponse>, GetAllocationQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetPerformanceQuery, InvestmentPerformanceResponse>,
            GetPerformanceQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetInvestmentDashboardSummaryQuery, InvestmentDashboardResponse>,
            GetInvestmentDashboardSummaryQueryHandler>();
        services.AddScoped<IQueryHandler<GetAccountsQuery, InvestmentAccountListResponse>, GetAccountsQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetAccountByIdQuery, InvestmentAccountResponse>,
            GetAccountByIdQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetProvidersQuery, InvestmentProviderListResponse>,
            GetProvidersQueryHandler>();

        return services;
    }
}

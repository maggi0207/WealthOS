using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Investments.Commands;
using WealthOS.Application.Investments.DTOs.Responses;
using WealthOS.Application.Investments.Interfaces;

namespace WealthOS.Application.Investments.Commands.Handlers;

public sealed class CreateInvestmentAccountCommandHandler
    : ICommandHandler<CreateInvestmentAccountCommand, InvestmentAccountResponse>
{
    private readonly IInvestmentService _service;

    public CreateInvestmentAccountCommandHandler(IInvestmentService service) => _service = service;

    public Task<Result<InvestmentAccountResponse>> HandleAsync(
        CreateInvestmentAccountCommand command,
        CancellationToken cancellationToken = default) =>
        _service.CreateAccountAsync(command.Request, cancellationToken);
}

public sealed class UpdateInvestmentAccountCommandHandler
    : ICommandHandler<UpdateInvestmentAccountCommand, InvestmentAccountResponse>
{
    private readonly IInvestmentService _service;

    public UpdateInvestmentAccountCommandHandler(IInvestmentService service) => _service = service;

    public Task<Result<InvestmentAccountResponse>> HandleAsync(
        UpdateInvestmentAccountCommand command,
        CancellationToken cancellationToken = default) =>
        _service.UpdateAccountAsync(command.AccountId, command.Request, cancellationToken);
}

public sealed class DeleteInvestmentAccountCommandHandler : ICommandHandler<DeleteInvestmentAccountCommand>
{
    private readonly IInvestmentService _service;

    public DeleteInvestmentAccountCommandHandler(IInvestmentService service) => _service = service;

    public Task<Result> HandleAsync(
        DeleteInvestmentAccountCommand command,
        CancellationToken cancellationToken = default) =>
        _service.DeleteAccountAsync(command.AccountId, cancellationToken);
}

public sealed class AddManualHoldingCommandHandler
    : ICommandHandler<AddManualHoldingCommand, HoldingResponse>
{
    private readonly IInvestmentService _service;

    public AddManualHoldingCommandHandler(IInvestmentService service) => _service = service;

    public Task<Result<HoldingResponse>> HandleAsync(
        AddManualHoldingCommand command,
        CancellationToken cancellationToken = default) =>
        _service.AddManualHoldingAsync(command.Request, cancellationToken);
}

public sealed class UpdateHoldingCommandHandler : ICommandHandler<UpdateHoldingCommand, HoldingResponse>
{
    private readonly IInvestmentService _service;

    public UpdateHoldingCommandHandler(IInvestmentService service) => _service = service;

    public Task<Result<HoldingResponse>> HandleAsync(
        UpdateHoldingCommand command,
        CancellationToken cancellationToken = default) =>
        _service.UpdateHoldingAsync(command.HoldingId, command.Request, cancellationToken);
}

public sealed class DeleteHoldingCommandHandler : ICommandHandler<DeleteHoldingCommand>
{
    private readonly IInvestmentService _service;

    public DeleteHoldingCommandHandler(IInvestmentService service) => _service = service;

    public Task<Result> HandleAsync(
        DeleteHoldingCommand command,
        CancellationToken cancellationToken = default) =>
        _service.DeleteHoldingAsync(command.HoldingId, cancellationToken);
}

public sealed class RecordTransactionCommandHandler
    : ICommandHandler<RecordTransactionCommand, InvestmentTransactionResponse>
{
    private readonly IInvestmentService _service;

    public RecordTransactionCommandHandler(IInvestmentService service) => _service = service;

    public Task<Result<InvestmentTransactionResponse>> HandleAsync(
        RecordTransactionCommand command,
        CancellationToken cancellationToken = default) =>
        _service.RecordTransactionAsync(command.Request, cancellationToken);
}

public sealed class ConnectProviderCommandHandler : ICommandHandler<ConnectProviderCommand>
{
    private readonly IProviderSyncService _syncService;

    public ConnectProviderCommandHandler(IProviderSyncService syncService) => _syncService = syncService;

    public Task<Result> HandleAsync(
        ConnectProviderCommand command,
        CancellationToken cancellationToken = default) =>
        _syncService.ConnectAsync(command.AccountId, cancellationToken);
}

public sealed class SyncProviderCommandHandler : ICommandHandler<SyncProviderCommand>
{
    private readonly IProviderSyncService _syncService;

    public SyncProviderCommandHandler(IProviderSyncService syncService) => _syncService = syncService;

    public Task<Result> HandleAsync(
        SyncProviderCommand command,
        CancellationToken cancellationToken = default) =>
        command.SyncTarget.ToLowerInvariant() switch
        {
            "holdings" => _syncService.SyncHoldingsAsync(command.AccountId, cancellationToken),
            "transactions" => _syncService.SyncTransactionsAsync(command.AccountId, cancellationToken),
            _ => _syncService.SyncPortfolioAsync(command.AccountId, cancellationToken),
        };
}

public sealed class DisconnectProviderCommandHandler : ICommandHandler<DisconnectProviderCommand>
{
    private readonly IProviderSyncService _syncService;

    public DisconnectProviderCommandHandler(IProviderSyncService syncService) => _syncService = syncService;

    public Task<Result> HandleAsync(
        DisconnectProviderCommand command,
        CancellationToken cancellationToken = default) =>
        _syncService.DisconnectAsync(command.AccountId, cancellationToken);
}

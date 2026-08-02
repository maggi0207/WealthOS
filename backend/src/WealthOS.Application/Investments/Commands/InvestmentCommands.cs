using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Investments.DTOs.Requests;

namespace WealthOS.Application.Investments.Commands;

public sealed class CreateInvestmentAccountCommand : ICommand
{
    public CreateInvestmentAccountRequest Request { get; init; } = null!;
}

public sealed class UpdateInvestmentAccountCommand : ICommand
{
    public Guid AccountId { get; init; }

    public UpdateInvestmentAccountRequest Request { get; init; } = null!;
}

public sealed class DeleteInvestmentAccountCommand : ICommand
{
    public Guid AccountId { get; init; }
}

public sealed class AddManualHoldingCommand : ICommand
{
    public AddManualHoldingRequest Request { get; init; } = null!;
}

public sealed class UpdateHoldingCommand : ICommand
{
    public Guid HoldingId { get; init; }

    public UpdateHoldingRequest Request { get; init; } = null!;
}

public sealed class DeleteHoldingCommand : ICommand
{
    public Guid HoldingId { get; init; }
}

public sealed class RecordTransactionCommand : ICommand
{
    public RecordTransactionRequest Request { get; init; } = null!;
}

public sealed class ConnectProviderCommand : ICommand
{
    public Guid AccountId { get; init; }
}

public sealed class SyncProviderCommand : ICommand
{
    public Guid AccountId { get; init; }

    /// <summary>portfolio | holdings | transactions</summary>
    public string SyncTarget { get; init; } = "portfolio";
}

public sealed class DisconnectProviderCommand : ICommand
{
    public Guid AccountId { get; init; }
}

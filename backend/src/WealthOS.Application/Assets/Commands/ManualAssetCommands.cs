using WealthOS.Application.Assets.DTOs.Requests;
using WealthOS.Application.Common.Abstractions;

namespace WealthOS.Application.Assets.Commands;

public sealed class CreateManualAssetCommand : ICommand
{
    public CreateManualAssetRequest Request { get; init; } = null!;
}

public sealed class UpdateManualAssetCommand : ICommand
{
    public Guid AssetId { get; init; }

    public UpdateManualAssetRequest Request { get; init; } = null!;
}

public sealed class DeleteManualAssetCommand : ICommand
{
    public Guid AssetId { get; init; }
}

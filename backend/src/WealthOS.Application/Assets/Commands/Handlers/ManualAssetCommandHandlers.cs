using WealthOS.Application.Assets.Commands;
using WealthOS.Application.Assets.DTOs.Responses;
using WealthOS.Application.Assets.Interfaces;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;

namespace WealthOS.Application.Assets.Commands.Handlers;

public sealed class CreateManualAssetCommandHandler
    : ICommandHandler<CreateManualAssetCommand, ManualAssetResponse>
{
    private readonly IManualAssetService _service;

    public CreateManualAssetCommandHandler(IManualAssetService service)
    {
        _service = service;
    }

    public Task<Result<ManualAssetResponse>> HandleAsync(
        CreateManualAssetCommand command,
        CancellationToken cancellationToken = default) =>
        _service.CreateAsync(command.Request, cancellationToken);
}

public sealed class UpdateManualAssetCommandHandler
    : ICommandHandler<UpdateManualAssetCommand, ManualAssetResponse>
{
    private readonly IManualAssetService _service;

    public UpdateManualAssetCommandHandler(IManualAssetService service)
    {
        _service = service;
    }

    public Task<Result<ManualAssetResponse>> HandleAsync(
        UpdateManualAssetCommand command,
        CancellationToken cancellationToken = default) =>
        _service.UpdateAsync(command.AssetId, command.Request, cancellationToken);
}

public sealed class DeleteManualAssetCommandHandler : ICommandHandler<DeleteManualAssetCommand>
{
    private readonly IManualAssetService _service;

    public DeleteManualAssetCommandHandler(IManualAssetService service)
    {
        _service = service;
    }

    public Task<Result> HandleAsync(
        DeleteManualAssetCommand command,
        CancellationToken cancellationToken = default) =>
        _service.DeleteAsync(command.AssetId, cancellationToken);
}

using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Properties.Commands;
using WealthOS.Application.Properties.DTOs.Responses;
using WealthOS.Application.Properties.Interfaces;

namespace WealthOS.Application.Properties.Commands.Handlers;

public sealed class CreatePropertyCommandHandler
    : ICommandHandler<CreatePropertyCommand, PropertyResponse>
{
    private readonly IPropertyService _propertyService;

    public CreatePropertyCommandHandler(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    public Task<Result<PropertyResponse>> HandleAsync(
        CreatePropertyCommand command,
        CancellationToken cancellationToken = default) =>
        _propertyService.CreateAsync(command.Request, cancellationToken);
}

public sealed class UpdatePropertyCommandHandler
    : ICommandHandler<UpdatePropertyCommand, PropertyResponse>
{
    private readonly IPropertyService _propertyService;

    public UpdatePropertyCommandHandler(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    public Task<Result<PropertyResponse>> HandleAsync(
        UpdatePropertyCommand command,
        CancellationToken cancellationToken = default) =>
        _propertyService.UpdateAsync(command.PropertyId, command.Request, cancellationToken);
}

public sealed class DeletePropertyCommandHandler : ICommandHandler<DeletePropertyCommand>
{
    private readonly IPropertyService _propertyService;

    public DeletePropertyCommandHandler(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    public Task<Result> HandleAsync(
        DeletePropertyCommand command,
        CancellationToken cancellationToken = default) =>
        _propertyService.DeleteAsync(command.PropertyId, cancellationToken);
}

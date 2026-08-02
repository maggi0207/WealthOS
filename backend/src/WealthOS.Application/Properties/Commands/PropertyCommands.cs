using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Properties.DTOs.Requests;

namespace WealthOS.Application.Properties.Commands;

/// <summary>
/// Creates a new property for the authenticated user.
/// </summary>
public sealed class CreatePropertyCommand : ICommand
{
    public CreatePropertyRequest Request { get; init; } = null!;
}

/// <summary>
/// Updates an existing property owned by the authenticated user.
/// </summary>
public sealed class UpdatePropertyCommand : ICommand
{
    public Guid PropertyId { get; init; }

    public UpdatePropertyRequest Request { get; init; } = null!;
}

/// <summary>
/// Soft-deletes a property owned by the authenticated user.
/// </summary>
public sealed class DeletePropertyCommand : ICommand
{
    public Guid PropertyId { get; init; }
}

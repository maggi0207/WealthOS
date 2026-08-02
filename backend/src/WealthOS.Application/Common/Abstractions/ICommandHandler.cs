namespace WealthOS.Application.Common.Abstractions;

/// <summary>
/// Marker for application commands (CQRS write side).
/// </summary>
public interface ICommand;

/// <summary>
/// Handles a command and returns a <see cref="Models.Result{TResult}"/>.
/// Lightweight alternative to MediatR — register handlers explicitly in DI.
/// </summary>
public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand
{
    Task<Models.Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handles a command with no success payload.
/// </summary>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Models.Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

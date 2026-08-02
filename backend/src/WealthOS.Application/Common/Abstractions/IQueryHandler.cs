namespace WealthOS.Application.Common.Abstractions;

/// <summary>
/// Marker for application queries (CQRS read side).
/// </summary>
public interface IQuery;

/// <summary>
/// Handles a query and returns a <see cref="Models.Result{TResult}"/>.
/// Lightweight alternative to MediatR — register handlers explicitly in DI.
/// </summary>
/// <typeparam name="TQuery">Query type.</typeparam>
/// <typeparam name="TResult">Success payload type.</typeparam>
public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery
{
    Task<Models.Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}

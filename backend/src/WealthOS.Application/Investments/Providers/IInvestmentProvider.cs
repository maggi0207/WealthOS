using WealthOS.Application.Common.Models;
using WealthOS.Domain.Investments.Enums;

namespace WealthOS.Application.Investments.Providers;

/// <summary>
/// Broker / platform adapter. Phase 7 implementations must not call external APIs.
/// </summary>
public interface IInvestmentProvider
{
    ProviderKind Kind { get; }

    Task<Result> ConnectAsync(Guid accountId, Guid userId, CancellationToken cancellationToken = default);

    Task<Result> SyncPortfolioAsync(Guid accountId, Guid userId, CancellationToken cancellationToken = default);

    Task<Result> SyncHoldingsAsync(Guid accountId, Guid userId, CancellationToken cancellationToken = default);

    Task<Result> SyncTransactionsAsync(Guid accountId, Guid userId, CancellationToken cancellationToken = default);

    Task<Result> DisconnectAsync(Guid accountId, Guid userId, CancellationToken cancellationToken = default);
}

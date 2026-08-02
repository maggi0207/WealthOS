using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Investments.Interfaces;
using WealthOS.Application.Investments.Providers;
using WealthOS.Domain.Investments.Repositories;

namespace WealthOS.Application.Investments.Services;

/// <summary>
/// Resolves the correct <see cref="IInvestmentProvider"/> for an account and delegates sync operations.
/// Does not call external broker APIs.
/// </summary>
public sealed class ProviderSyncService : IProviderSyncService
{
    private readonly IInvestmentAccountRepository _accountRepository;
    private readonly IEnumerable<IInvestmentProvider> _providers;
    private readonly ICurrentUserService _currentUser;

    public ProviderSyncService(
        IInvestmentAccountRepository accountRepository,
        IEnumerable<IInvestmentProvider> providers,
        ICurrentUserService currentUser)
    {
        _accountRepository = accountRepository;
        _providers = providers;
        _currentUser = currentUser;
    }

    public Task<Result> ConnectAsync(Guid accountId, CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, (provider, id, userId, ct) => provider.ConnectAsync(id, userId, ct), cancellationToken);

    public Task<Result> SyncPortfolioAsync(Guid accountId, CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, (provider, id, userId, ct) => provider.SyncPortfolioAsync(id, userId, ct), cancellationToken);

    public Task<Result> SyncHoldingsAsync(Guid accountId, CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, (provider, id, userId, ct) => provider.SyncHoldingsAsync(id, userId, ct), cancellationToken);

    public Task<Result> SyncTransactionsAsync(Guid accountId, CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, (provider, id, userId, ct) => provider.SyncTransactionsAsync(id, userId, ct), cancellationToken);

    public Task<Result> DisconnectAsync(Guid accountId, CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, (provider, id, userId, ct) => provider.DisconnectAsync(id, userId, ct), cancellationToken);

    private async Task<Result> ExecuteAsync(
        Guid accountId,
        Func<IInvestmentProvider, Guid, Guid, CancellationToken, Task<Result>> action,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure(Error.Unauthorized());
        }

        var userId = _currentUser.UserId.Value;
        var account = await _accountRepository.GetByIdWithProviderAsync(accountId, userId, cancellationToken);
        if (account is null || account.Provider is null)
        {
            return Result.Failure(Error.NotFound("InvestmentAccount", accountId));
        }

        var provider = _providers.FirstOrDefault(p => p.Kind == account.Provider.Kind);
        if (provider is null)
        {
            return Result.Failure(Error.Failure(
                "provider_unavailable",
                $"No adapter registered for provider kind '{account.Provider.Kind}'."));
        }

        return await action(provider, accountId, userId, cancellationToken);
    }
}

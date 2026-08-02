using WealthOS.Application.Common.Models;
using WealthOS.Application.Investments.Providers;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Domain.Investments.Repositories;

namespace WealthOS.Infrastructure.Investments.Providers;

/// <summary>
/// Functional manual provider — marks the account as synced without external calls.
/// </summary>
public sealed class ManualInvestmentProvider : IInvestmentProvider
{
    private readonly IInvestmentAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ManualInvestmentProvider(
        IInvestmentAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public ProviderKind Kind => ProviderKind.Manual;

    public async Task<Result> ConnectAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdForUserAsync(accountId, userId, cancellationToken);
        if (account is null)
        {
            return Result.Failure(Error.NotFound("InvestmentAccount", accountId));
        }

        account.Status = InvestmentAccountStatus.Manual;
        account.LastSyncedAt = DateTime.UtcNow;
        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public Task<Result> SyncPortfolioAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        TouchSyncAsync(accountId, userId, cancellationToken);

    public Task<Result> SyncHoldingsAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        TouchSyncAsync(accountId, userId, cancellationToken);

    public Task<Result> SyncTransactionsAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        TouchSyncAsync(accountId, userId, cancellationToken);

    public async Task<Result> DisconnectAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdForUserAsync(accountId, userId, cancellationToken);
        if (account is null)
        {
            return Result.Failure(Error.NotFound("InvestmentAccount", accountId));
        }

        account.Status = InvestmentAccountStatus.Disconnected;
        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> TouchSyncAsync(Guid accountId, Guid userId, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdForUserAsync(accountId, userId, cancellationToken);
        if (account is null)
        {
            return Result.Failure(Error.NotFound("InvestmentAccount", accountId));
        }

        account.LastSyncedAt = DateTime.UtcNow;
        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

/// <summary>
/// IndiaBonds placeholder — no live bond platform API in Phase 7.
/// </summary>
public sealed class IndiaBondsProvider : IInvestmentProvider
{
    public ProviderKind Kind => ProviderKind.IndiaBonds;

    public Task<Result> ConnectAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotImplemented("Connect"));

    public Task<Result> SyncPortfolioAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotImplemented("SyncPortfolio"));

    public Task<Result> SyncHoldingsAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotImplemented("SyncHoldings"));

    public Task<Result> SyncTransactionsAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotImplemented("SyncTransactions"));

    public Task<Result> DisconnectAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotImplemented("Disconnect"));

    private static Result NotImplemented(string operation) =>
        Result.Failure(Error.Failure(
            "provider_not_implemented",
            $"IndiaBonds {operation} is a Phase 7 placeholder. Live integration is deferred."));
}

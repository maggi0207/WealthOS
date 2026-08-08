using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Investments.Providers;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Investments.Entities;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Domain.Investments.Repositories;

namespace WealthOS.Infrastructure.Investments.Providers;

/// <summary>
/// Angel One SmartAPI provider — auth + holdings sync (read-only).
/// </summary>
public sealed class AngelOneProvider : IInvestmentProvider
{
    private readonly IInvestmentAccountRepository _accountRepository;
    private readonly IHoldingRepository _holdingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AngelOneSmartApiClient _client;
    private readonly AngelOneTokenStore _tokenStore;
    private readonly AngelOneOptions _options;
    private readonly ILogger<AngelOneProvider> _logger;

    public AngelOneProvider(
        IInvestmentAccountRepository accountRepository,
        IHoldingRepository holdingRepository,
        IUnitOfWork unitOfWork,
        AngelOneSmartApiClient client,
        AngelOneTokenStore tokenStore,
        IOptions<AngelOneOptions> options,
        ILogger<AngelOneProvider> logger)
    {
        _accountRepository = accountRepository;
        _holdingRepository = holdingRepository;
        _unitOfWork = unitOfWork;
        _client = client;
        _tokenStore = tokenStore;
        _options = options.Value;
        _logger = logger;
    }

    public ProviderKind Kind => ProviderKind.AngelOne;

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

        var auth = await _client.AuthenticateAsync(cancellationToken);
        if (auth.IsFailure)
        {
            return auth;
        }

        account.Status = InvestmentAccountStatus.Connected;
        account.LastSyncedAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(_options.ClientCode))
        {
            account.ExternalAccountReference = _options.ClientCode;
        }

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (_client.IsConfigured)
        {
            var sync = await SyncHoldingsInternalAsync(account, userId, cancellationToken);
            if (sync.IsFailure)
            {
                return sync;
            }
        }

        _logger.LogInformation("Angel One account {AccountId} connected (read-only).", accountId);
        return Result.Success();
    }

    public Task<Result> SyncPortfolioAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        SyncHoldingsAsync(accountId, userId, cancellationToken);

    public async Task<Result> SyncHoldingsAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdForUserAsync(accountId, userId, cancellationToken);
        if (account is null)
        {
            return Result.Failure(Error.NotFound("InvestmentAccount", accountId));
        }

        return await SyncHoldingsInternalAsync(account, userId, cancellationToken);
    }

    public Task<Result> SyncTransactionsAsync(
        Guid accountId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        SyncHoldingsAsync(accountId, userId, cancellationToken);

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

        _tokenStore.Clear();
        account.Status = InvestmentAccountStatus.Disconnected;
        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> SyncHoldingsInternalAsync(
        InvestmentAccount account,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (!_options.EnableLiveSync)
        {
            account.LastSyncedAt = DateTime.UtcNow;
            account.Status = InvestmentAccountStatus.Connected;
            _accountRepository.Update(account);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
                "Angel One holdings sync stubbed for account {AccountId} (EnableLiveSync=false).",
                account.Id);
            return Result.Success();
        }

        var remoteResult = await _client.GetAllHoldingsAsync(cancellationToken);
        if (remoteResult.IsFailure)
        {
            return Result.Failure(remoteResult.Error!);
        }

        var remote = remoteResult.Value;
        var existing = await _holdingRepository.ListTrackedForAccountAsync(account.Id, userId, cancellationToken);
        var synced = existing
            .Where(h => h.Notes is not null && h.Notes.StartsWith("angelone|", StringComparison.Ordinal))
            .ToDictionary(
                h => h.Symbol.Trim().ToUpperInvariant(),
                h => h,
                StringComparer.OrdinalIgnoreCase);

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var now = DateTime.UtcNow;

        foreach (var row in remote)
        {
            seen.Add(row.MatchKey);
            if (synced.TryGetValue(row.MatchKey, out var holding))
            {
                ApplyRemote(holding, row, account.Id, userId);
                _holdingRepository.Update(holding);
                continue;
            }

            var created = new Holding
            {
                UserId = userId,
                AccountId = account.Id,
            };
            ApplyRemote(created, row, account.Id, userId);
            await _holdingRepository.AddAsync(created, cancellationToken);
        }

        foreach (var orphan in synced.Values.Where(h => !seen.Contains(h.Symbol.Trim().ToUpperInvariant())))
        {
            orphan.IsDeleted = true;
            orphan.DeletedAt = now;
            _holdingRepository.Update(orphan);
        }

        account.LastSyncedAt = now;
        account.Status = InvestmentAccountStatus.Connected;
        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Angel One holdings sync completed for account {AccountId}. Upserted={Count}.",
            account.Id,
            remote.Count);
        return Result.Success();
    }

    private static void ApplyRemote(Holding holding, AngelOneHoldingDto row, Guid accountId, Guid userId)
    {
        holding.UserId = userId;
        holding.AccountId = accountId;
        holding.Name = row.DisplayName;
        holding.Symbol = row.MatchKey;
        holding.Category = row.Category;
        holding.InvestmentType = row.InvestmentType;
        holding.Quantity = row.Quantity;
        holding.AverageCost = row.AveragePrice;
        holding.InvestedAmount = row.InvestedAmount;
        holding.CurrentPrice = row.Ltp;
        holding.CurrentValue = row.CurrentValue;
        holding.DayChange = row.DayChange;
        holding.DayChangePercent = row.DayChangePercent;
        holding.CurrencyCode = "INR";
        holding.Notes = row.SyncNotes;
        holding.IsDeleted = false;
        holding.DeletedAt = null;
    }
}

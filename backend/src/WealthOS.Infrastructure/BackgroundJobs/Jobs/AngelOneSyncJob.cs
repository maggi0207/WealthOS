using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WealthOS.Application.Investments.Providers;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Domain.Investments.Repositories;
using WealthOS.Domain.Notifications.Enums;
using WealthOS.Infrastructure.Investments.Providers;

namespace WealthOS.Infrastructure.BackgroundJobs.Jobs;

/// <summary>
/// Hangfire job that syncs connected Angel One accounts (read-only holdings).
/// </summary>
public sealed class AngelOneSyncJob
{
    private readonly AngelOneSmartApiClient _client;
    private readonly IInvestmentAccountRepository _accountRepository;
    private readonly IInvestmentProvider _angelOneProvider;
    private readonly IOptions<AngelOneOptions> _options;
    private readonly BackgroundJobLogWriter _logWriter;
    private readonly ILogger<AngelOneSyncJob> _logger;

    public AngelOneSyncJob(
        AngelOneSmartApiClient client,
        IInvestmentAccountRepository accountRepository,
        IEnumerable<IInvestmentProvider> providers,
        IOptions<AngelOneOptions> options,
        BackgroundJobLogWriter logWriter,
        ILogger<AngelOneSyncJob> logger)
    {
        _client = client;
        _accountRepository = accountRepository;
        _angelOneProvider = providers.First(p => p.Kind == ProviderKind.AngelOne);
        _options = options;
        _logWriter = logWriter;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        if (!_client.IsConfigured)
        {
            await _logWriter.WriteAsync(
                HangfireBackgroundJobService.InvestmentSync,
                BackgroundJobStatus.Succeeded,
                "Angel One sync stub — credentials not configured; no broker API called.",
                cancellationToken: cancellationToken);
            _logger.LogInformation("Angel One Hangfire sync skipped (not configured).");
            return;
        }

        var accounts = await _accountRepository.ListConnectedByProviderKindAsync(
            ProviderKind.AngelOne,
            cancellationToken);

        if (accounts.Count == 0)
        {
            await _logWriter.WriteAsync(
                HangfireBackgroundJobService.InvestmentSync,
                BackgroundJobStatus.Succeeded,
                "Angel One sync — no connected accounts.",
                cancellationToken: cancellationToken);
            return;
        }

        var failures = 0;
        foreach (var account in accounts)
        {
            var result = await _angelOneProvider.SyncHoldingsAsync(account.Id, account.UserId, cancellationToken);
            if (result.IsFailure)
            {
                failures += 1;
                _logger.LogWarning(
                    "Angel One Hangfire sync failed for account {AccountId}: {Error}",
                    account.Id,
                    result.Error?.Message);
            }
        }

        var ok = failures == 0;
        await _logWriter.WriteAsync(
            HangfireBackgroundJobService.InvestmentSync,
            ok ? BackgroundJobStatus.Succeeded : BackgroundJobStatus.Failed,
            ok
                ? $"Angel One holdings sync completed for {accounts.Count} account(s)."
                : $"Angel One holdings sync finished with {failures}/{accounts.Count} failure(s).",
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Angel One Hangfire sync finished. Accounts={Count} Failures={Failures} EnableLiveSync={EnableLiveSync}",
            accounts.Count,
            failures,
            _options.Value.EnableLiveSync);
    }
}

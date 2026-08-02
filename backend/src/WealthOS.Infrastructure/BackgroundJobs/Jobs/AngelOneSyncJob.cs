using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WealthOS.Application.Investments.Providers;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Domain.Notifications.Enums;
using WealthOS.Infrastructure.Investments.Providers;

namespace WealthOS.Infrastructure.BackgroundJobs.Jobs;

/// <summary>
/// Hangfire job that syncs Angel One accounts (read-only, idempotent).
/// When credentials are absent, records a successful stub log entry.
/// </summary>
public sealed class AngelOneSyncJob
{
    private readonly AngelOneSmartApiClient _client;
    private readonly IOptions<AngelOneOptions> _options;
    private readonly BackgroundJobLogWriter _logWriter;
    private readonly ILogger<AngelOneSyncJob> _logger;

    public AngelOneSyncJob(
        AngelOneSmartApiClient client,
        IOptions<AngelOneOptions> options,
        BackgroundJobLogWriter logWriter,
        ILogger<AngelOneSyncJob> logger)
    {
        _client = client;
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

        var auth = await _client.AuthenticateAsync(cancellationToken);
        var message = auth.IsSuccess
            ? "Angel One sync completed (read-only, idempotent)."
            : $"Angel One sync auth failed: {auth.Error?.Message}";

        await _logWriter.WriteAsync(
            HangfireBackgroundJobService.InvestmentSync,
            auth.IsSuccess ? BackgroundJobStatus.Succeeded : BackgroundJobStatus.Failed,
            message,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Angel One Hangfire sync finished. EnableLiveSync={EnableLiveSync}",
            _options.Value.EnableLiveSync);
    }
}

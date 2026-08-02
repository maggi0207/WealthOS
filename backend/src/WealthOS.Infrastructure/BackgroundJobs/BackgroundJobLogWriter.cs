using Microsoft.Extensions.Logging;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Notifications.Entities;
using WealthOS.Domain.Notifications.Enums;
using WealthOS.Domain.Notifications.Repositories;

namespace WealthOS.Infrastructure.BackgroundJobs;

/// <summary>
/// Persists <see cref="BackgroundJobLog"/> rows for Hangfire stub executions.
/// </summary>
public sealed class BackgroundJobLogWriter
{
    private readonly IBackgroundJobLogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BackgroundJobLogWriter> _logger;

    public BackgroundJobLogWriter(
        IBackgroundJobLogRepository logRepository,
        IUnitOfWork unitOfWork,
        ILogger<BackgroundJobLogWriter> logger)
    {
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task WriteAsync(
        string jobName,
        BackgroundJobStatus status,
        string message,
        string? hangfireJobId = null,
        string? errorDetails = null,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var log = new BackgroundJobLog
        {
            JobName = jobName,
            HangfireJobId = hangfireJobId,
            Status = status,
            StartedAt = now,
            CompletedAt = now,
            AttemptCount = 1,
            Message = message,
            ErrorDetails = errorDetails,
        };

        await _logRepository.AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Background job {JobName} finished with status {Status}: {Message}",
            jobName,
            status,
            message);
    }
}

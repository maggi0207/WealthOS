using WealthOS.Infrastructure.BackgroundJobs.Jobs;
using WealthOS.Infrastructure.BackgroundJobs;
using Microsoft.Extensions.Logging;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Notifications.Interfaces;

namespace WealthOS.Infrastructure.BackgroundJobs;

/// <summary>
/// Hangfire-backed background job service. Jobs are stubs that log and write
/// <c>BackgroundJobLog</c> rows — no real module integrations yet.
/// </summary>
public sealed class HangfireBackgroundJobService : IBackgroundJobService
{
    public const string DailyDashboardSummary = "daily-dashboard-summary";
    public const string LoanReminder = "loan-reminder";
    public const string SalaryReminder = "salary-reminder";
    public const string BusinessInvoiceReminder = "business-invoice-reminder";
    public const string DocumentExpiryReminder = "document-expiry-reminder";
    public const string InvestmentSync = "investment-sync";
    public const string GoalProgressCheck = "goal-progress-check";
    public const string WeeklySummary = "weekly-summary";
    public const string MonthlySummary = "monthly-summary";

    private readonly ILogger<HangfireBackgroundJobService> _logger;

    public HangfireBackgroundJobService(ILogger<HangfireBackgroundJobService> logger)
    {
        _logger = logger;
    }

    public Task<Result<string>> EnqueueStubJobAsync(
        string jobName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var jobId = Hangfire.BackgroundJob.Enqueue<NotificationStubJobs>(
            jobs => jobs.RunNamedStubAsync(jobName, CancellationToken.None));

        _logger.LogInformation(
            "Enqueued stub background job {JobName} with id {JobId}",
            jobName,
            jobId);

        return Task.FromResult(Result.Success(jobId));
    }

    public void RegisterRecurringJobs()
    {
        Hangfire.RecurringJob.AddOrUpdate<NotificationStubJobs>(
            DailyDashboardSummary,
            jobs => jobs.RunDailyDashboardSummaryAsync(CancellationToken.None),
            Hangfire.Cron.Daily(6));

        Hangfire.RecurringJob.AddOrUpdate<NotificationStubJobs>(
            LoanReminder,
            jobs => jobs.RunLoanReminderAsync(CancellationToken.None),
            Hangfire.Cron.Daily(7));

        Hangfire.RecurringJob.AddOrUpdate<NotificationStubJobs>(
            SalaryReminder,
            jobs => jobs.RunSalaryReminderAsync(CancellationToken.None),
            Hangfire.Cron.Monthly(1, 8));

        Hangfire.RecurringJob.AddOrUpdate<NotificationStubJobs>(
            BusinessInvoiceReminder,
            jobs => jobs.RunBusinessInvoiceReminderAsync(CancellationToken.None),
            Hangfire.Cron.Daily(9));

        Hangfire.RecurringJob.AddOrUpdate<NotificationStubJobs>(
            DocumentExpiryReminder,
            jobs => jobs.RunDocumentExpiryReminderAsync(CancellationToken.None),
            Hangfire.Cron.Daily(10));

        Hangfire.RecurringJob.AddOrUpdate<AngelOneSyncJob>(
            InvestmentSync,
            jobs => jobs.RunAsync(CancellationToken.None),
            Hangfire.Cron.Hourly());

        Hangfire.RecurringJob.AddOrUpdate<NotificationStubJobs>(
            GoalProgressCheck,
            jobs => jobs.RunGoalProgressCheckAsync(CancellationToken.None),
            Hangfire.Cron.Daily(11));

        Hangfire.RecurringJob.AddOrUpdate<NotificationStubJobs>(
            WeeklySummary,
            jobs => jobs.RunWeeklySummaryAsync(CancellationToken.None),
            Hangfire.Cron.Weekly(DayOfWeek.Monday, 7));

        Hangfire.RecurringJob.AddOrUpdate<NotificationStubJobs>(
            MonthlySummary,
            jobs => jobs.RunMonthlySummaryAsync(CancellationToken.None),
            Hangfire.Cron.Monthly(1, 7));

        _logger.LogInformation("Registered {Count} recurring Hangfire stub jobs", 9);
    }
}

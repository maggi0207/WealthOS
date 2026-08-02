using WealthOS.Domain.Notifications.Enums;
using WealthOS.Infrastructure.BackgroundJobs;

namespace WealthOS.Infrastructure.BackgroundJobs.Jobs;

/// <summary>
/// Hangfire job stubs. Each method logs a <c>BackgroundJobLog</c> entry and returns.
/// Real module integrations (loans, documents, investments, etc.) are future work.
/// </summary>
public sealed class NotificationStubJobs
{
    private readonly BackgroundJobLogWriter _logWriter;

    public NotificationStubJobs(BackgroundJobLogWriter logWriter)
    {
        _logWriter = logWriter;
    }

    public Task RunDailyDashboardSummaryAsync(CancellationToken cancellationToken) =>
        ExecuteStubAsync(
            HangfireBackgroundJobService.DailyDashboardSummary,
            "Daily dashboard summary stub — no aggregation performed.",
            cancellationToken);

    public Task RunLoanReminderAsync(CancellationToken cancellationToken) =>
        ExecuteStubAsync(
            HangfireBackgroundJobService.LoanReminder,
            "Loan / EMI reminder stub — no loan scan performed.",
            cancellationToken);

    public Task RunSalaryReminderAsync(CancellationToken cancellationToken) =>
        ExecuteStubAsync(
            HangfireBackgroundJobService.SalaryReminder,
            "Salary reminder stub — no payroll scan performed.",
            cancellationToken);

    public Task RunBusinessInvoiceReminderAsync(CancellationToken cancellationToken) =>
        ExecuteStubAsync(
            HangfireBackgroundJobService.BusinessInvoiceReminder,
            "Business invoice reminder stub — no invoice scan performed.",
            cancellationToken);

    public Task RunDocumentExpiryReminderAsync(CancellationToken cancellationToken) =>
        ExecuteStubAsync(
            HangfireBackgroundJobService.DocumentExpiryReminder,
            "Document expiry reminder stub — no document scan performed.",
            cancellationToken);

    public Task RunInvestmentSyncAsync(CancellationToken cancellationToken) =>
        ExecuteStubAsync(
            HangfireBackgroundJobService.InvestmentSync,
            "Investment sync stub — no broker API called.",
            cancellationToken);

    public Task RunGoalProgressCheckAsync(CancellationToken cancellationToken) =>
        ExecuteStubAsync(
            HangfireBackgroundJobService.GoalProgressCheck,
            "Goal progress check stub — no goal calculation performed.",
            cancellationToken);

    public Task RunWeeklySummaryAsync(CancellationToken cancellationToken) =>
        ExecuteStubAsync(
            HangfireBackgroundJobService.WeeklySummary,
            "Weekly summary stub — no summary generated.",
            cancellationToken);

    public Task RunMonthlySummaryAsync(CancellationToken cancellationToken) =>
        ExecuteStubAsync(
            HangfireBackgroundJobService.MonthlySummary,
            "Monthly summary stub — no summary generated.",
            cancellationToken);

    /// <summary>
    /// Dispatches a named stub for fire-and-forget enqueue testing.
    /// </summary>
    public Task RunNamedStubAsync(string jobName, CancellationToken cancellationToken) =>
        ExecuteStubAsync(jobName, $"Named stub job '{jobName}' executed.", cancellationToken);

    private Task ExecuteStubAsync(
        string jobName,
        string message,
        CancellationToken cancellationToken) =>
        _logWriter.WriteAsync(
            jobName,
            BackgroundJobStatus.Succeeded,
            message,
            hangfireJobId: null,
            errorDetails: null,
            cancellationToken);
}

using WealthOS.Application.Dashboard.Providers;

namespace WealthOS.Infrastructure.Dashboard.Providers;

/// <summary>
/// Placeholder income/expense totals aligned with frontend dashboard demo data.
/// </summary>
public sealed class MockIncomeSummaryProvider : IIncomeSummaryProvider
{
    public Task<IncomeModuleSummary> GetSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new IncomeModuleSummary
        {
            MonthlyIncome = 24_800m,
            MonthlyExpense = 15_380m,
            CurrencyCode = "USD",
        });
    }
}

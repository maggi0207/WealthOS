namespace WealthOS.Application.Dashboard.Providers;

/// <summary>
/// Cross-module income/expense totals. Implemented by Income module later; mock in Phase 3.
/// </summary>
public interface IIncomeSummaryProvider
{
    Task<IncomeModuleSummary> GetSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Income module summary shape consumed by Dashboard.
/// </summary>
public sealed class IncomeModuleSummary
{
    public decimal MonthlyIncome { get; init; }

    public decimal MonthlyExpense { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

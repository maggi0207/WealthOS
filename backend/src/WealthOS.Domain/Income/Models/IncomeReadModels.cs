namespace WealthOS.Domain.Income.Models;

/// <summary>
/// Computed cash-flow snapshot for a period (not persisted).
/// Derived from salary payments, invoice payments, payroll, and expenses.
/// </summary>
public sealed class CashFlowSummary
{
    public string Period { get; init; } = string.Empty;

    public string PeriodLabel { get; init; } = string.Empty;

    public decimal SalaryIncome { get; init; }

    public decimal BusinessRevenue { get; init; }

    public decimal DeveloperPayroll { get; init; }

    public decimal BusinessExpenses { get; init; }

    /// <summary>
    /// Optional personal household outflow (not tracked in this module yet; reserved for Goals/Budget).
    /// </summary>
    public decimal PersonalOutflow { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

/// <summary>
/// Computed profit &amp; loss for a period (not persisted).
/// </summary>
public sealed class MonthlyProfit
{
    public string Period { get; init; } = string.Empty;

    public decimal BusinessRevenue { get; init; }

    public decimal DeveloperCost { get; init; }

    public decimal BusinessExpenses { get; init; }

    public decimal GrossProfit { get; init; }

    public decimal NetProfit { get; init; }

    public decimal SalaryIncome { get; init; }

    public decimal TotalIncome { get; init; }

    public decimal CashAvailable { get; init; }

    public decimal SavingsRatePercent { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

/// <summary>
/// Aggregated income dashboard KPIs (not persisted).
/// </summary>
public sealed class IncomeDashboardSummary
{
    public string Period { get; init; } = string.Empty;

    public decimal MonthlyIncome { get; init; }

    public decimal BusinessRevenue { get; init; }

    public decimal Salary { get; init; }

    public decimal DeveloperCost { get; init; }

    public decimal BusinessExpenses { get; init; }

    public decimal OutstandingInvoices { get; init; }

    public decimal NetProfit { get; init; }

    public decimal CashAvailable { get; init; }

    public decimal SavingsRatePercent { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

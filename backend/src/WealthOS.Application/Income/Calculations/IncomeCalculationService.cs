using System.Globalization;
using WealthOS.Domain.Income.Models;

namespace WealthOS.Application.Income.Calculations;

/// <summary>
/// Pure calculation helpers for income / business KPIs (no GST/tax).
/// </summary>
public interface IIncomeCalculationService
{
    string CurrentPeriod(DateTime? utcNow = null);

    string FormatPeriodLabel(string period);

    MonthlyProfit BuildMonthlyProfit(
        string period,
        decimal salaryIncome,
        decimal businessRevenue,
        decimal developerCost,
        decimal businessExpenses,
        decimal personalOutflow = 0m,
        string currencyCode = "INR");

    IncomeDashboardSummary BuildDashboard(
        string period,
        decimal salaryIncome,
        decimal businessRevenue,
        decimal developerCost,
        decimal businessExpenses,
        decimal outstandingInvoices,
        decimal personalOutflow = 0m,
        string currencyCode = "INR");

    CashFlowSummary BuildCashFlow(
        string period,
        decimal salaryIncome,
        decimal businessRevenue,
        decimal developerPayroll,
        decimal businessExpenses,
        decimal personalOutflow = 0m,
        string currencyCode = "INR");
}

public sealed class IncomeCalculationService : IIncomeCalculationService
{
    public string CurrentPeriod(DateTime? utcNow = null)
    {
        var now = utcNow ?? DateTime.UtcNow;
        return $"{now.Year:D4}-{now.Month:D2}";
    }

    public string FormatPeriodLabel(string period)
    {
        if (!DateTime.TryParseExact(
                period + "-01",
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
        {
            return period;
        }

        return date.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
    }

    public MonthlyProfit BuildMonthlyProfit(
        string period,
        decimal salaryIncome,
        decimal businessRevenue,
        decimal developerCost,
        decimal businessExpenses,
        decimal personalOutflow = 0m,
        string currencyCode = "INR")
    {
        var grossProfit = businessRevenue - developerCost - businessExpenses;
        var netProfit = grossProfit;
        var totalIncome = salaryIncome + netProfit;
        var cashAvailable = salaryIncome + businessRevenue - developerCost - businessExpenses - personalOutflow;
        var inflows = salaryIncome + businessRevenue;
        var savingsRate = inflows <= 0m
            ? 0m
            : Math.Round((cashAvailable / inflows) * 100m, 2, MidpointRounding.AwayFromZero);

        return new MonthlyProfit
        {
            Period = period,
            BusinessRevenue = businessRevenue,
            DeveloperCost = developerCost,
            BusinessExpenses = businessExpenses,
            GrossProfit = grossProfit,
            NetProfit = netProfit,
            SalaryIncome = salaryIncome,
            TotalIncome = totalIncome,
            CashAvailable = cashAvailable,
            SavingsRatePercent = savingsRate,
            CurrencyCode = currencyCode,
        };
    }

    public IncomeDashboardSummary BuildDashboard(
        string period,
        decimal salaryIncome,
        decimal businessRevenue,
        decimal developerCost,
        decimal businessExpenses,
        decimal outstandingInvoices,
        decimal personalOutflow = 0m,
        string currencyCode = "INR")
    {
        var profit = BuildMonthlyProfit(
            period,
            salaryIncome,
            businessRevenue,
            developerCost,
            businessExpenses,
            personalOutflow,
            currencyCode);

        return new IncomeDashboardSummary
        {
            Period = period,
            MonthlyIncome = salaryIncome + businessRevenue,
            BusinessRevenue = businessRevenue,
            Salary = salaryIncome,
            DeveloperCost = developerCost,
            BusinessExpenses = businessExpenses,
            OutstandingInvoices = outstandingInvoices,
            NetProfit = profit.NetProfit,
            CashAvailable = profit.CashAvailable,
            SavingsRatePercent = profit.SavingsRatePercent,
            CurrencyCode = currencyCode,
        };
    }

    public CashFlowSummary BuildCashFlow(
        string period,
        decimal salaryIncome,
        decimal businessRevenue,
        decimal developerPayroll,
        decimal businessExpenses,
        decimal personalOutflow = 0m,
        string currencyCode = "INR") =>
        new()
        {
            Period = period,
            PeriodLabel = FormatPeriodLabel(period),
            SalaryIncome = salaryIncome,
            BusinessRevenue = businessRevenue,
            DeveloperPayroll = developerPayroll,
            BusinessExpenses = businessExpenses,
            PersonalOutflow = personalOutflow,
            CurrencyCode = currencyCode,
        };
}

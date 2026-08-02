using WealthOS.Application.Dashboard.Providers;
using WealthOS.Domain.Income.Repositories;

namespace WealthOS.Infrastructure.Income.Providers;

/// <summary>
/// Dashboard income/expense totals backed by the Income &amp; Business module.
/// Falls back to the previous calendar month when the current period has no activity
/// (common with demo seed data).
/// </summary>
public sealed class IncomeSummaryProvider : IIncomeSummaryProvider
{
    private readonly ISalaryRepository _salaryRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IDeveloperRepository _developerRepository;
    private readonly IBusinessExpenseRepository _expenseRepository;

    public IncomeSummaryProvider(
        ISalaryRepository salaryRepository,
        IInvoiceRepository invoiceRepository,
        IDeveloperRepository developerRepository,
        IBusinessExpenseRepository expenseRepository)
    {
        _salaryRepository = salaryRepository;
        _invoiceRepository = invoiceRepository;
        _developerRepository = developerRepository;
        _expenseRepository = expenseRepository;
    }

    public async Task<IncomeModuleSummary> GetSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var period = CurrentPeriod();
        var totals = await LoadPeriodTotalsAsync(userId, period, cancellationToken);

        if (totals.Income == 0m && totals.Expense == 0m)
        {
            totals = await LoadPeriodTotalsAsync(userId, PreviousPeriod(period), cancellationToken);
        }

        return new IncomeModuleSummary
        {
            MonthlyIncome = totals.Income,
            MonthlyExpense = totals.Expense,
            CurrencyCode = "INR",
        };
    }

    private async Task<(decimal Income, decimal Expense)> LoadPeriodTotalsAsync(
        Guid userId,
        string period,
        CancellationToken cancellationToken)
    {
        var salary = await _salaryRepository.SumPaymentsForPeriodAsync(userId, period, cancellationToken);
        var revenue = await _invoiceRepository.SumPaymentsForPeriodAsync(userId, period, cancellationToken);
        var payroll = await _developerRepository.SumPayrollForPeriodAsync(userId, period, cancellationToken);
        var expenses = await _expenseRepository.SumForPeriodAsync(userId, period, cancellationToken);

        return (salary + revenue, payroll + expenses);
    }

    private static string CurrentPeriod(DateTime? utcNow = null)
    {
        var now = utcNow ?? DateTime.UtcNow;
        return $"{now.Year:D4}-{now.Month:D2}";
    }

    private static string PreviousPeriod(string period)
    {
        if (!DateTime.TryParseExact(
                $"{period}-01",
                "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var monthStart))
        {
            return period;
        }

        var previous = monthStart.AddMonths(-1);
        return $"{previous.Year:D4}-{previous.Month:D2}";
    }
}

using AutoMapper;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Income.Calculations;
using WealthOS.Application.Income.DTOs.Requests;
using WealthOS.Application.Income.DTOs.Responses;
using WealthOS.Application.Income.Interfaces;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Income.Entities;
using WealthOS.Domain.Income.Enums;
using WealthOS.Domain.Income.Repositories;

namespace WealthOS.Application.Income.Services;

/// <summary>
/// Aggregates salary + business figures into dashboard / cash-flow / P&amp;L views.
/// </summary>
public sealed class IncomeService : IIncomeService
{
    private readonly ISalaryRepository _salaryRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IDeveloperRepository _developerRepository;
    private readonly IBusinessExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IIncomeCalculationService _calculator;
    private readonly IMapper _mapper;

    public IncomeService(
        ISalaryRepository salaryRepository,
        IInvoiceRepository invoiceRepository,
        IDeveloperRepository developerRepository,
        IBusinessExpenseRepository expenseRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IIncomeCalculationService calculator,
        IMapper mapper)
    {
        _salaryRepository = salaryRepository;
        _invoiceRepository = invoiceRepository;
        _developerRepository = developerRepository;
        _expenseRepository = expenseRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _calculator = calculator;
        _mapper = mapper;
    }

    public async Task<Result<IncomeDashboardResponse>> GetDashboardAsync(
        string? period,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<IncomeDashboardResponse>(userResult.Error!);
        }

        var resolvedPeriod = string.IsNullOrWhiteSpace(period)
            ? _calculator.CurrentPeriod()
            : period.Trim();

        var totals = await LoadPeriodTotalsAsync(userResult.Value, resolvedPeriod, cancellationToken);
        var outstanding = await _invoiceRepository.SumOutstandingAsync(userResult.Value, cancellationToken);
        var summary = _calculator.BuildDashboard(
            resolvedPeriod,
            totals.Salary,
            totals.Revenue,
            totals.Payroll,
            totals.Expenses,
            outstanding);

        return Result.Success(new IncomeDashboardResponse
        {
            Period = summary.Period,
            MonthlyIncome = summary.MonthlyIncome,
            BusinessRevenue = summary.BusinessRevenue,
            Salary = summary.Salary,
            DeveloperCost = summary.DeveloperCost,
            BusinessExpenses = summary.BusinessExpenses,
            OutstandingInvoices = summary.OutstandingInvoices,
            NetProfit = summary.NetProfit,
            CashAvailable = summary.CashAvailable,
            SavingsRatePercent = summary.SavingsRatePercent,
            CurrencyCode = summary.CurrencyCode,
        });
    }

    public async Task<Result<CashFlowResponse>> GetCashFlowAsync(
        string? period,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<CashFlowResponse>(userResult.Error!);
        }

        var resolvedPeriod = string.IsNullOrWhiteSpace(period)
            ? _calculator.CurrentPeriod()
            : period.Trim();

        var totals = await LoadPeriodTotalsAsync(userResult.Value, resolvedPeriod, cancellationToken);
        var cashFlow = _calculator.BuildCashFlow(
            resolvedPeriod,
            totals.Salary,
            totals.Revenue,
            totals.Payroll,
            totals.Expenses);

        return Result.Success(new CashFlowResponse
        {
            Period = cashFlow.Period,
            PeriodLabel = cashFlow.PeriodLabel,
            SalaryIncome = cashFlow.SalaryIncome,
            BusinessRevenue = cashFlow.BusinessRevenue,
            DeveloperPayroll = cashFlow.DeveloperPayroll,
            BusinessExpenses = cashFlow.BusinessExpenses,
            PersonalOutflow = cashFlow.PersonalOutflow,
            NetCashFlow = cashFlow.SalaryIncome + cashFlow.BusinessRevenue
                - cashFlow.DeveloperPayroll - cashFlow.BusinessExpenses - cashFlow.PersonalOutflow,
            CurrencyCode = cashFlow.CurrencyCode,
        });
    }

    public async Task<Result<ProfitLossResponse>> GetProfitLossAsync(
        string? period,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<ProfitLossResponse>(userResult.Error!);
        }

        var resolvedPeriod = string.IsNullOrWhiteSpace(period)
            ? _calculator.CurrentPeriod()
            : period.Trim();

        var totals = await LoadPeriodTotalsAsync(userResult.Value, resolvedPeriod, cancellationToken);
        var profit = _calculator.BuildMonthlyProfit(
            resolvedPeriod,
            totals.Salary,
            totals.Revenue,
            totals.Payroll,
            totals.Expenses);

        return Result.Success(new ProfitLossResponse
        {
            Period = profit.Period,
            BusinessRevenue = profit.BusinessRevenue,
            DeveloperCost = profit.DeveloperCost,
            BusinessExpenses = profit.BusinessExpenses,
            GrossProfit = profit.GrossProfit,
            NetProfit = profit.NetProfit,
            SalaryIncome = profit.SalaryIncome,
            TotalIncome = profit.TotalIncome,
            CashAvailable = profit.CashAvailable,
            SavingsRatePercent = profit.SavingsRatePercent,
            CurrencyCode = profit.CurrencyCode,
        });
    }

    public async Task<Result<MonthlyIncomeTrendResponse>> GetMonthlyIncomeAsync(
        int months = 6,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<MonthlyIncomeTrendResponse>(userResult.Error!);
        }

        months = Math.Clamp(months, 1, 36);
        var end = DateTime.UtcNow;
        var start = end.AddMonths(-(months - 1));
        var fromPeriod = $"{start.Year:D4}-{start.Month:D2}";
        var toPeriod = $"{end.Year:D4}-{end.Month:D2}";

        var salaryTotals = await _salaryRepository.GetMonthlySalaryTotalsAsync(
            userResult.Value,
            fromPeriod,
            toPeriod,
            cancellationToken);
        var revenueTotals = await _invoiceRepository.GetMonthlyRevenueTotalsAsync(
            userResult.Value,
            fromPeriod,
            toPeriod,
            cancellationToken);

        var salaryMap = salaryTotals.ToDictionary(x => x.Period, x => x.Amount);
        var revenueMap = revenueTotals.ToDictionary(x => x.Period, x => x.Amount);

        var points = new List<MonthlyIncomePointResponse>();
        for (var i = 0; i < months; i++)
        {
            var cursor = start.AddMonths(i);
            var period = $"{cursor.Year:D4}-{cursor.Month:D2}";
            points.Add(new MonthlyIncomePointResponse
            {
                Period = period,
                Label = cursor.ToString("MMM"),
                Salary = salaryMap.GetValueOrDefault(period),
                Business = revenueMap.GetValueOrDefault(period),
            });
        }

        return Result.Success(new MonthlyIncomeTrendResponse { Points = points });
    }

    public async Task<Result<SalaryResponse>> RecordSalaryAsync(
        RecordSalaryRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<SalaryResponse>(userResult.Error!);
        }

        Salary salary;
        if (request.SalaryId.HasValue)
        {
            var existing = await _salaryRepository.GetByIdForUserAsync(
                request.SalaryId.Value,
                userResult.Value,
                cancellationToken);

            if (existing is null)
            {
                return Result.Failure<SalaryResponse>(Error.NotFound(nameof(Salary), request.SalaryId.Value));
            }

            salary = existing;
            salary.MemberName = request.MemberName.Trim();
            salary.Employer = request.Employer.Trim();
            salary.Role = request.Role.Trim();
            salary.MonthlyAmount = request.MonthlyAmount;
            salary.CurrencyCode = NormalizeCurrency(request.CurrencyCode);
            salary.Status = request.Status;
            salary.NextExpectedOn = request.NextExpectedOn;
            salary.Notes = request.Notes;
            _salaryRepository.Update(salary);
        }
        else
        {
            salary = new Salary
            {
                UserId = userResult.Value,
                MemberName = request.MemberName.Trim(),
                Employer = request.Employer.Trim(),
                Role = request.Role.Trim(),
                MonthlyAmount = request.MonthlyAmount,
                CurrencyCode = NormalizeCurrency(request.CurrencyCode),
                Status = request.Status,
                NextExpectedOn = request.NextExpectedOn,
                Notes = request.Notes,
            };
            await _salaryRepository.AddAsync(salary, cancellationToken);
        }

        var period = request.Period.Trim();
        var payment = new SalaryPayment
        {
            SalaryId = salary.Id,
            UserId = userResult.Value,
            Amount = request.MonthlyAmount,
            PaidOn = request.PaidOn,
            Period = period,
            Notes = request.Notes,
        };

        salary.LastCreditedOn = request.PaidOn;
        salary.Payments.Add(payment);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var mapped = _mapper.Map<SalaryResponse>(salary);
        var response = new SalaryResponse
        {
            Id = mapped.Id,
            MemberName = mapped.MemberName,
            Employer = mapped.Employer,
            Role = mapped.Role,
            MonthlyAmount = mapped.MonthlyAmount,
            CurrencyCode = mapped.CurrencyCode,
            LastCreditedOn = mapped.LastCreditedOn,
            NextExpectedOn = mapped.NextExpectedOn,
            Status = mapped.Status,
            PaymentId = payment.Id,
            Notes = mapped.Notes,
        };

        return Result.Success(response);
    }

    private async Task<(decimal Salary, decimal Revenue, decimal Payroll, decimal Expenses)> LoadPeriodTotalsAsync(
        Guid userId,
        string period,
        CancellationToken cancellationToken)
    {
        var salary = await _salaryRepository.SumPaymentsForPeriodAsync(userId, period, cancellationToken);
        var revenue = await _invoiceRepository.SumPaymentsForPeriodAsync(userId, period, cancellationToken);
        var payroll = await _developerRepository.SumPayrollForPeriodAsync(userId, period, cancellationToken);
        var expenses = await _expenseRepository.SumForPeriodAsync(userId, period, cancellationToken);
        return (salary, revenue, payroll, expenses);
    }

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }

    private static string NormalizeCurrency(string? code) =>
        string.IsNullOrWhiteSpace(code) ? "INR" : code.Trim().ToUpperInvariant();
}

using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Domain.Loans.Enums;
using WealthOS.Domain.Loans.Models;
using WealthOS.Domain.Loans.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Repositories;

namespace WealthOS.Infrastructure.Loans.Repositories;

/// <summary>
/// EF Core repository for the Loan aggregate.
/// </summary>
public sealed class LoanRepository : Repository<Loan>, ILoanRepository
{
    public LoanRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Loan?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            loan => loan.Id == id && loan.UserId == userId,
            cancellationToken);

    public async Task<Loan?> GetByIdWithDetailsAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsSplitQuery()
            .Include(loan => loan.LoanProvider)
            .Include(loan => loan.Payments)
            .Include(loan => loan.Reminders)
            .Include(loan => loan.InterestRates)
            .Include(loan => loan.DocumentLinks)
            .Include(loan => loan.PropertyLinks)
            .FirstOrDefaultAsync(
                loan => loan.Id == id && loan.UserId == userId,
                cancellationToken);

    public async Task<(IReadOnlyList<Loan> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        LoanStatus? status,
        LoanType? type,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(loan => loan.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(loan =>
                loan.Name.ToLower().Contains(term)
                || loan.LenderName.ToLower().Contains(term)
                || (loan.AccountNumber != null && loan.AccountNumber.ToLower().Contains(term)));
        }

        if (status.HasValue)
        {
            query = query.Where(loan => loan.Status == status.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(loan => loan.Type == type.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(loan => loan.UpdatedAt ?? loan.CreatedAt)
            .ThenBy(loan => loan.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<LoanSummary> GetPortfolioSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var loans = await DbSet
            .AsNoTracking()
            .Where(loan => loan.UserId == userId)
            .Select(loan => new
            {
                loan.Principal,
                loan.OutstandingBalance,
                loan.EmiAmount,
                loan.NextEmiDate,
                loan.CurrencyCode,
                loan.Status,
                loan.PaymentFrequency,
            })
            .ToListAsync(cancellationToken);

        if (loans.Count == 0)
        {
            return new LoanSummary();
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var upcomingHorizon = today.AddDays(45);

        var monthlyEmi = loans
            .Where(loan => loan.Status is LoanStatus.Active or LoanStatus.Overdue)
            .Where(loan => loan.PaymentFrequency == PaymentFrequency.Monthly)
            .Sum(loan => loan.EmiAmount);

        var upcomingEmi = loans
            .Where(loan => loan.Status is LoanStatus.Active or LoanStatus.Overdue)
            .Where(loan => loan.NextEmiDate.HasValue
                && loan.NextEmiDate.Value >= today
                && loan.NextEmiDate.Value <= upcomingHorizon)
            .Sum(loan => loan.EmiAmount);

        return new LoanSummary
        {
            LoanCount = loans.Count,
            TotalLoanAmount = loans.Sum(loan => loan.Principal),
            OutstandingBalance = loans.Sum(loan => loan.OutstandingBalance),
            MonthlyEmi = monthlyEmi,
            UpcomingEmi = upcomingEmi,
            CurrencyCode = loans
                .GroupBy(loan => loan.CurrencyCode)
                .OrderByDescending(group => group.Count())
                .First()
                .Key,
            ActiveCount = loans.Count(loan => loan.Status == LoanStatus.Active),
            ClosedCount = loans.Count(loan => loan.Status == LoanStatus.Closed),
        };
    }

    public async Task<IReadOnlyList<LoanReminder>> GetUpcomingRemindersAsync(
        Guid userId,
        int daysAhead,
        int take,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var horizon = today.AddDays(daysAhead);

        return await Context.Set<LoanReminder>()
            .AsNoTracking()
            .Include(reminder => reminder.Loan)
            .Where(reminder =>
                !reminder.IsDismissed
                && reminder.Loan.UserId == userId
                && reminder.DueOn >= today
                && reminder.DueOn <= horizon)
            .OrderBy(reminder => reminder.DueOn)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Loan>> GetActiveLoansWithNextEmiAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Where(loan =>
                loan.UserId == userId
                && loan.Status == LoanStatus.Active
                && loan.NextEmiDate != null)
            .OrderBy(loan => loan.NextEmiDate)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        DbSet.AnyAsync(loan => loan.Id == id && loan.UserId == userId, cancellationToken);
}

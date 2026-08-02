using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Income.Entities;
using WealthOS.Domain.Income.Enums;
using WealthOS.Domain.Income.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Repositories;

namespace WealthOS.Infrastructure.Income.Repositories;

public sealed class DeveloperRepository : Repository<Developer>, IDeveloperRepository
{
    public DeveloperRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Developer?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(x => x.PrimaryClient)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

    public async Task<(IReadOnlyList<Developer> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(x => x.PrimaryClient)
            .Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.Name.ToLower().Contains(term) || x.Role.ToLower().Contains(term));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<(IReadOnlyList<DeveloperPayroll> Items, int TotalCount)> ListPayrollForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? period,
        PayrollStatus? status,
        Guid? developerId,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<DeveloperPayroll>()
            .AsNoTracking()
            .Include(x => x.Developer)
            .Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(period))
        {
            query = query.Where(x => x.Period == period.Trim());
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (developerId.HasValue)
        {
            query = query.Where(x => x.DeveloperId == developerId.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.Period)
            .ThenBy(x => x.Developer!.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<decimal> SumPayrollForPeriodAsync(
        Guid userId,
        string period,
        CancellationToken cancellationToken = default) =>
        await Context.Set<DeveloperPayroll>()
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Period == period)
            .SumAsync(x => x.Amount, cancellationToken);

    public async Task<bool> ExistsForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.AnyAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
}

public sealed class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Invoice?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

    public async Task<Invoice?> GetByIdWithDetailsAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsSplitQuery()
            .Include(x => x.Client)
            .Include(x => x.Items)
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

    public async Task<(IReadOnlyList<Invoice> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        Guid? clientId,
        InvoiceStatus? status,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.Items)
            .Include(x => x.Payments)
            .Where(x => x.UserId == userId);

        if (clientId.HasValue)
        {
            query = query.Where(x => x.ClientId == clientId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.InvoiceNumber.ToLower().Contains(term)
                || (x.Client != null && x.Client.Name.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.IssueDate)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<decimal> SumOutstandingAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var rows = await DbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Status != InvoiceStatus.Cancelled)
            .Select(x => new { x.SubTotal, x.AmountPaid })
            .ToListAsync(cancellationToken);

        return rows.Sum(x => Math.Max(0m, x.SubTotal - x.AmountPaid));
    }

    public async Task<decimal> SumPaymentsForPeriodAsync(
        Guid userId,
        string period,
        CancellationToken cancellationToken = default)
    {
        var start = DateOnly.ParseExact(period + "-01", "yyyy-MM-dd");
        var end = start.AddMonths(1);

        return await Context.Set<Payment>()
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.PaidOn >= start && x.PaidOn < end)
            .SumAsync(x => x.Amount, cancellationToken);
    }

    public async Task<IReadOnlyList<(string Period, decimal Amount)>> GetMonthlyRevenueTotalsAsync(
        Guid userId,
        string fromPeriod,
        string toPeriod,
        CancellationToken cancellationToken = default)
    {
        var from = DateOnly.ParseExact(fromPeriod + "-01", "yyyy-MM-dd");
        var toExclusive = DateOnly.ParseExact(toPeriod + "-01", "yyyy-MM-dd").AddMonths(1);

        var payments = await Context.Set<Payment>()
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.PaidOn >= from && x.PaidOn < toExclusive)
            .Select(x => new { x.PaidOn, x.Amount })
            .ToListAsync(cancellationToken);

        return payments
            .GroupBy(x => $"{x.PaidOn.Year:D4}-{x.PaidOn.Month:D2}")
            .Select(g => (Period: g.Key, Amount: g.Sum(x => x.Amount)))
            .OrderBy(x => x.Period)
            .ToList();
    }

    public async Task<IReadOnlyDictionary<Guid, (decimal Outstanding, decimal LastPaymentAmount, DateOnly? LastPaymentOn)>>
        GetClientPaymentStatsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var invoices = await DbSet
            .AsNoTracking()
            .Include(x => x.Payments)
            .Where(x => x.UserId == userId && x.Status != InvoiceStatus.Cancelled)
            .ToListAsync(cancellationToken);

        return invoices
            .GroupBy(x => x.ClientId)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var outstanding = g.Sum(inv => Math.Max(0m, inv.SubTotal - inv.AmountPaid));
                    var lastPayment = g.SelectMany(inv => inv.Payments)
                        .OrderByDescending(p => p.PaidOn)
                        .ThenByDescending(p => p.CreatedAt)
                        .FirstOrDefault();

                    return (
                        Outstanding: outstanding,
                        LastPaymentAmount: lastPayment?.Amount ?? 0m,
                        LastPaymentOn: lastPayment?.PaidOn);
                });
    }
}

public sealed class BusinessExpenseRepository : Repository<BusinessExpense>, IBusinessExpenseRepository
{
    public BusinessExpenseRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<BusinessExpense?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

    public async Task<(IReadOnlyList<BusinessExpense> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        Guid? categoryId,
        string? period,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.UserId == userId);

        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(period))
        {
            query = query.Where(x => x.Period == period.Trim());
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.PaidOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<decimal> SumForPeriodAsync(
        Guid userId,
        string period,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Period == period)
            .SumAsync(x => x.Amount, cancellationToken);

    public async Task<ExpenseCategory?> GetCategoryByIdForUserAsync(
        Guid categoryId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<ExpenseCategory>()
            .FirstOrDefaultAsync(x => x.Id == categoryId && x.UserId == userId, cancellationToken);

    public async Task<ExpenseCategory?> FindCategoryByNameAsync(
        Guid userId,
        string name,
        CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim().ToLowerInvariant();
        return await Context.Set<ExpenseCategory>()
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.Name.ToLower() == normalized,
                cancellationToken);
    }

    public async Task AddCategoryAsync(ExpenseCategory category, CancellationToken cancellationToken = default) =>
        await Context.Set<ExpenseCategory>().AddAsync(category, cancellationToken);

    public async Task<IReadOnlyList<ExpenseCategory>> ListCategoriesAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<ExpenseCategory>()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
}

public sealed class IncomeSourceRepository : Repository<IncomeSource>, IIncomeSourceRepository
{
    public IncomeSourceRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<(IReadOnlyList<IncomeSource> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(x => x.UserId == userId);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }
}

using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Income.Entities;
using WealthOS.Domain.Income.Enums;
using WealthOS.Domain.Income.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Repositories;

namespace WealthOS.Infrastructure.Income.Repositories;

public sealed class SalaryRepository : Repository<Salary>, ISalaryRepository
{
    public SalaryRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Salary?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

    public async Task<(IReadOnlyList<Salary> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(x => x.UserId == userId);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }

    public async Task<decimal> SumPaymentsForPeriodAsync(
        Guid userId,
        string period,
        CancellationToken cancellationToken = default) =>
        await Context.Set<SalaryPayment>()
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Period == period)
            .SumAsync(x => x.Amount, cancellationToken);

    public async Task<IReadOnlyList<(string Period, decimal Amount)>> GetMonthlySalaryTotalsAsync(
        Guid userId,
        string fromPeriod,
        string toPeriod,
        CancellationToken cancellationToken = default)
    {
        var rows = await Context.Set<SalaryPayment>()
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Period.CompareTo(fromPeriod) >= 0 && x.Period.CompareTo(toPeriod) <= 0)
            .GroupBy(x => x.Period)
            .Select(g => new { Period = g.Key, Amount = g.Sum(x => x.Amount) })
            .ToListAsync(cancellationToken);

        return rows.Select(x => (x.Period, x.Amount)).ToList();
    }
}

public sealed class BusinessClientRepository : Repository<BusinessClient>, IBusinessClientRepository
{
    public BusinessClientRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<BusinessClient?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

    public async Task<(IReadOnlyList<BusinessClient> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        ClientStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.Name.ToLower().Contains(term) || x.Engagement.ToLower().Contains(term));
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .ThenBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<bool> ExistsForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.AnyAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
}

public sealed class BusinessProjectRepository : Repository<BusinessProject>, IBusinessProjectRepository
{
    public BusinessProjectRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<BusinessProject?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

    public async Task<BusinessProject?> GetByIdWithDevelopersAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsSplitQuery()
            .Include(x => x.Client)
            .Include(x => x.Developers)
                .ThenInclude(x => x.Developer)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

    public async Task<(IReadOnlyList<BusinessProject> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        Guid? clientId,
        ProjectStatus? status,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.Developers)
                .ThenInclude(x => x.Developer)
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
            query = query.Where(x => x.Name.ToLower().Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}

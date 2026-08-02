using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Investments.Entities;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Domain.Investments.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Repositories;

namespace WealthOS.Infrastructure.Investments.Repositories;

public sealed class InvestmentProviderRepository : Repository<InvestmentProvider>, IInvestmentProviderRepository
{
    public InvestmentProviderRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<InvestmentProvider?> GetByKindAsync(
        ProviderKind kind,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(x => x.Kind == kind, cancellationToken);

    public async Task<IReadOnlyList<InvestmentProvider>> ListAllAsync(
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);
}

public sealed class InvestmentAccountRepository : Repository<InvestmentAccount>, IInvestmentAccountRepository
{
    public InvestmentAccountRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<InvestmentAccount?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

    public async Task<InvestmentAccount?> GetByIdWithProviderAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(x => x.Provider)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

    public async Task<(IReadOnlyList<InvestmentAccount> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        InvestmentAccountStatus? status,
        Guid? providerId,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Include(x => x.Provider).Where(x => x.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (providerId.HasValue)
        {
            query = query.Where(x => x.ProviderId == providerId.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<int> CountForUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await DbSet.CountAsync(x => x.UserId == userId, cancellationToken);

    public async Task<bool> ExistsForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.AnyAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
}

public sealed class HoldingRepository : Repository<Holding>, IHoldingRepository
{
    public HoldingRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Holding?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

    public async Task<(IReadOnlyList<Holding> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        Guid? accountId,
        InvestmentCategory? category,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Include(x => x.Account).Where(x => x.UserId == userId);

        if (accountId.HasValue)
        {
            query = query.Where(x => x.AccountId == accountId.Value);
        }

        if (category.HasValue)
        {
            query = query.Where(x => x.Category == category.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.Name.ToLower().Contains(term) || x.Symbol.ToLower().Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CurrentValue)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<Holding>> ListAllForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Include(x => x.Account)
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

    public async Task<int> CountForUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await DbSet.CountAsync(x => x.UserId == userId, cancellationToken);
}

public sealed class InvestmentTransactionRepository
    : Repository<InvestmentTransaction>, IInvestmentTransactionRepository
{
    public InvestmentTransactionRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<InvestmentTransaction?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

    public async Task<(IReadOnlyList<InvestmentTransaction> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        Guid? accountId,
        Guid? holdingId,
        InvestmentTransactionType? type,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Include(x => x.Holding).Where(x => x.UserId == userId);

        if (accountId.HasValue)
        {
            query = query.Where(x => x.AccountId == accountId.Value);
        }

        if (holdingId.HasValue)
        {
            query = query.Where(x => x.HoldingId == holdingId.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(x => x.TransactionType == type.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}

public sealed class PortfolioSnapshotRepository : Repository<PortfolioSnapshot>, IPortfolioSnapshotRepository
{
    public PortfolioSnapshotRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<(IReadOnlyList<PortfolioSnapshot> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        Guid? accountId,
        DateOnly? from,
        DateOnly? to,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(x => x.UserId == userId);

        if (accountId.HasValue)
        {
            query = query.Where(x => x.AccountId == accountId.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(x => x.SnapshotDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.SnapshotDate <= to.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.SnapshotDate).ToListAsync(cancellationToken);
        return (items, total);
    }

    public async Task<IReadOnlyList<PortfolioSnapshot>> ListRecentForUserAsync(
        Guid userId,
        int take,
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.SnapshotDate)
            .Take(take)
            .ToListAsync(cancellationToken);
}

public sealed class WatchlistRepository : Repository<WatchlistItem>, IWatchlistRepository
{
    public WatchlistRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<(IReadOnlyList<WatchlistItem> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(x => x.UserId == userId);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.Symbol)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }
}

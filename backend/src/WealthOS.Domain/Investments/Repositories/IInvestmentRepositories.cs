using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Investments.Entities;
using WealthOS.Domain.Investments.Enums;

namespace WealthOS.Domain.Investments.Repositories;

public interface IInvestmentProviderRepository : IRepository<InvestmentProvider>
{
    Task<InvestmentProvider?> GetByKindAsync(ProviderKind kind, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InvestmentProvider>> ListAllAsync(CancellationToken cancellationToken = default);
}

public interface IInvestmentAccountRepository : IRepository<InvestmentAccount>
{
    Task<InvestmentAccount?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<InvestmentAccount?> GetByIdWithProviderAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<InvestmentAccount> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        InvestmentAccountStatus? status,
        Guid? providerId,
        CancellationToken cancellationToken = default);

    Task<int> CountForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> ExistsForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}

public interface IHoldingRepository : IRepository<Holding>
{
    Task<Holding?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Holding> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        Guid? accountId,
        InvestmentCategory? category,
        string? search,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Holding>> ListAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<int> CountForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IInvestmentTransactionRepository : IRepository<InvestmentTransaction>
{
    Task<InvestmentTransaction?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<InvestmentTransaction> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        Guid? accountId,
        Guid? holdingId,
        InvestmentTransactionType? type,
        CancellationToken cancellationToken = default);
}

public interface IPortfolioSnapshotRepository : IRepository<PortfolioSnapshot>
{
    Task<(IReadOnlyList<PortfolioSnapshot> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        Guid? accountId,
        DateOnly? from,
        DateOnly? to,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PortfolioSnapshot>> ListRecentForUserAsync(
        Guid userId,
        int take,
        CancellationToken cancellationToken = default);
}

public interface IWatchlistRepository : IRepository<WatchlistItem>
{
    Task<(IReadOnlyList<WatchlistItem> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WealthOS.Domain.Investments.Entities;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Infrastructure.Persistence;

namespace WealthOS.Infrastructure.Investments;

/// <summary>
/// Seeds the investment provider catalog only.
/// Sample accounts / holdings / transactions are not seeded — users add real data.
/// Also removes legacy demo rows (fixed seed IDs) if they still exist.
/// </summary>
public static class InvestmentDataSeeder
{
    public static readonly Guid ProviderManualId = Guid.Parse("a1111111-1111-2222-3333-444444444401");
    public static readonly Guid ProviderAngelOneId = Guid.Parse("a1111111-1111-2222-3333-444444444402");
    public static readonly Guid ProviderIndiaBondsId = Guid.Parse("a1111111-1111-2222-3333-444444444403");
    public static readonly Guid ProviderGrowwId = Guid.Parse("a1111111-1111-2222-3333-444444444404");
    public static readonly Guid ProviderZerodhaId = Guid.Parse("a1111111-1111-2222-3333-444444444405");
    public static readonly Guid ProviderUpstoxId = Guid.Parse("a1111111-1111-2222-3333-444444444406");

    private static readonly Guid[] LegacyDemoAccountIds =
    [
        Guid.Parse("b2222222-1111-2222-3333-444444444401"),
        Guid.Parse("b2222222-1111-2222-3333-444444444402"),
        Guid.Parse("b2222222-1111-2222-3333-444444444403"),
        Guid.Parse("b2222222-1111-2222-3333-444444444404"),
    ];

    private static readonly Guid[] LegacyDemoHoldingIds =
    [
        Guid.Parse("c3333333-1111-2222-3333-444444444401"),
        Guid.Parse("c3333333-1111-2222-3333-444444444402"),
        Guid.Parse("c3333333-1111-2222-3333-444444444403"),
        Guid.Parse("c3333333-1111-2222-3333-444444444404"),
        Guid.Parse("c3333333-1111-2222-3333-444444444405"),
        Guid.Parse("c3333333-1111-2222-3333-444444444406"),
        Guid.Parse("c3333333-1111-2222-3333-444444444407"),
        Guid.Parse("c3333333-1111-2222-3333-444444444408"),
        Guid.Parse("c3333333-1111-2222-3333-444444444409"),
        Guid.Parse("c3333333-1111-2222-3333-444444444410"),
        Guid.Parse("c3333333-1111-2222-3333-444444444411"),
        Guid.Parse("c3333333-1111-2222-3333-444444444412"),
        Guid.Parse("c3333333-1111-2222-3333-444444444413"),
    ];

    private static readonly Guid[] LegacyDemoTransactionIds =
    [
        Guid.Parse("d4444444-1111-2222-3333-444444444401"),
        Guid.Parse("d4444444-1111-2222-3333-444444444402"),
        Guid.Parse("d4444444-1111-2222-3333-444444444403"),
        Guid.Parse("d4444444-1111-2222-3333-444444444404"),
    ];

    private static readonly Guid[] LegacyDemoSnapshotIds =
    [
        Guid.Parse("e5555555-1111-2222-3333-444444444401"),
        Guid.Parse("e5555555-1111-2222-3333-444444444402"),
        Guid.Parse("e5555555-1111-2222-3333-444444444403"),
        Guid.Parse("e5555555-1111-2222-3333-444444444404"),
        Guid.Parse("e5555555-1111-2222-3333-444444444405"),
        Guid.Parse("e5555555-1111-2222-3333-444444444406"),
    ];

    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("InvestmentDataSeeder");
        var dbContext = services.GetRequiredService<ApplicationDbContext>();

        await EnsureProvidersAsync(dbContext, logger, cancellationToken);
        await RemoveLegacyDemoDataAsync(dbContext, logger, cancellationToken);
    }

    private static async Task EnsureProvidersAsync(
        ApplicationDbContext dbContext,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var providers = new[]
        {
            new InvestmentProvider(ProviderManualId)
            {
                Kind = ProviderKind.Manual,
                Name = "Manual",
                Description = "Manually tracked investments (SGB, FD, unlisted).",
                IsEnabled = true,
                SupportsSync = false,
            },
            new InvestmentProvider(ProviderAngelOneId)
            {
                Kind = ProviderKind.AngelOne,
                Name = "Angel One",
                Description = "Angel One broker — connect and sync holdings from Investments.",
                IsEnabled = true,
                SupportsSync = true,
            },
            new InvestmentProvider(ProviderIndiaBondsId)
            {
                Kind = ProviderKind.IndiaBonds,
                Name = "IndiaBonds",
                Description = "Corporate bonds platform (integration coming soon).",
                IsEnabled = true,
                SupportsSync = false,
            },
            new InvestmentProvider(ProviderGrowwId)
            {
                Kind = ProviderKind.Groww,
                Name = "Groww",
                Description = "Future-ready stub.",
                IsEnabled = false,
                SupportsSync = false,
            },
            new InvestmentProvider(ProviderZerodhaId)
            {
                Kind = ProviderKind.Zerodha,
                Name = "Zerodha",
                Description = "Future-ready stub.",
                IsEnabled = false,
                SupportsSync = false,
            },
            new InvestmentProvider(ProviderUpstoxId)
            {
                Kind = ProviderKind.Upstox,
                Name = "Upstox",
                Description = "Future-ready stub.",
                IsEnabled = false,
                SupportsSync = false,
            },
        };

        var added = 0;
        foreach (var provider in providers)
        {
            var exists = await dbContext.InvestmentProviders.IgnoreQueryFilters()
                .AnyAsync(p => p.Id == provider.Id, cancellationToken);
            if (exists)
            {
                continue;
            }

            await dbContext.InvestmentProviders.AddAsync(provider, cancellationToken);
            added += 1;
        }

        if (added > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded {Count} investment provider catalog entries.", added);
        }
    }

    private static async Task RemoveLegacyDemoDataAsync(
        ApplicationDbContext dbContext,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        // Soft-delete by fixed seed IDs and by any rows still attached to those demo accounts.
        var accounts = await dbContext.InvestmentAccounts.IgnoreQueryFilters()
            .Where(a => LegacyDemoAccountIds.Contains(a.Id) && !a.IsDeleted)
            .ToListAsync(cancellationToken);

        var accountIds = LegacyDemoAccountIds.ToList();

        var holdings = await dbContext.Holdings.IgnoreQueryFilters()
            .Where(h => !h.IsDeleted &&
                        (LegacyDemoHoldingIds.Contains(h.Id) || accountIds.Contains(h.AccountId)))
            .ToListAsync(cancellationToken);
        foreach (var holding in holdings)
        {
            holding.IsDeleted = true;
            holding.DeletedAt = now;
        }

        var holdingIds = holdings.Select(h => h.Id).ToHashSet();

        var transactions = await dbContext.InvestmentTransactions.IgnoreQueryFilters()
            .Where(t => !t.IsDeleted &&
                        (LegacyDemoTransactionIds.Contains(t.Id) ||
                         accountIds.Contains(t.AccountId) ||
                         (t.HoldingId.HasValue && holdingIds.Contains(t.HoldingId.Value))))
            .ToListAsync(cancellationToken);
        foreach (var transaction in transactions)
        {
            transaction.IsDeleted = true;
            transaction.DeletedAt = now;
        }

        var snapshots = await dbContext.PortfolioSnapshots.IgnoreQueryFilters()
            .Where(s => !s.IsDeleted &&
                        (LegacyDemoSnapshotIds.Contains(s.Id) || accountIds.Contains(s.AccountId)))
            .ToListAsync(cancellationToken);
        foreach (var snapshot in snapshots)
        {
            snapshot.IsDeleted = true;
            snapshot.DeletedAt = now;
        }

        foreach (var account in accounts)
        {
            account.IsDeleted = true;
            account.DeletedAt = now;
            account.Status = InvestmentAccountStatus.Disconnected;
            account.LastSyncedAt = null;
        }

        var removed = holdings.Count + transactions.Count + snapshots.Count + accounts.Count;
        if (removed == 0)
        {
            return;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Removed legacy demo investments data: {Accounts} accounts, {Holdings} holdings, {Transactions} transactions, {Snapshots} snapshots.",
            accounts.Count,
            holdings.Count,
            transactions.Count,
            snapshots.Count);
    }
}

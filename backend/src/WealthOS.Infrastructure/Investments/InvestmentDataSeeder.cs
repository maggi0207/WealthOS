using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WealthOS.Domain.Authentication.Entities;
using WealthOS.Domain.Investments.Entities;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Infrastructure.Persistence;

namespace WealthOS.Infrastructure.Investments;

/// <summary>
/// Seeds Investments sample data aligned with frontend <c>investments-data.ts</c>.
/// </summary>
public static class InvestmentDataSeeder
{
    public static readonly Guid ProviderManualId = Guid.Parse("a1111111-1111-2222-3333-444444444401");
    public static readonly Guid ProviderAngelOneId = Guid.Parse("a1111111-1111-2222-3333-444444444402");
    public static readonly Guid ProviderIndiaBondsId = Guid.Parse("a1111111-1111-2222-3333-444444444403");
    public static readonly Guid ProviderGrowwId = Guid.Parse("a1111111-1111-2222-3333-444444444404");
    public static readonly Guid ProviderZerodhaId = Guid.Parse("a1111111-1111-2222-3333-444444444405");
    public static readonly Guid ProviderUpstoxId = Guid.Parse("a1111111-1111-2222-3333-444444444406");

    public static readonly Guid AccountAngelMageshId = Guid.Parse("b2222222-1111-2222-3333-444444444401");
    public static readonly Guid AccountAngelWifeId = Guid.Parse("b2222222-1111-2222-3333-444444444402");
    public static readonly Guid AccountIndiaBondsId = Guid.Parse("b2222222-1111-2222-3333-444444444403");
    public static readonly Guid AccountManualId = Guid.Parse("b2222222-1111-2222-3333-444444444404");

    public static readonly Guid HoldingHdfcBankId = Guid.Parse("c3333333-1111-2222-3333-444444444401");
    public static readonly Guid HoldingInfosysId = Guid.Parse("c3333333-1111-2222-3333-444444444402");
    public static readonly Guid HoldingTataMotorsId = Guid.Parse("c3333333-1111-2222-3333-444444444403");
    public static readonly Guid HoldingItcId = Guid.Parse("c3333333-1111-2222-3333-444444444404");
    public static readonly Guid HoldingRelianceId = Guid.Parse("c3333333-1111-2222-3333-444444444405");
    public static readonly Guid HoldingNiftyId = Guid.Parse("c3333333-1111-2222-3333-444444444406");
    public static readonly Guid HoldingPpfasId = Guid.Parse("c3333333-1111-2222-3333-444444444407");
    public static readonly Guid HoldingMiraeId = Guid.Parse("c3333333-1111-2222-3333-444444444408");
    public static readonly Guid HoldingHdfcBondId = Guid.Parse("c3333333-1111-2222-3333-444444444409");
    public static readonly Guid HoldingMuthootId = Guid.Parse("c3333333-1111-2222-3333-444444444410");
    public static readonly Guid HoldingGoldBeesId = Guid.Parse("c3333333-1111-2222-3333-444444444411");
    public static readonly Guid HoldingSgbId = Guid.Parse("c3333333-1111-2222-3333-444444444412");
    public static readonly Guid HoldingLiquidId = Guid.Parse("c3333333-1111-2222-3333-444444444413");

    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("InvestmentDataSeeder");
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        if (await dbContext.InvestmentProviders.IgnoreQueryFilters()
                .AnyAsync(p => p.Id == ProviderManualId, cancellationToken))
        {
            logger.LogInformation("Sample investments data already exists. Skipping seed.");
            return;
        }

        var adminUser = await userManager.Users
            .OrderBy(user => user.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (adminUser is null)
        {
            logger.LogWarning("No users found. Skipping investments seed until identity seed completes.");
            return;
        }

        var userId = adminUser.Id;
        var now = DateTime.UtcNow;

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

        var accounts = new[]
        {
            new InvestmentAccount(AccountAngelMageshId)
            {
                UserId = userId,
                ProviderId = ProviderAngelOneId,
                Name = "Angel One",
                OwnerName = "Magesh",
                KindLabel = "Broker · Stocks & MF",
                Status = InvestmentAccountStatus.Connected,
                LastSyncedAt = now.AddMinutes(-12),
            },
            new InvestmentAccount(AccountAngelWifeId)
            {
                UserId = userId,
                ProviderId = ProviderAngelOneId,
                Name = "Angel One",
                OwnerName = "Wife",
                KindLabel = "Broker · Stocks & MF",
                Status = InvestmentAccountStatus.Connected,
                LastSyncedAt = now.AddMinutes(-38),
            },
            new InvestmentAccount(AccountIndiaBondsId)
            {
                UserId = userId,
                ProviderId = ProviderIndiaBondsId,
                Name = "IndiaBonds",
                OwnerName = "Magesh",
                KindLabel = "Corporate bonds",
                Status = InvestmentAccountStatus.ComingSoon,
            },
            new InvestmentAccount(AccountManualId)
            {
                UserId = userId,
                ProviderId = ProviderManualId,
                Name = "Manual Investments",
                OwnerName = "Household",
                KindLabel = "SGB, FD & unlisted",
                Status = InvestmentAccountStatus.Manual,
                LastSyncedAt = now.AddDays(-2),
            },
        };

        var holdings = new[]
        {
            CreateHolding(HoldingHdfcBankId, userId, AccountAngelMageshId, "HDFC Bank", "HDFCBANK", InvestmentCategory.Stocks, InvestmentType.Equity, 12_40_000m, 9_10_000m, 9_800m, 0.79m),
            CreateHolding(HoldingInfosysId, userId, AccountAngelMageshId, "Infosys", "INFY", InvestmentCategory.Stocks, InvestmentType.Equity, 9_80_000m, 7_60_000m, -6_200m, -0.63m),
            CreateHolding(HoldingTataMotorsId, userId, AccountAngelWifeId, "Tata Motors", "TATAMOTORS", InvestmentCategory.Stocks, InvestmentType.Equity, 7_20_000m, 5_40_000m, 14_100m, 1.99m),
            CreateHolding(HoldingItcId, userId, AccountAngelWifeId, "ITC", "ITC", InvestmentCategory.Stocks, InvestmentType.Equity, 6_10_000m, 5_20_000m, 2_300m, 0.38m),
            CreateHolding(HoldingRelianceId, userId, AccountAngelMageshId, "Reliance Industries", "RELIANCE", InvestmentCategory.Stocks, InvestmentType.Equity, 16_80_000m, 12_90_000m, 21_400m, 1.29m),
            CreateHolding(HoldingNiftyId, userId, AccountAngelMageshId, "Nifty 50 Index Fund", "UTINIFTY", InvestmentCategory.MutualFunds, InvestmentType.MutualFund, 18_40_000m, 13_10_000m, 11_600m, 0.63m),
            CreateHolding(HoldingPpfasId, userId, AccountAngelMageshId, "Parag Parikh Flexi Cap", "PPFAS", InvestmentCategory.MutualFunds, InvestmentType.MutualFund, 14_10_000m, 9_80_000m, 7_900m, 0.56m),
            CreateHolding(HoldingMiraeId, userId, AccountAngelWifeId, "Mirae Emerging Bluechip", "MIRAE", InvestmentCategory.MutualFunds, InvestmentType.MutualFund, 9_10_000m, 7_40_000m, -3_100m, -0.34m),
            CreateHolding(HoldingHdfcBondId, userId, AccountIndiaBondsId, "HDFC Corporate Bond 8.4%", "HDFCCB28", InvestmentCategory.CorporateBonds, InvestmentType.Bond, 14_50_000m, 13_20_000m, 600m, 0.04m),
            CreateHolding(HoldingMuthootId, userId, AccountIndiaBondsId, "Muthoot Finance NCD 9.1%", "MUTHNCD", InvestmentCategory.CorporateBonds, InvestmentType.Bond, 10_00_000m, 9_40_000m, 500m, 0.05m),
            CreateHolding(HoldingGoldBeesId, userId, AccountManualId, "Nippon Gold ETF", "GOLDBEES", InvestmentCategory.GoldEtfs, InvestmentType.Etf, 13_20_000m, 9_60_000m, 8_400m, 0.64m),
            CreateHolding(HoldingSgbId, userId, AccountManualId, "Sovereign Gold Bond 2031", "SGB31", InvestmentCategory.GoldEtfs, InvestmentType.Gold, 8_60_000m, 6_40_000m, 3_700m, 0.43m),
            CreateHolding(HoldingLiquidId, userId, AccountManualId, "Liquid Fund — Idle Cash", "LIQUID", InvestmentCategory.Cash, InvestmentType.Cash, 7_00_000m, 6_90_000m, 300m, 0.04m),
        };

        var transactions = new[]
        {
            new InvestmentTransaction(Guid.Parse("d4444444-1111-2222-3333-444444444401"))
            {
                UserId = userId,
                AccountId = AccountAngelMageshId,
                HoldingId = HoldingNiftyId,
                TransactionType = InvestmentTransactionType.Sip,
                Quantity = 1,
                Price = 40_000m,
                Amount = 40_000m,
                TransactionDate = new DateOnly(2026, 7, 5),
                Notes = "Nifty 50 Index Fund SIP",
            },
            new InvestmentTransaction(Guid.Parse("d4444444-1111-2222-3333-444444444402"))
            {
                UserId = userId,
                AccountId = AccountAngelMageshId,
                HoldingId = HoldingRelianceId,
                TransactionType = InvestmentTransactionType.Buy,
                Quantity = 40,
                Price = 2_960m,
                Amount = 1_18_400m,
                TransactionDate = new DateOnly(2026, 7, 2),
                Notes = "Bought 40 · Reliance",
            },
            new InvestmentTransaction(Guid.Parse("d4444444-1111-2222-3333-444444444403"))
            {
                UserId = userId,
                AccountId = AccountAngelMageshId,
                HoldingId = HoldingInfosysId,
                TransactionType = InvestmentTransactionType.Dividend,
                Quantity = 0,
                Price = 0,
                Amount = 9_400m,
                TransactionDate = new DateOnly(2026, 6, 28),
                Notes = "Infosys dividend",
            },
            new InvestmentTransaction(Guid.Parse("d4444444-1111-2222-3333-444444444404"))
            {
                UserId = userId,
                AccountId = AccountAngelWifeId,
                HoldingId = HoldingItcId,
                TransactionType = InvestmentTransactionType.Sell,
                Quantity = 150,
                Price = 475m,
                Amount = 71_250m,
                TransactionDate = new DateOnly(2026, 6, 19),
                Notes = "Sold 150 · ITC",
            },
        };

        var snapshots = new List<PortfolioSnapshot>();
        var monthValues = new (int Year, int Month, decimal Value)[]
        {
            (2025, 12, 128_40_000m),
            (2026, 1, 131_90_000m),
            (2026, 2, 134_20_000m),
            (2026, 3, 138_60_000m),
            (2026, 4, 142_70_000m),
            (2026, 5, 147_20_000m),
        };

        var snapshotIndex = 0;
        foreach (var (year, month, value) in monthValues)
        {
            snapshots.Add(new PortfolioSnapshot(Guid.Parse($"e5555555-1111-2222-3333-4444444444{snapshotIndex + 1:D2}"))
            {
                UserId = userId,
                AccountId = AccountAngelMageshId,
                SnapshotDate = new DateOnly(year, month, 1),
                InvestedAmount = 1_26_00_000m,
                CurrentValue = value,
                DayChange = 10_000m,
                DayChangePercent = 0.1m,
            });
            snapshotIndex++;
        }

        await dbContext.InvestmentProviders.AddRangeAsync(providers, cancellationToken);
        await dbContext.InvestmentAccounts.AddRangeAsync(accounts, cancellationToken);
        await dbContext.Holdings.AddRangeAsync(holdings, cancellationToken);
        await dbContext.InvestmentTransactions.AddRangeAsync(transactions, cancellationToken);
        await dbContext.PortfolioSnapshots.AddRangeAsync(snapshots, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seeded investments module: {ProviderCount} providers, {AccountCount} accounts, {HoldingCount} holdings.",
            providers.Length,
            accounts.Length,
            holdings.Length);
    }

    private static Holding CreateHolding(
        Guid id,
        Guid userId,
        Guid accountId,
        string name,
        string symbol,
        InvestmentCategory category,
        InvestmentType type,
        decimal currentValue,
        decimal invested,
        decimal dayChange,
        decimal dayChangePct)
    {
        var quantity = currentValue > 0 && invested > 0 ? Math.Round(currentValue / Math.Max(invested / 100m, 1m), 4) : 1m;
        var avgCost = quantity == 0 ? 0 : Math.Round(invested / Math.Max(quantity, 1m), 2);
        var price = quantity == 0 ? 0 : Math.Round(currentValue / Math.Max(quantity, 1m), 2);

        return new Holding(id)
        {
            UserId = userId,
            AccountId = accountId,
            Name = name,
            Symbol = symbol,
            Category = category,
            InvestmentType = type,
            Quantity = quantity,
            AverageCost = avgCost,
            InvestedAmount = invested,
            CurrentPrice = price,
            CurrentValue = currentValue,
            DayChange = dayChange,
            DayChangePercent = dayChangePct,
        };
    }
}

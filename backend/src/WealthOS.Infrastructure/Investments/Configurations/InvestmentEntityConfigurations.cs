using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Investments.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Investments.Configurations;

public sealed class InvestmentProviderConfiguration : AuditableEntityConfiguration<InvestmentProvider>
{
    public override void Configure(EntityTypeBuilder<InvestmentProvider> builder)
    {
        base.Configure(builder);
        builder.ToTable("InvestmentProviders");
        builder.HasIndex(x => x.Kind).IsUnique();

        builder.Property(x => x.Kind).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.IsEnabled).IsRequired();
        builder.Property(x => x.SupportsSync).IsRequired();

        builder.HasMany(x => x.Accounts)
            .WithOne(x => x.Provider)
            .HasForeignKey(x => x.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class InvestmentAccountConfiguration : AuditableEntityConfiguration<InvestmentAccount>
{
    public override void Configure(EntityTypeBuilder<InvestmentAccount> builder)
    {
        base.Configure(builder);
        builder.ToTable("InvestmentAccounts");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ProviderId);
        builder.HasIndex(x => new { x.UserId, x.Status });

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.OwnerName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.KindLabel).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(4000);
        builder.Property(x => x.ExternalAccountReference).HasMaxLength(200);

        builder.HasMany(x => x.Holdings)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Snapshots)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Transactions)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class HoldingConfiguration : AuditableEntityConfiguration<Holding>
{
    public override void Configure(EntityTypeBuilder<Holding> builder)
    {
        base.Configure(builder);
        builder.ToTable("Holdings");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.AccountId);
        builder.HasIndex(x => new { x.UserId, x.Category });
        builder.HasIndex(x => new { x.UserId, x.Symbol });

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Symbol).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.InvestmentType).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Quantity).HasPrecision(18, 6).IsRequired();
        builder.Property(x => x.AverageCost).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.InvestedAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrentPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrentValue).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.DayChange).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.DayChangePercent).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(4000);

        builder.HasMany(x => x.Transactions)
            .WithOne(x => x.Holding)
            .HasForeignKey(x => x.HoldingId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Dividends)
            .WithOne(x => x.Holding)
            .HasForeignKey(x => x.HoldingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.CorporateActions)
            .WithOne(x => x.Holding)
            .HasForeignKey(x => x.HoldingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class InvestmentTransactionConfiguration : AuditableEntityConfiguration<InvestmentTransaction>
{
    public override void Configure(EntityTypeBuilder<InvestmentTransaction> builder)
    {
        base.Configure(builder);
        builder.ToTable("InvestmentTransactions");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.AccountId);
        builder.HasIndex(x => x.HoldingId);
        builder.HasIndex(x => new { x.UserId, x.TransactionDate });

        builder.Property(x => x.TransactionType).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Quantity).HasPrecision(18, 6).IsRequired();
        builder.Property(x => x.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Fees).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.Property(x => x.ExternalReference).HasMaxLength(200);
    }
}

public sealed class PortfolioSnapshotConfiguration : AuditableEntityConfiguration<PortfolioSnapshot>
{
    public override void Configure(EntityTypeBuilder<PortfolioSnapshot> builder)
    {
        base.Configure(builder);
        builder.ToTable("PortfolioSnapshots");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.AccountId);
        builder.HasIndex(x => new { x.AccountId, x.SnapshotDate }).IsUnique();

        builder.Property(x => x.InvestedAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrentValue).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.DayChange).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.DayChangePercent).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
    }
}

public sealed class DividendConfiguration : AuditableEntityConfiguration<Dividend>
{
    public override void Configure(EntityTypeBuilder<Dividend> builder)
    {
        base.Configure(builder);
        builder.ToTable("Dividends");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.HoldingId);
        builder.HasIndex(x => x.AccountId);

        builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(2000);
    }
}

public sealed class CorporateActionConfiguration : AuditableEntityConfiguration<CorporateAction>
{
    public override void Configure(EntityTypeBuilder<CorporateAction> builder)
    {
        base.Configure(builder);
        builder.ToTable("CorporateActions");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.HoldingId);

        builder.Property(x => x.ActionType).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Ratio).HasMaxLength(64);
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(2000);
    }
}

public sealed class WatchlistItemConfiguration : AuditableEntityConfiguration<WatchlistItem>
{
    public override void Configure(EntityTypeBuilder<WatchlistItem> builder)
    {
        base.Configure(builder);
        builder.ToTable("WatchlistItems");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.UserId, x.Symbol });

        builder.Property(x => x.Symbol).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.TargetPrice).HasPrecision(18, 2);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(2000);
    }
}

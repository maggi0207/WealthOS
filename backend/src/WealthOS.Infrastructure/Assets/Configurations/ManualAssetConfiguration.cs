using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Assets.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Assets.Configurations;

public sealed class ManualAssetConfiguration : AuditableEntityConfiguration<ManualAsset>
{
    public override void Configure(EntityTypeBuilder<ManualAsset> builder)
    {
        base.Configure(builder);

        builder.ToTable("ManualAssets");

        builder.HasIndex(asset => asset.UserId);
        builder.HasIndex(asset => new { asset.UserId, asset.Type });
        builder.HasIndex(asset => new { asset.UserId, asset.Name });

        builder.Property(asset => asset.Type)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(asset => asset.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(asset => asset.PurchaseValue)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(asset => asset.CurrentValue)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(asset => asset.Quantity)
            .HasPrecision(18, 4);

        builder.Property(asset => asset.Institution)
            .HasMaxLength(200);

        builder.Property(asset => asset.Notes)
            .HasMaxLength(4000);

        builder.Property(asset => asset.CurrencyCode)
            .HasMaxLength(3)
            .IsRequired();
    }
}

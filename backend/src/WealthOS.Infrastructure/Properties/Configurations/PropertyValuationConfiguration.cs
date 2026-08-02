using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Properties.Configurations;

public sealed class PropertyValuationConfiguration : AuditableEntityConfiguration<PropertyValuation>
{
    public override void Configure(EntityTypeBuilder<PropertyValuation> builder)
    {
        base.Configure(builder);

        builder.ToTable("PropertyValuations");

        builder.HasIndex(valuation => valuation.PropertyId);
        builder.HasIndex(valuation => new { valuation.PropertyId, valuation.ValuationDate });

        builder.Property(valuation => valuation.Value)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(valuation => valuation.CurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(valuation => valuation.Source)
            .HasMaxLength(128);

        builder.Property(valuation => valuation.Notes)
            .HasMaxLength(1000);
    }
}

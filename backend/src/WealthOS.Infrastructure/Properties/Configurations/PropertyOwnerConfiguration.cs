using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Properties.Configurations;

public sealed class PropertyOwnerConfiguration : AuditableEntityConfiguration<PropertyOwner>
{
    public override void Configure(EntityTypeBuilder<PropertyOwner> builder)
    {
        base.Configure(builder);

        builder.ToTable("PropertyOwners");

        builder.HasIndex(owner => owner.PropertyId);

        builder.Property(owner => owner.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(owner => owner.OwnershipPercentage)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(owner => owner.OwnershipType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
    }
}

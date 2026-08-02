using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Properties.Configurations;

public sealed class PropertyAddressConfiguration : AuditableEntityConfiguration<PropertyAddress>
{
    public override void Configure(EntityTypeBuilder<PropertyAddress> builder)
    {
        base.Configure(builder);

        builder.ToTable("PropertyAddresses");

        builder.HasIndex(address => address.PropertyId)
            .IsUnique();

        builder.Property(address => address.Line1).HasMaxLength(256);
        builder.Property(address => address.Line2).HasMaxLength(256);
        builder.Property(address => address.Locality).HasMaxLength(128);
        builder.Property(address => address.City).HasMaxLength(128);
        builder.Property(address => address.State).HasMaxLength(128);
        builder.Property(address => address.PostalCode).HasMaxLength(32);
        builder.Property(address => address.Country).HasMaxLength(128);
        builder.Property(address => address.FullAddress).HasMaxLength(512);
        builder.Property(address => address.GoogleMapsUrl).HasMaxLength(1024);

        builder.Property(address => address.Latitude)
            .HasPrecision(9, 6);

        builder.Property(address => address.Longitude)
            .HasPrecision(9, 6);
    }
}

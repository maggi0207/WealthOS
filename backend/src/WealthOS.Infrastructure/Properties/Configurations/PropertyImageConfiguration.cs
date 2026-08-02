using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Properties.Configurations;

public sealed class PropertyImageConfiguration : AuditableEntityConfiguration<PropertyImage>
{
    public override void Configure(EntityTypeBuilder<PropertyImage> builder)
    {
        base.Configure(builder);

        builder.ToTable("PropertyImages");

        builder.HasIndex(image => image.PropertyId);
        builder.HasIndex(image => new { image.PropertyId, image.SortOrder });

        builder.Property(image => image.Url)
            .HasMaxLength(2048);

        builder.Property(image => image.StorageKey)
            .HasMaxLength(512);

        builder.Property(image => image.Caption)
            .HasMaxLength(256);

        builder.Property(image => image.Category)
            .HasMaxLength(64);
    }
}

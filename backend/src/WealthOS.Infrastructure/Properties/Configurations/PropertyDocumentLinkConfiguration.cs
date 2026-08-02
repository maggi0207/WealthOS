using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Properties.Configurations;

public sealed class PropertyDocumentLinkConfiguration : AuditableEntityConfiguration<PropertyDocumentLink>
{
    public override void Configure(EntityTypeBuilder<PropertyDocumentLink> builder)
    {
        base.Configure(builder);

        builder.ToTable("PropertyDocumentLinks");

        builder.HasIndex(link => link.PropertyId);
        builder.HasIndex(link => link.DocumentId);
        builder.HasIndex(link => new { link.PropertyId, link.DocumentId })
            .IsUnique();

        builder.Property(link => link.Notes)
            .HasMaxLength(500);
    }
}

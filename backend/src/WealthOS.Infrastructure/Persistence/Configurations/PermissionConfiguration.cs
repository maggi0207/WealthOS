using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Authentication.Entities;

namespace WealthOS.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration : AuditableEntityConfiguration<Permission>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        builder.ToTable("Permissions");

        builder.Property(permission => permission.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(permission => permission.NormalizedName)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(permission => permission.NormalizedName)
            .IsUnique();

        builder.Property(permission => permission.Description)
            .HasMaxLength(500);
    }
}

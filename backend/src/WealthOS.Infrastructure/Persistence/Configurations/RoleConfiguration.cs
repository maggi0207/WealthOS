using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Authentication.Entities;

namespace WealthOS.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.Property(role => role.Description)
            .HasMaxLength(500);

        builder.Property(role => role.CreatedAt)
            .IsRequired();

        builder.Property(role => role.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();
    }
}

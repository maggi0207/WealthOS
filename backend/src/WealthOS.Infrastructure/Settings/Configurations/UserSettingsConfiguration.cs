using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Settings.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Settings.Configurations;

public sealed class UserSettingsConfiguration : AuditableEntityConfiguration<UserSettings>
{
    public override void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserSettings");

        builder.HasIndex(settings => settings.UserId).IsUnique();

        builder.Property(settings => settings.WorkspaceName).HasMaxLength(200).IsRequired();
        builder.Property(settings => settings.AvatarUrl).HasMaxLength(1000);
        builder.Property(settings => settings.Timezone).HasMaxLength(100).IsRequired();
        builder.Property(settings => settings.Country).HasMaxLength(8).IsRequired();
        builder.Property(settings => settings.Theme).HasMaxLength(32).IsRequired();
        builder.Property(settings => settings.LayoutDensity).HasMaxLength(32).IsRequired();
        builder.Property(settings => settings.CurrencyCode).HasMaxLength(8).IsRequired();
        builder.Property(settings => settings.Locale).HasMaxLength(32).IsRequired();
        builder.Property(settings => settings.DateFormat).HasMaxLength(32).IsRequired();
        builder.Property(settings => settings.NumberFormat).HasMaxLength(32).IsRequired();
    }
}

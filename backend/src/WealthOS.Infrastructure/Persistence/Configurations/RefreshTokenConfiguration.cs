using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Authentication.Entities;

namespace WealthOS.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.ToTable("RefreshTokens");

        builder.Property(token => token.Token)
            .HasMaxLength(512)
            .IsRequired();

        builder.HasIndex(token => token.Token)
            .IsUnique();

        builder.Property(token => token.CreatedByIp)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(token => token.RevokedByIp)
            .HasMaxLength(64);

        builder.Property(token => token.ReplacedByToken)
            .HasMaxLength(512);

        builder.Property(token => token.ExpiresAt)
            .IsRequired();

        builder.Property(token => token.CreatedAt)
            .IsRequired();

        builder.HasIndex(token => token.UserId);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Loans.Configurations;

public sealed class LoanProviderConfiguration : AuditableEntityConfiguration<LoanProvider>
{
    public override void Configure(EntityTypeBuilder<LoanProvider> builder)
    {
        base.Configure(builder);

        builder.ToTable("LoanProviders");

        builder.HasIndex(provider => provider.UserId);
        builder.HasIndex(provider => new { provider.UserId, provider.Name });

        builder.Property(provider => provider.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(provider => provider.Code)
            .HasMaxLength(64);

        builder.Property(provider => provider.ContactPhone)
            .HasMaxLength(32);

        builder.Property(provider => provider.ContactEmail)
            .HasMaxLength(256);

        builder.Property(provider => provider.Website)
            .HasMaxLength(512);

        builder.Property(provider => provider.Notes)
            .HasMaxLength(2000);

        builder.HasMany(provider => provider.Loans)
            .WithOne(loan => loan.LoanProvider)
            .HasForeignKey(loan => loan.LoanProviderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

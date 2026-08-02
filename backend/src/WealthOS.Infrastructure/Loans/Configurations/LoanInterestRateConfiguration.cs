using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Loans.Configurations;

public sealed class LoanInterestRateConfiguration : AuditableEntityConfiguration<LoanInterestRate>
{
    public override void Configure(EntityTypeBuilder<LoanInterestRate> builder)
    {
        base.Configure(builder);

        builder.ToTable("LoanInterestRates");

        builder.HasIndex(rate => rate.LoanId);
        builder.HasIndex(rate => new { rate.LoanId, rate.EffectiveFrom });

        builder.Property(rate => rate.RatePercent)
            .HasPrecision(8, 4)
            .IsRequired();

        builder.Property(rate => rate.InterestType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(rate => rate.Reason)
            .HasMaxLength(200);

        builder.Property(rate => rate.Notes)
            .HasMaxLength(1000);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Loans.Configurations;

public sealed class LoanPaymentConfiguration : AuditableEntityConfiguration<LoanPayment>
{
    public override void Configure(EntityTypeBuilder<LoanPayment> builder)
    {
        base.Configure(builder);

        builder.ToTable("LoanPayments");

        builder.HasIndex(payment => payment.LoanId);
        builder.HasIndex(payment => new { payment.LoanId, payment.PaidOn });

        builder.Property(payment => payment.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(payment => payment.PrincipalComponent)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(payment => payment.InterestComponent)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(payment => payment.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(payment => payment.PaymentMode)
            .HasMaxLength(64);

        builder.Property(payment => payment.Reference)
            .HasMaxLength(128);

        builder.Property(payment => payment.Notes)
            .HasMaxLength(1000);
    }
}

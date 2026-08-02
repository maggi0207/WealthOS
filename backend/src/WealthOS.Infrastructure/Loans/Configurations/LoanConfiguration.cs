using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Loans.Configurations;

public sealed class LoanConfiguration : AuditableEntityConfiguration<Loan>
{
    public override void Configure(EntityTypeBuilder<Loan> builder)
    {
        base.Configure(builder);

        builder.ToTable("Loans");

        builder.HasIndex(loan => loan.UserId);
        builder.HasIndex(loan => new { loan.UserId, loan.Status });
        builder.HasIndex(loan => new { loan.UserId, loan.Type });
        builder.HasIndex(loan => loan.LinkedPropertyId);
        builder.HasIndex(loan => loan.NextEmiDate);
        builder.HasIndex(loan => loan.LoanProviderId);

        builder.Property(loan => loan.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(loan => loan.Type)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(loan => loan.LenderName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(loan => loan.AccountNumber)
            .HasMaxLength(64);

        builder.Property(loan => loan.Principal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(loan => loan.OutstandingBalance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(loan => loan.InterestRate)
            .HasPrecision(8, 4)
            .IsRequired();

        builder.Property(loan => loan.InterestType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(loan => loan.EmiAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(loan => loan.PaymentFrequency)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(loan => loan.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(loan => loan.CurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(loan => loan.Notes)
            .HasMaxLength(4000);

        builder.HasOne(loan => loan.LinkedProperty)
            .WithMany()
            .HasForeignKey(loan => loan.LinkedPropertyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(loan => loan.Payments)
            .WithOne(payment => payment.Loan)
            .HasForeignKey(payment => payment.LoanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(loan => loan.Schedules)
            .WithOne(schedule => schedule.Loan)
            .HasForeignKey(schedule => schedule.LoanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(loan => loan.Reminders)
            .WithOne(reminder => reminder.Loan)
            .HasForeignKey(reminder => reminder.LoanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(loan => loan.InterestRates)
            .WithOne(rate => rate.Loan)
            .HasForeignKey(rate => rate.LoanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(loan => loan.DocumentLinks)
            .WithOne(link => link.Loan)
            .HasForeignKey(link => link.LoanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(loan => loan.PropertyLinks)
            .WithOne(link => link.Loan)
            .HasForeignKey(link => link.LoanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

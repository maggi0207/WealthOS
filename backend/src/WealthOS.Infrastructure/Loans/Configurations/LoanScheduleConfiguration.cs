using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Loans.Configurations;

public sealed class LoanScheduleConfiguration : AuditableEntityConfiguration<LoanSchedule>
{
    public override void Configure(EntityTypeBuilder<LoanSchedule> builder)
    {
        base.Configure(builder);

        builder.ToTable("LoanSchedules");

        builder.HasIndex(schedule => schedule.LoanId);
        builder.HasIndex(schedule => new { schedule.LoanId, schedule.InstalmentNumber })
            .IsUnique();

        builder.Property(schedule => schedule.EmiAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(schedule => schedule.PrincipalComponent)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(schedule => schedule.InterestComponent)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(schedule => schedule.OpeningBalance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(schedule => schedule.ClosingBalance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(schedule => schedule.LoanPayment)
            .WithMany()
            .HasForeignKey(schedule => schedule.LoanPaymentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

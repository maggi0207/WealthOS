using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Loans.Configurations;

public sealed class LoanReminderConfiguration : AuditableEntityConfiguration<LoanReminder>
{
    public override void Configure(EntityTypeBuilder<LoanReminder> builder)
    {
        base.Configure(builder);

        builder.ToTable("LoanReminders");

        builder.HasIndex(reminder => reminder.LoanId);
        builder.HasIndex(reminder => reminder.DueOn);

        builder.Property(reminder => reminder.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(reminder => reminder.Detail)
            .HasMaxLength(500);

        builder.Property(reminder => reminder.Amount)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}

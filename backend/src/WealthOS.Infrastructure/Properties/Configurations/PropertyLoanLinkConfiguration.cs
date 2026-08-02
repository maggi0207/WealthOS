using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Properties.Configurations;

public sealed class PropertyLoanLinkConfiguration : AuditableEntityConfiguration<PropertyLoanLink>
{
    public override void Configure(EntityTypeBuilder<PropertyLoanLink> builder)
    {
        base.Configure(builder);

        builder.ToTable("PropertyLoanLinks");

        builder.HasIndex(link => link.PropertyId);
        builder.HasIndex(link => link.LoanId);
        builder.HasIndex(link => new { link.PropertyId, link.LoanId })
            .IsUnique();

        builder.Property(link => link.Notes)
            .HasMaxLength(500);

        builder.HasOne(link => link.Loan)
            .WithMany()
            .HasForeignKey(link => link.LoanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

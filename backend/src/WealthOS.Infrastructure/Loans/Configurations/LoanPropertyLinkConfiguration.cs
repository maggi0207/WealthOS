using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Loans.Configurations;

public sealed class LoanPropertyLinkConfiguration : AuditableEntityConfiguration<LoanPropertyLink>
{
    public override void Configure(EntityTypeBuilder<LoanPropertyLink> builder)
    {
        base.Configure(builder);

        builder.ToTable("LoanPropertyLinks");

        builder.HasIndex(link => link.LoanId);
        builder.HasIndex(link => link.PropertyId);
        builder.HasIndex(link => new { link.LoanId, link.PropertyId })
            .IsUnique();

        builder.Property(link => link.Notes)
            .HasMaxLength(500);

        builder.HasOne(link => link.Property)
            .WithMany()
            .HasForeignKey(link => link.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

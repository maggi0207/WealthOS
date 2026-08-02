using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Loans.Configurations;

public sealed class LoanDocumentLinkConfiguration : AuditableEntityConfiguration<LoanDocumentLink>
{
    public override void Configure(EntityTypeBuilder<LoanDocumentLink> builder)
    {
        base.Configure(builder);

        builder.ToTable("LoanDocumentLinks");

        builder.HasIndex(link => link.LoanId);
        builder.HasIndex(link => link.DocumentId);
        builder.HasIndex(link => new { link.LoanId, link.DocumentId })
            .IsUnique();

        builder.Property(link => link.Notes)
            .HasMaxLength(500);
    }
}

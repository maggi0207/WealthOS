using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Properties.Configurations;

public sealed class PropertyNoteConfiguration : AuditableEntityConfiguration<PropertyNote>
{
    public override void Configure(EntityTypeBuilder<PropertyNote> builder)
    {
        base.Configure(builder);

        builder.ToTable("PropertyNotes");

        builder.HasIndex(note => note.PropertyId);

        builder.Property(note => note.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(note => note.Body)
            .HasMaxLength(4000)
            .IsRequired();
    }
}

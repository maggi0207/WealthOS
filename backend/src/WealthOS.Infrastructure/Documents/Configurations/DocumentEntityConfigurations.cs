using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Documents.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Documents.Configurations;

public sealed class DocumentConfiguration : AuditableEntityConfiguration<Document>
{
    public override void Configure(EntityTypeBuilder<Document> builder)
    {
        base.Configure(builder);

        builder.ToTable("Documents");

        builder.HasIndex(document => document.UserId);
        builder.HasIndex(document => new { document.UserId, document.Category });
        builder.HasIndex(document => new { document.UserId, document.Status });
        builder.HasIndex(document => new { document.UserId, document.ExpiryDate });
        builder.HasIndex(document => new { document.UserId, document.Owner });
        builder.HasIndex(document => new { document.ReferenceModule, document.ReferenceId });
        builder.HasIndex(document => document.Title);

        builder.Property(document => document.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(document => document.Description)
            .HasMaxLength(4000);

        builder.Property(document => document.Category)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(document => document.Owner)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(document => document.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(document => document.AccessLevel)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(document => document.ReferenceModule)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(document => document.Notes)
            .HasMaxLength(4000);

        builder.Property(document => document.OriginalFileName)
            .HasMaxLength(500);

        builder.Property(document => document.ContentType)
            .HasMaxLength(128);

        builder.Property(document => document.StorageProvider)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(document => document.StoragePath)
            .HasMaxLength(1000);

        // Soft GUID primary reference only — no FK to other modules.
        builder.Property(document => document.ReferenceId);

        builder.HasOne(document => document.Metadata)
            .WithOne(metadata => metadata.Document)
            .HasForeignKey<DocumentMetadata>(metadata => metadata.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(document => document.Tags)
            .WithOne(tag => tag.Document)
            .HasForeignKey(tag => tag.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(document => document.Versions)
            .WithOne(version => version.Document)
            .HasForeignKey(version => version.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(document => document.Links)
            .WithOne(link => link.Document)
            .HasForeignKey(link => link.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(document => document.Reminders)
            .WithOne(reminder => reminder.Document)
            .HasForeignKey(reminder => reminder.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class DocumentMetadataConfiguration : AuditableEntityConfiguration<DocumentMetadata>
{
    public override void Configure(EntityTypeBuilder<DocumentMetadata> builder)
    {
        base.Configure(builder);

        builder.ToTable("DocumentMetadata");

        builder.HasIndex(metadata => metadata.DocumentId).IsUnique();

        builder.Property(metadata => metadata.DocumentNumber).HasMaxLength(128);
        builder.Property(metadata => metadata.IssuedBy).HasMaxLength(200);
        builder.Property(metadata => metadata.IssuerCountry).HasMaxLength(64);
        builder.Property(metadata => metadata.Checksum).HasMaxLength(128);
        builder.Property(metadata => metadata.CustomAttributesJson).HasMaxLength(8000);
    }
}

public sealed class DocumentTagConfiguration : AuditableEntityConfiguration<DocumentTag>
{
    public override void Configure(EntityTypeBuilder<DocumentTag> builder)
    {
        base.Configure(builder);

        builder.ToTable("DocumentTags");

        builder.HasIndex(tag => tag.DocumentId);
        builder.HasIndex(tag => new { tag.DocumentId, tag.Name }).IsUnique();

        builder.Property(tag => tag.Name)
            .HasMaxLength(64)
            .IsRequired();
    }
}

public sealed class DocumentVersionConfiguration : AuditableEntityConfiguration<DocumentVersion>
{
    public override void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        base.Configure(builder);

        builder.ToTable("DocumentVersions");

        builder.HasIndex(version => version.DocumentId);
        builder.HasIndex(version => new { version.DocumentId, version.VersionNumber }).IsUnique();

        builder.Property(version => version.OriginalFileName).HasMaxLength(500);
        builder.Property(version => version.ContentType).HasMaxLength(128);
        builder.Property(version => version.StorageProvider)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        builder.Property(version => version.StoragePath).HasMaxLength(1000);
        builder.Property(version => version.Notes).HasMaxLength(1000);
    }
}

public sealed class DocumentLinkConfiguration : AuditableEntityConfiguration<DocumentLink>
{
    public override void Configure(EntityTypeBuilder<DocumentLink> builder)
    {
        base.Configure(builder);

        builder.ToTable("DocumentLinks");

        builder.HasIndex(link => link.DocumentId);
        builder.HasIndex(link => new { link.ReferenceModule, link.ReferenceId });
        builder.HasIndex(link => new { link.DocumentId, link.ReferenceModule, link.ReferenceId });

        builder.Property(link => link.ReferenceModule)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(link => link.Notes).HasMaxLength(1000);
    }
}

public sealed class DocumentReminderConfiguration : AuditableEntityConfiguration<DocumentReminder>
{
    public override void Configure(EntityTypeBuilder<DocumentReminder> builder)
    {
        base.Configure(builder);

        builder.ToTable("DocumentReminders");

        builder.HasIndex(reminder => reminder.DocumentId);
        builder.HasIndex(reminder => new { reminder.DocumentId, reminder.ReminderDate });
        builder.HasIndex(reminder => reminder.IsDismissed);

        builder.Property(reminder => reminder.Message)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(reminder => reminder.Notes).HasMaxLength(1000);
    }
}

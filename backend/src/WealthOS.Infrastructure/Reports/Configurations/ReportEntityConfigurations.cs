using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Reports.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Reports.Configurations;

public sealed class ReportDefinitionConfiguration : AuditableEntityConfiguration<ReportDefinition>
{
    public override void Configure(EntityTypeBuilder<ReportDefinition> builder)
    {
        base.Configure(builder);

        builder.ToTable("ReportDefinitions");
        builder.HasIndex(entity => entity.Code).IsUnique();
        builder.HasIndex(entity => entity.ReportType);
        builder.Property(entity => entity.Code).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.Name).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.Description).HasMaxLength(2000).IsRequired();
        builder.Property(entity => entity.ReportType).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.DefaultFiltersJson).HasMaxLength(8000);
        builder.Property(entity => entity.ParameterSchemaJson).HasMaxLength(8000);
    }
}

public sealed class ReportConfiguration : AuditableEntityConfiguration<Report>
{
    public override void Configure(EntityTypeBuilder<Report> builder)
    {
        base.Configure(builder);

        builder.ToTable("Reports");
        builder.HasIndex(entity => entity.UserId);
        builder.HasIndex(entity => new { entity.UserId, entity.ReportType });
        builder.Property(entity => entity.Title).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.ReportType).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.LastFiltersJson).HasMaxLength(8000);

        builder.HasOne(entity => entity.Definition)
            .WithMany()
            .HasForeignKey(entity => entity.ReportDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ReportExecutionConfiguration : AuditableEntityConfiguration<ReportExecution>
{
    public override void Configure(EntityTypeBuilder<ReportExecution> builder)
    {
        base.Configure(builder);

        builder.ToTable("ReportExecutions");
        builder.HasIndex(entity => entity.UserId);
        builder.HasIndex(entity => new { entity.UserId, entity.StartedAt });
        builder.HasIndex(entity => entity.ReportType);
        builder.Property(entity => entity.ReportType).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.FiltersJson).HasMaxLength(8000);
        builder.Property(entity => entity.ParametersJson).HasMaxLength(8000);
        builder.Property(entity => entity.ResultSummaryJson).HasMaxLength(32000);
        builder.Property(entity => entity.ErrorMessage).HasMaxLength(2000);

        builder.HasOne(entity => entity.Report)
            .WithMany()
            .HasForeignKey(entity => entity.ReportId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(entity => entity.Definition)
            .WithMany()
            .HasForeignKey(entity => entity.ReportDefinitionId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}

public sealed class ReportSnapshotConfiguration : AuditableEntityConfiguration<ReportSnapshot>
{
    public override void Configure(EntityTypeBuilder<ReportSnapshot> builder)
    {
        base.Configure(builder);

        builder.ToTable("ReportSnapshots");
        builder.HasIndex(entity => entity.UserId);
        builder.HasIndex(entity => new { entity.UserId, entity.CapturedAt });
        builder.Property(entity => entity.Title).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.ReportType).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.PayloadJson).HasColumnType("text").IsRequired();
        builder.Property(entity => entity.FiltersJson).HasMaxLength(8000);
        builder.Property(entity => entity.CurrencyCode).HasMaxLength(3).IsRequired();

        builder.HasOne(entity => entity.Execution)
            .WithMany()
            .HasForeignKey(entity => entity.ReportExecutionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public sealed class ReportExportConfiguration : AuditableEntityConfiguration<ReportExport>
{
    public override void Configure(EntityTypeBuilder<ReportExport> builder)
    {
        base.Configure(builder);

        builder.ToTable("ReportExports");
        builder.HasIndex(entity => entity.UserId);
        builder.HasIndex(entity => new { entity.UserId, entity.RequestedAt });
        builder.Property(entity => entity.ReportType).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.Format).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.FileName).HasMaxLength(260);
        builder.Property(entity => entity.ContentType).HasMaxLength(120);
        builder.Property(entity => entity.Message).HasMaxLength(2000);

        builder.HasOne(entity => entity.Snapshot)
            .WithMany()
            .HasForeignKey(entity => entity.ReportSnapshotId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

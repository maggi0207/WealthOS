using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Income.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Income.Configurations;

public sealed class SalaryConfiguration : AuditableEntityConfiguration<Salary>
{
    public override void Configure(EntityTypeBuilder<Salary> builder)
    {
        base.Configure(builder);
        builder.ToTable("Salaries");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.UserId, x.Status });

        builder.Property(x => x.MemberName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Employer).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Role).HasMaxLength(200).IsRequired();
        builder.Property(x => x.MonthlyAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(4000);

        builder.HasMany(x => x.Payments)
            .WithOne(x => x.Salary)
            .HasForeignKey(x => x.SalaryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class SalaryPaymentConfiguration : AuditableEntityConfiguration<SalaryPayment>
{
    public override void Configure(EntityTypeBuilder<SalaryPayment> builder)
    {
        base.Configure(builder);
        builder.ToTable("SalaryPayments");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.UserId, x.Period });
        builder.HasIndex(x => x.SalaryId);

        builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Period).HasMaxLength(7).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(2000);
    }
}

public sealed class BusinessClientConfiguration : AuditableEntityConfiguration<BusinessClient>
{
    public override void Configure(EntityTypeBuilder<BusinessClient> builder)
    {
        base.Configure(builder);
        builder.ToTable("BusinessClients");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.UserId, x.Status });
        builder.HasIndex(x => new { x.UserId, x.Name });

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Engagement).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.MonthlyRevenue).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.ContactEmail).HasMaxLength(256);
        builder.Property(x => x.ContactPhone).HasMaxLength(32);
        builder.Property(x => x.Notes).HasMaxLength(4000);

        builder.HasMany(x => x.Projects)
            .WithOne(x => x.Client)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Invoices)
            .WithOne(x => x.Client)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class BusinessProjectConfiguration : AuditableEntityConfiguration<BusinessProject>
{
    public override void Configure(EntityTypeBuilder<BusinessProject> builder)
    {
        base.Configure(builder);
        builder.ToTable("BusinessProjects");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ClientId);
        builder.HasIndex(x => new { x.UserId, x.Status });

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(4000);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.MonthlyRevenue).HasPrecision(18, 2);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();

        builder.HasMany(x => x.Developers)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Invoices)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public sealed class ProjectDeveloperConfiguration : AuditableEntityConfiguration<ProjectDeveloper>
{
    public override void Configure(EntityTypeBuilder<ProjectDeveloper> builder)
    {
        base.Configure(builder);
        builder.ToTable("ProjectDevelopers");
        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.DeveloperId);
        builder.HasIndex(x => new { x.ProjectId, x.DeveloperId }).IsUnique();

        builder.Property(x => x.RoleOnProject).HasMaxLength(200);

        builder.HasOne(x => x.Developer)
            .WithMany(x => x.ProjectAssignments)
            .HasForeignKey(x => x.DeveloperId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class DeveloperConfiguration : AuditableEntityConfiguration<Developer>
{
    public override void Configure(EntityTypeBuilder<Developer> builder)
    {
        base.Configure(builder);
        builder.ToTable("Developers");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.PrimaryClientId);
        builder.HasIndex(x => new { x.UserId, x.IsActive });

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Role).HasMaxLength(200).IsRequired();
        builder.Property(x => x.MonthlySalary).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(4000);

        builder.HasOne(x => x.PrimaryClient)
            .WithMany()
            .HasForeignKey(x => x.PrimaryClientId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.PayrollRecords)
            .WithOne(x => x.Developer)
            .HasForeignKey(x => x.DeveloperId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class DeveloperPayrollConfiguration : AuditableEntityConfiguration<DeveloperPayroll>
{
    public override void Configure(EntityTypeBuilder<DeveloperPayroll> builder)
    {
        base.Configure(builder);
        builder.ToTable("DeveloperPayrolls");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.DeveloperId);
        builder.HasIndex(x => new { x.UserId, x.Period });
        builder.HasIndex(x => new { x.UserId, x.Status });

        builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Period).HasMaxLength(7).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(2000);
    }
}

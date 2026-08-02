using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Income.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Income.Configurations;

public sealed class InvoiceConfiguration : AuditableEntityConfiguration<Invoice>
{
    public override void Configure(EntityTypeBuilder<Invoice> builder)
    {
        base.Configure(builder);
        builder.ToTable("Invoices");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ClientId);
        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => new { x.UserId, x.Status });
        builder.HasIndex(x => new { x.UserId, x.InvoiceNumber }).IsUnique();

        builder.Property(x => x.InvoiceNumber).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.SubTotal).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.AmountPaid).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(4000);

        builder.Ignore(x => x.OutstandingAmount);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Invoice)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Payments)
            .WithOne(x => x.Invoice)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class InvoiceItemConfiguration : AuditableEntityConfiguration<InvoiceItem>
{
    public override void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        base.Configure(builder);
        builder.ToTable("InvoiceItems");
        builder.HasIndex(x => x.InvoiceId);

        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Quantity).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.LineTotal).HasPrecision(18, 2).IsRequired();
    }
}

public sealed class PaymentConfiguration : AuditableEntityConfiguration<Payment>
{
    public override void Configure(EntityTypeBuilder<Payment> builder)
    {
        base.Configure(builder);
        builder.ToTable("Payments");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.InvoiceId);
        builder.HasIndex(x => new { x.UserId, x.PaidOn });

        builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Method).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Reference).HasMaxLength(128);
        builder.Property(x => x.Notes).HasMaxLength(2000);
    }
}

public sealed class ExpenseCategoryConfiguration : AuditableEntityConfiguration<ExpenseCategory>
{
    public override void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        base.Configure(builder);
        builder.ToTable("ExpenseCategories");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.UserId, x.Name }).IsUnique();

        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);

        builder.HasMany(x => x.Expenses)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class BusinessExpenseConfiguration : AuditableEntityConfiguration<BusinessExpense>
{
    public override void Configure(EntityTypeBuilder<BusinessExpense> builder)
    {
        base.Configure(builder);
        builder.ToTable("BusinessExpenses");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => new { x.UserId, x.Period });

        builder.Property(x => x.Vendor).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.Period).HasMaxLength(7);
        builder.Property(x => x.Notes).HasMaxLength(2000);
    }
}

public sealed class IncomeSourceConfiguration : AuditableEntityConfiguration<IncomeSource>
{
    public override void Configure(EntityTypeBuilder<IncomeSource> builder)
    {
        base.Configure(builder);
        builder.ToTable("IncomeSources");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.UserId, x.Type });

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.MonthlyEstimate).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(2000);
    }
}

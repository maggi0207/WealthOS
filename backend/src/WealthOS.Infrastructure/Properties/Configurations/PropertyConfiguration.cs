using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Properties.Configurations;

public sealed class PropertyConfiguration : AuditableEntityConfiguration<Property>
{
    public override void Configure(EntityTypeBuilder<Property> builder)
    {
        base.Configure(builder);

        builder.ToTable("Properties");

        builder.Property(property => property.UserId)
            .IsRequired();

        builder.HasIndex(property => property.UserId);
        builder.HasIndex(property => new { property.UserId, property.Status });
        builder.HasIndex(property => new { property.UserId, property.Type });

        builder.Property(property => property.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(property => property.Type)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(property => property.OwnershipType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(property => property.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(property => property.PurchasePrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(property => property.CurrentMarketValue)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(property => property.CurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(property => property.Area)
            .HasPrecision(18, 2);

        builder.Property(property => property.BuiltUpArea)
            .HasPrecision(18, 2);

        builder.Property(property => property.Floor)
            .HasMaxLength(64);

        builder.Property(property => property.Facing)
            .HasMaxLength(64);

        builder.Property(property => property.Description)
            .HasMaxLength(4000);

        builder.Property(property => property.Notes)
            .HasMaxLength(4000);

        builder.HasOne(property => property.Address)
            .WithOne(address => address.Property)
            .HasForeignKey<PropertyAddress>(address => address.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(property => property.Owners)
            .WithOne(owner => owner.Property)
            .HasForeignKey(owner => owner.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(property => property.Valuations)
            .WithOne(valuation => valuation.Property)
            .HasForeignKey(valuation => valuation.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(property => property.LoanLinks)
            .WithOne(link => link.Property)
            .HasForeignKey(link => link.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(property => property.DocumentLinks)
            .WithOne(link => link.Property)
            .HasForeignKey(link => link.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(property => property.Images)
            .WithOne(image => image.Property)
            .HasForeignKey(image => image.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(property => property.PropertyNotes)
            .WithOne(note => note.Property)
            .HasForeignKey(note => note.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

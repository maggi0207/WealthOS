using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WealthOS.Domain.Authentication.Entities;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Domain.Properties.Enums;
using WealthOS.Infrastructure.Persistence;

namespace WealthOS.Infrastructure.Properties;

/// <summary>
/// Seeds sample property data aligned with the frontend Ramana Flats demo.
/// </summary>
public static class PropertyDataSeeder
{
    /// <summary>
    /// Stable id so re-runs remain idempotent.
    /// </summary>
    public static readonly Guid RamanaFlatsPropertyId =
        Guid.Parse("11111111-2222-3333-4444-555555555555");

    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("PropertyDataSeeder");
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        if (await dbContext.Properties.IgnoreQueryFilters()
                .AnyAsync(property => property.Id == RamanaFlatsPropertyId, cancellationToken))
        {
            logger.LogInformation("Sample property {PropertyId} already exists. Skipping seed.", RamanaFlatsPropertyId);
            return;
        }

        var adminUser = await userManager.Users
            .OrderBy(user => user.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (adminUser is null)
        {
            logger.LogWarning("No users found. Skipping property seed until identity seed completes.");
            return;
        }

        var property = new Property(RamanaFlatsPropertyId)
        {
            UserId = adminUser.Id,
            Name = "Ramana Flats",
            Type = PropertyType.Apartment,
            OwnershipType = OwnershipType.Sole,
            PurchaseDate = new DateOnly(2018, 3, 14),
            PurchasePrice = 9_600_000m,
            CurrentMarketValue = 15_000_000m,
            CurrencyCode = "INR",
            Area = 1250m,
            BuiltUpArea = 1100m,
            Floor = "3",
            Facing = "East",
            Bedrooms = 3,
            Bathrooms = 2,
            Parking = 1,
            Status = PropertyStatus.Active,
            Description =
                "Ramana Flats Door No.3, Anna Avenue, Adyar — residential apartment aligned with the WealthOS frontend demo.",
            Notes = "Seeded sample property for local development.",
            IsRentalEnabled = false,
            Address = new PropertyAddress
            {
                Line1 = "No.16, Ramana Flats, Door No.3",
                Line2 = "Anna Avenue",
                Locality = "Adyar",
                City = "Chennai",
                State = "Tamil Nadu",
                PostalCode = "600020",
                Country = "India",
                FullAddress = "No.16, Ramana Flats, Door No.3, Anna Avenue, Adyar, Chennai – 600020",
                Latitude = 13.0067m,
                Longitude = 80.2570m,
                GoogleMapsUrl = "https://maps.google.com/?q=13.0067,80.2570",
            },
            Owners =
            {
                new PropertyOwner
                {
                    Name = string.IsNullOrWhiteSpace(adminUser.DisplayName)
                        ? $"{adminUser.FirstName} {adminUser.LastName}".Trim()
                        : adminUser.DisplayName!,
                    OwnershipPercentage = 100m,
                    OwnershipType = OwnershipType.Sole,
                    IsPrimary = true,
                    LinkedUserId = adminUser.Id,
                },
            },
            Valuations =
            {
                new PropertyValuation
                {
                    ValuationDate = new DateOnly(2018, 3, 14),
                    Value = 9_600_000m,
                    CurrencyCode = "INR",
                    Source = "Purchase",
                    Notes = "Purchase price at acquisition.",
                },
                new PropertyValuation
                {
                    ValuationDate = new DateOnly(2026, 1, 1),
                    Value = 15_000_000m,
                    CurrencyCode = "INR",
                    Source = "Estimate",
                    Notes = "Current market estimate for demo.",
                },
            },
            PropertyNotes =
            {
                new PropertyNote
                {
                    Title = "Demo seed",
                    Body = "Sample property matching frontend Ramana Flats, Adyar passport data.",
                },
            },
        };

        await dbContext.Properties.AddAsync(property, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seeded sample property {PropertyName} ({PropertyId}) for user {UserId}",
            property.Name,
            property.Id,
            adminUser.Id);
    }
}

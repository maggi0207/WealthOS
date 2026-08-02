using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WealthOS.Domain.Authentication.Entities;
using WealthOS.Domain.Documents.Entities;
using WealthOS.Domain.Documents.Enums;
using WealthOS.Infrastructure.Loans;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Properties;

namespace WealthOS.Infrastructure.Documents;

/// <summary>
/// Seeds sample document metadata aligned with the frontend documents-data fixtures.
/// Storage paths are placeholders only — no real file I/O.
/// </summary>
public static class DocumentDataSeeder
{
    public static readonly Guid SaleDeedId =
        Guid.Parse("e1111111-1111-2222-3333-444444444401");

    public static readonly Guid EncumbranceId =
        Guid.Parse("e1111111-1111-2222-3333-444444444402");

    public static readonly Guid PattaId =
        Guid.Parse("e1111111-1111-2222-3333-444444444403");

    public static readonly Guid PropertyTaxId =
        Guid.Parse("e1111111-1111-2222-3333-444444444404");

    public static readonly Guid HomeLoanSanctionId =
        Guid.Parse("e1111111-1111-2222-3333-444444444405");

    public static readonly Guid LoanAmortisationId =
        Guid.Parse("e1111111-1111-2222-3333-444444444406");

    public static readonly Guid JewelPledgeId =
        Guid.Parse("e1111111-1111-2222-3333-444444444407");

    public static readonly Guid AngelHoldingId =
        Guid.Parse("e1111111-1111-2222-3333-444444444408");

    public static readonly Guid IndiaBondsAdviceId =
        Guid.Parse("e1111111-1111-2222-3333-444444444409");

    public static readonly Guid PanCardId =
        Guid.Parse("e1111111-1111-2222-3333-444444444410");

    public static readonly Guid AadhaarId =
        Guid.Parse("e1111111-1111-2222-3333-444444444411");

    public static readonly Guid PassportId =
        Guid.Parse("e1111111-1111-2222-3333-444444444412");

    public static readonly Guid TermLifeId =
        Guid.Parse("e1111111-1111-2222-3333-444444444413");

    public static readonly Guid FamilyHealthId =
        Guid.Parse("e1111111-1111-2222-3333-444444444414");

    public static readonly Guid CarInsuranceId =
        Guid.Parse("e1111111-1111-2222-3333-444444444415");

    public static readonly Guid ItrAckId =
        Guid.Parse("e1111111-1111-2222-3333-444444444416");

    public static readonly Guid Form16Id =
        Guid.Parse("e1111111-1111-2222-3333-444444444417");

    public static readonly Guid GstReturnsId =
        Guid.Parse("e1111111-1111-2222-3333-444444444418");

    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DocumentDataSeeder");
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        if (await dbContext.Documents.IgnoreQueryFilters()
                .AnyAsync(document => document.Id == SaleDeedId, cancellationToken))
        {
            logger.LogInformation("Sample documents already exist. Skipping seed.");
            return;
        }

        var adminUser = await userManager.Users
            .OrderBy(user => user.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (adminUser is null)
        {
            logger.LogWarning("No users found. Skipping document seed until identity seed completes.");
            return;
        }

        Guid? propertyId = null;
        if (await dbContext.Properties.AnyAsync(
                property => property.Id == PropertyDataSeeder.RamanaFlatsPropertyId,
                cancellationToken))
        {
            propertyId = PropertyDataSeeder.RamanaFlatsPropertyId;
        }

        Guid? homeLoanId = null;
        if (await dbContext.Loans.AnyAsync(
                loan => loan.Id == LoanDataSeeder.HomeLoanId,
                cancellationToken))
        {
            homeLoanId = LoanDataSeeder.HomeLoanId;
        }

        Guid? jewelLoanId = null;
        if (await dbContext.Loans.AnyAsync(
                loan => loan.Id == LoanDataSeeder.JewelLoanId,
                cancellationToken))
        {
            jewelLoanId = LoanDataSeeder.JewelLoanId;
        }

        var owner = $"{adminUser.FirstName} {adminUser.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(owner))
        {
            owner = adminUser.Email ?? "Owner";
        }

        var documents = BuildDocuments(adminUser.Id, owner, propertyId, homeLoanId, jewelLoanId);

        await dbContext.Documents.AddRangeAsync(documents, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded {Count} sample documents.", documents.Count);
    }

    private static List<Document> BuildDocuments(
        Guid userId,
        string owner,
        Guid? propertyId,
        Guid? homeLoanId,
        Guid? jewelLoanId)
    {
        return
        [
            Build(
                SaleDeedId,
                userId,
                owner,
                "Sale deed — Ramana Flats",
                DocumentCategory.Property,
                DocumentStatus.Verified,
                "application/pdf",
                "sale-deed-ramana-flats.pdf",
                4_400_000,
                new DateOnly(2026, 5, 14),
                null,
                propertyId,
                DocumentReferenceModule.Property,
                ["deed", "adyar"],
                "Ramana Flats, Door No. 3"),
            Build(
                EncumbranceId,
                userId,
                owner,
                "Encumbrance certificate",
                DocumentCategory.Property,
                DocumentStatus.Expiring,
                "application/pdf",
                "encumbrance-certificate.pdf",
                1_100_000,
                new DateOnly(2025, 9, 2),
                new DateOnly(2026, 9, 1),
                propertyId,
                DocumentReferenceModule.Property,
                ["ec", "registrar"],
                "Ramana Flats, Door No. 3",
                reminderDate: new DateOnly(2026, 8, 1)),
            Build(
                PattaId,
                userId,
                owner,
                "Patta / Chitta extract",
                DocumentCategory.Property,
                DocumentStatus.Verified,
                "application/pdf",
                "patta-chitta.pdf",
                820_000,
                new DateOnly(2026, 1, 22),
                null,
                propertyId,
                DocumentReferenceModule.Property,
                ["patta"],
                "Ramana Flats, Door No. 3"),
            Build(
                PropertyTaxId,
                userId,
                owner,
                "Property tax receipt 2026",
                DocumentCategory.Tax,
                DocumentStatus.Verified,
                "application/pdf",
                "property-tax-2026.pdf",
                310_000,
                new DateOnly(2026, 4, 8),
                null,
                propertyId,
                DocumentReferenceModule.Property,
                ["receipt", "gcc"]),
            Build(
                HomeLoanSanctionId,
                userId,
                owner,
                "Home loan sanction letter",
                DocumentCategory.Loan,
                DocumentStatus.Verified,
                "application/pdf",
                "hdfc-sanction-letter.pdf",
                1_600_000,
                new DateOnly(2018, 6, 8),
                null,
                homeLoanId,
                DocumentReferenceModule.Loan,
                ["hdfc", "sanction"],
                "HDFC •••• 4821"),
            Build(
                LoanAmortisationId,
                userId,
                owner,
                "Loan amortisation statement",
                DocumentCategory.Loan,
                DocumentStatus.Pending,
                "application/pdf",
                "loan-amortisation.pdf",
                640_000,
                new DateOnly(2026, 7, 6),
                null,
                homeLoanId,
                DocumentReferenceModule.Loan,
                ["statement"],
                "HDFC •••• 4821"),
            Build(
                JewelPledgeId,
                userId,
                owner,
                "Jewel loan pledge receipt",
                DocumentCategory.Loan,
                DocumentStatus.Verified,
                "image/jpeg",
                "jewel-pledge.jpg",
                2_000_000,
                new DateOnly(2024, 11, 18),
                null,
                jewelLoanId,
                DocumentReferenceModule.Loan,
                ["gold", "pledge"],
                "IOB •••• 7710"),
            Build(
                AngelHoldingId,
                userId,
                owner,
                "Angel One holding statement",
                DocumentCategory.Investment,
                DocumentStatus.Verified,
                "application/pdf",
                "angel-one-holding.pdf",
                980_000,
                new DateOnly(2026, 7, 1),
                null,
                null,
                DocumentReferenceModule.None,
                ["broker", "cas"],
                "Angel One (Magesh)"),
            Build(
                IndiaBondsAdviceId,
                userId,
                owner,
                "IndiaBonds allotment advice",
                DocumentCategory.Investment,
                DocumentStatus.Verified,
                "application/pdf",
                "indiabonds-allotment.pdf",
                410_000,
                new DateOnly(2026, 3, 19),
                null,
                null,
                DocumentReferenceModule.None,
                ["bond"]),
            Build(
                PanCardId,
                userId,
                owner,
                "PAN card",
                DocumentCategory.Identity,
                DocumentStatus.Verified,
                "image/jpeg",
                "pan-card.jpg",
                180_000,
                new DateOnly(2021, 2, 11),
                null,
                null,
                DocumentReferenceModule.None,
                ["kyc"]),
            Build(
                AadhaarId,
                userId,
                owner,
                "Aadhaar",
                DocumentCategory.Identity,
                DocumentStatus.Verified,
                "application/pdf",
                "aadhaar.pdf",
                260_000,
                new DateOnly(2023, 8, 4),
                null,
                null,
                DocumentReferenceModule.None,
                ["kyc"]),
            Build(
                PassportId,
                userId,
                owner,
                "Passport",
                DocumentCategory.Identity,
                DocumentStatus.Expiring,
                "application/pdf",
                "passport.pdf",
                1_200_000,
                new DateOnly(2016, 10, 30),
                new DateOnly(2026, 10, 29),
                null,
                DocumentReferenceModule.None,
                ["kyc", "travel"],
                reminderDate: new DateOnly(2026, 9, 29)),
            Build(
                TermLifeId,
                userId,
                owner,
                "Term life policy — ₹2 Cr",
                DocumentCategory.Insurance,
                DocumentStatus.Verified,
                "application/pdf",
                "term-life-policy.pdf",
                2_400_000,
                new DateOnly(2024, 12, 1),
                new DateOnly(2027, 1, 15),
                null,
                DocumentReferenceModule.None,
                ["term", "hdfc life"]),
            Build(
                FamilyHealthId,
                userId,
                owner,
                "Family health cover",
                DocumentCategory.Insurance,
                DocumentStatus.Expiring,
                "application/pdf",
                "family-health.pdf",
                1_800_000,
                new DateOnly(2025, 8, 20),
                new DateOnly(2026, 8, 19),
                null,
                DocumentReferenceModule.None,
                ["health", "star"],
                reminderDate: new DateOnly(2026, 7, 20)),
            Build(
                CarInsuranceId,
                userId,
                owner,
                "Car insurance",
                DocumentCategory.Insurance,
                DocumentStatus.Expired,
                "application/pdf",
                "car-insurance.pdf",
                760_000,
                new DateOnly(2025, 6, 10),
                new DateOnly(2026, 6, 9),
                null,
                DocumentReferenceModule.None,
                ["motor"]),
            Build(
                ItrAckId,
                userId,
                owner,
                "ITR acknowledgement AY 2025-26",
                DocumentCategory.Tax,
                DocumentStatus.Verified,
                "application/pdf",
                "itr-ack-ay2526.pdf",
                520_000,
                new DateOnly(2025, 7, 26),
                null,
                null,
                DocumentReferenceModule.None,
                ["itr"]),
            Build(
                Form16Id,
                userId,
                owner,
                "Form 16 — FY 2025-26",
                DocumentCategory.Tax,
                DocumentStatus.Pending,
                "application/pdf",
                "form-16-fy2526.pdf",
                340_000,
                new DateOnly(2026, 6, 12),
                null,
                null,
                DocumentReferenceModule.None,
                ["salary", "tds"]),
            Build(
                GstReturnsId,
                userId,
                owner,
                "GST returns — Q1 FY27",
                DocumentCategory.Tax,
                DocumentStatus.Verified,
                "application/pdf",
                "gst-q1-fy27.pdf",
                290_000,
                new DateOnly(2026, 7, 20),
                null,
                null,
                DocumentReferenceModule.None,
                ["gst", "business"],
                "Business"),
        ];
    }

    private static Document Build(
        Guid id,
        Guid userId,
        string owner,
        string title,
        DocumentCategory category,
        DocumentStatus status,
        string contentType,
        string fileName,
        long fileSizeBytes,
        DateOnly updatedOn,
        DateOnly? expiryDate,
        Guid? referenceId,
        DocumentReferenceModule referenceModule,
        IEnumerable<string> tags,
        string? notes = null,
        DateOnly? reminderDate = null)
    {
        var document = new Document(id)
        {
            UserId = userId,
            Title = title,
            Category = category,
            Owner = owner,
            Status = status,
            AccessLevel = DocumentAccess.Private,
            IssueDate = updatedOn,
            ExpiryDate = expiryDate,
            ReminderDate = reminderDate,
            ReferenceModule = referenceModule,
            ReferenceId = referenceId,
            Notes = notes,
            OriginalFileName = fileName,
            ContentType = contentType,
            FileSizeBytes = fileSizeBytes,
            StorageProvider = DocumentStorageProvider.LocalPlaceholder,
            StoragePath = $"placeholder://documents/{userId:N}/{id:N}/{fileName}",
            CreatedAt = updatedOn.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            UpdatedAt = updatedOn.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
        };

        foreach (var tag in tags)
        {
            document.Tags.Add(new DocumentTag { Name = tag });
        }

        document.Versions.Add(new DocumentVersion
        {
            VersionNumber = 1,
            OriginalFileName = fileName,
            ContentType = contentType,
            FileSizeBytes = fileSizeBytes,
            StorageProvider = DocumentStorageProvider.LocalPlaceholder,
            StoragePath = document.StoragePath,
            Notes = "Seeded metadata placeholder",
        });

        if (referenceModule != DocumentReferenceModule.None && referenceId.HasValue)
        {
            document.Links.Add(new DocumentLink
            {
                ReferenceModule = referenceModule,
                ReferenceId = referenceId.Value,
                Notes = notes,
            });
        }

        if (reminderDate.HasValue)
        {
            document.Reminders.Add(new DocumentReminder
            {
                ReminderDate = reminderDate.Value,
                Message = $"Renew or review: {title}",
                Notes = "Seeded renewal reminder",
            });
        }

        document.Metadata = new DocumentMetadata
        {
            IssuedBy = category switch
            {
                DocumentCategory.Property => "Sub-Registrar",
                DocumentCategory.Loan => "Lender",
                DocumentCategory.Identity => "UIDAI / Income Tax",
                DocumentCategory.Insurance => "Insurer",
                DocumentCategory.Tax => "Income Tax Department",
                _ => "Issuer",
            },
            DocumentNumber = $"SEED-{id.ToString("N")[..8].ToUpperInvariant()}",
        };

        return document;
    }
}

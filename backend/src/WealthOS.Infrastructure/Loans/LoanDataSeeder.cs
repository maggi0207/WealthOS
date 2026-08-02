using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WealthOS.Domain.Authentication.Entities;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Domain.Loans.Enums;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Properties;

namespace WealthOS.Infrastructure.Loans;

/// <summary>
/// Seeds sample loans aligned with the frontend home / jewel / personal demo.
/// </summary>
public static class LoanDataSeeder
{
    public static readonly Guid HomeLoanId =
        Guid.Parse("aaaaaaaa-1111-2222-3333-444444444401");

    public static readonly Guid JewelLoanId =
        Guid.Parse("aaaaaaaa-1111-2222-3333-444444444402");

    public static readonly Guid PersonalLoanId =
        Guid.Parse("aaaaaaaa-1111-2222-3333-444444444403");

    public static readonly Guid HdfcProviderId =
        Guid.Parse("bbbbbbbb-1111-2222-3333-444444444401");

    public static readonly Guid IobProviderId =
        Guid.Parse("bbbbbbbb-1111-2222-3333-444444444402");

    public static readonly Guid AxisProviderId =
        Guid.Parse("bbbbbbbb-1111-2222-3333-444444444403");

    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("LoanDataSeeder");
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        if (await dbContext.Loans.IgnoreQueryFilters()
                .AnyAsync(loan => loan.Id == HomeLoanId, cancellationToken))
        {
            logger.LogInformation("Sample loans already exist. Skipping seed.");
            return;
        }

        var adminUser = await userManager.Users
            .OrderBy(user => user.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (adminUser is null)
        {
            logger.LogWarning("No users found. Skipping loan seed until identity seed completes.");
            return;
        }

        Guid? propertyId = null;
        if (await dbContext.Properties.AnyAsync(
                property => property.Id == PropertyDataSeeder.RamanaFlatsPropertyId,
                cancellationToken))
        {
            propertyId = PropertyDataSeeder.RamanaFlatsPropertyId;
        }

        var providers = new[]
        {
            new LoanProvider(HdfcProviderId)
            {
                UserId = adminUser.Id,
                Name = "HDFC Bank",
                Code = "HDFC",
                IsActive = true,
            },
            new LoanProvider(IobProviderId)
            {
                UserId = adminUser.Id,
                Name = "Indian Overseas Bank",
                Code = "IOB",
                IsActive = true,
            },
            new LoanProvider(AxisProviderId)
            {
                UserId = adminUser.Id,
                Name = "Axis Bank",
                Code = "AXIS",
                IsActive = true,
            },
        };

        await dbContext.LoanProviders.AddRangeAsync(providers, cancellationToken);

        var homeLoan = new Loan(HomeLoanId)
        {
            UserId = adminUser.Id,
            Name = "Home loan — Ramana Flats",
            Type = LoanType.Home,
            LenderName = "HDFC Bank",
            LoanProviderId = HdfcProviderId,
            AccountNumber = "•••• 4821",
            Principal = 6_200_000m,
            OutstandingBalance = 3_845_000m,
            InterestRate = 8.6m,
            InterestType = InterestType.Floating,
            EmiAmount = 52_400m,
            TenureMonths = 180,
            RemainingTenureMonths = 82,
            StartDate = new DateOnly(2018, 6, 5),
            EndDate = new DateOnly(2033, 5, 5),
            NextEmiDate = new DateOnly(2026, 8, 5),
            PaymentFrequency = PaymentFrequency.Monthly,
            Status = LoanStatus.Active,
            LinkedPropertyId = propertyId,
            CurrencyCode = "INR",
            AutoDebit = true,
            Notes = "Seeded home loan aligned with frontend Ramana Flats demo.",
            InterestRates =
            {
                new LoanInterestRate
                {
                    RatePercent = 8.6m,
                    InterestType = InterestType.Floating,
                    EffectiveFrom = new DateOnly(2018, 6, 5),
                    Reason = "Initial rate",
                },
            },
            Reminders =
            {
                new LoanReminder
                {
                    Title = "Home loan EMI",
                    Detail = "Auto debit · HDFC Bank",
                    DueOn = new DateOnly(2026, 8, 5),
                    Amount = 52_400m,
                    IsUrgent = true,
                },
            },
            Payments =
            {
                new LoanPayment
                {
                    PaidOn = new DateOnly(2026, 7, 5),
                    Amount = 52_400m,
                    PrincipalComponent = 24_850m,
                    InterestComponent = 27_550m,
                    Status = LoanPaymentStatus.Paid,
                    PaymentMode = "Auto debit",
                },
                new LoanPayment
                {
                    PaidOn = new DateOnly(2026, 6, 5),
                    Amount = 52_400m,
                    PrincipalComponent = 24_670m,
                    InterestComponent = 27_730m,
                    Status = LoanPaymentStatus.Paid,
                    PaymentMode = "Auto debit",
                },
            },
        };

        if (propertyId.HasValue)
        {
            homeLoan.PropertyLinks.Add(new LoanPropertyLink
            {
                PropertyId = propertyId.Value,
                IsPrimary = true,
                Notes = "Primary residential collateral",
            });
        }

        var jewelLoan = new Loan(JewelLoanId)
        {
            UserId = adminUser.Id,
            Name = "Jewel loan — gold pledge",
            Type = LoanType.Jewel,
            LenderName = "Indian Overseas Bank",
            LoanProviderId = IobProviderId,
            AccountNumber = "•••• 7710",
            Principal = 650_000m,
            OutstandingBalance = 410_000m,
            InterestRate = 9.4m,
            InterestType = InterestType.Fixed,
            EmiAmount = 18_900m,
            TenureMonths = 30,
            RemainingTenureMonths = 22,
            StartDate = new DateOnly(2024, 11, 18),
            EndDate = new DateOnly(2027, 5, 18),
            NextEmiDate = new DateOnly(2026, 8, 18),
            PaymentFrequency = PaymentFrequency.Monthly,
            Status = LoanStatus.Active,
            CurrencyCode = "INR",
            AutoDebit = false,
            Notes = "Seeded jewel loan aligned with frontend demo.",
            InterestRates =
            {
                new LoanInterestRate
                {
                    RatePercent = 9.4m,
                    InterestType = InterestType.Fixed,
                    EffectiveFrom = new DateOnly(2024, 11, 18),
                    Reason = "Initial rate",
                },
            },
            Reminders =
            {
                new LoanReminder
                {
                    Title = "Jewel loan EMI",
                    Detail = "Manual · pay via UPI",
                    DueOn = new DateOnly(2026, 8, 18),
                    Amount = 18_900m,
                    IsUrgent = false,
                },
            },
            Payments =
            {
                new LoanPayment
                {
                    PaidOn = new DateOnly(2026, 7, 18),
                    Amount = 18_900m,
                    PrincipalComponent = 15_690m,
                    InterestComponent = 3_210m,
                    Status = LoanPaymentStatus.Paid,
                    PaymentMode = "UPI",
                },
                new LoanPayment
                {
                    PaidOn = new DateOnly(2026, 6, 18),
                    Amount = 18_900m,
                    PrincipalComponent = 15_540m,
                    InterestComponent = 3_360m,
                    Status = LoanPaymentStatus.Paid,
                    PaymentMode = "UPI",
                },
            },
        };

        var personalLoan = new Loan(PersonalLoanId)
        {
            UserId = adminUser.Id,
            Name = "Personal loan — renovation",
            Type = LoanType.Personal,
            LenderName = "Axis Bank",
            LoanProviderId = AxisProviderId,
            AccountNumber = "•••• 2043",
            Principal = 800_000m,
            OutstandingBalance = 292_000m,
            InterestRate = 13.2m,
            InterestType = InterestType.Fixed,
            EmiAmount = 21_600m,
            TenureMonths = 60,
            RemainingTenureMonths = 15,
            StartDate = new DateOnly(2023, 3, 12),
            EndDate = new DateOnly(2028, 2, 12),
            NextEmiDate = new DateOnly(2026, 8, 12),
            PaymentFrequency = PaymentFrequency.Monthly,
            Status = LoanStatus.Active,
            CurrencyCode = "INR",
            AutoDebit = true,
            Notes = "Seeded personal loan aligned with frontend demo.",
            InterestRates =
            {
                new LoanInterestRate
                {
                    RatePercent = 13.2m,
                    InterestType = InterestType.Fixed,
                    EffectiveFrom = new DateOnly(2023, 3, 12),
                    Reason = "Initial rate",
                },
            },
            Reminders =
            {
                new LoanReminder
                {
                    Title = "Personal loan EMI",
                    Detail = "Auto debit · Axis Bank",
                    DueOn = new DateOnly(2026, 8, 12),
                    Amount = 21_600m,
                    IsUrgent = false,
                },
            },
            Payments =
            {
                new LoanPayment
                {
                    PaidOn = new DateOnly(2026, 7, 12),
                    Amount = 21_600m,
                    PrincipalComponent = 18_400m,
                    InterestComponent = 3_200m,
                    Status = LoanPaymentStatus.Paid,
                    PaymentMode = "Auto debit",
                },
                new LoanPayment
                {
                    PaidOn = new DateOnly(2026, 6, 12),
                    Amount = 21_600m,
                    PrincipalComponent = 18_210m,
                    InterestComponent = 3_390m,
                    Status = LoanPaymentStatus.Failed,
                    PaymentMode = "Auto debit",
                    Notes = "Auto debit bounced — demo failed payment.",
                },
            },
        };

        await dbContext.Loans.AddRangeAsync([homeLoan, jewelLoan, personalLoan], cancellationToken);

        if (propertyId.HasValue)
        {
            var existingLink = await dbContext.PropertyLoanLinks
                .AnyAsync(
                    link => link.PropertyId == propertyId.Value && link.LoanId == HomeLoanId,
                    cancellationToken);

            if (!existingLink)
            {
                await dbContext.PropertyLoanLinks.AddAsync(
                    new PropertyLoanLink
                    {
                        PropertyId = propertyId.Value,
                        LoanId = HomeLoanId,
                        Notes = "Home loan linked to Ramana Flats",
                    },
                    cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seeded {Count} sample loans for user {UserId}",
            3,
            adminUser.Id);
    }
}

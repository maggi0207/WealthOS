using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WealthOS.Domain.Authentication.Entities;
using WealthOS.Domain.Goals.Entities;
using WealthOS.Domain.Goals.Enums;
using WealthOS.Infrastructure.Loans;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Properties;

namespace WealthOS.Infrastructure.Goals;

/// <summary>
/// Seeds sample goals aligned with the frontend goals-data fixtures.
/// </summary>
public static class GoalDataSeeder
{
    public static readonly Guid HouseGoalId =
        Guid.Parse("cccccccc-1111-2222-3333-444444444401");

    public static readonly Guid LoanFreeGoalId =
        Guid.Parse("cccccccc-1111-2222-3333-444444444402");

    public static readonly Guid EmergencyGoalId =
        Guid.Parse("cccccccc-1111-2222-3333-444444444403");

    public static readonly Guid EducationGoalId =
        Guid.Parse("cccccccc-1111-2222-3333-444444444404");

    public static readonly Guid RetirementGoalId =
        Guid.Parse("cccccccc-1111-2222-3333-444444444405");

    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("GoalDataSeeder");
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        if (await dbContext.FinancialGoals.IgnoreQueryFilters()
                .AnyAsync(goal => goal.Id == HouseGoalId, cancellationToken))
        {
            logger.LogInformation("Sample goals already exist. Skipping seed.");
            return;
        }

        var adminUser = await userManager.Users
            .OrderBy(user => user.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (adminUser is null)
        {
            logger.LogWarning("No users found. Skipping goal seed until identity seed completes.");
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

        var goals = new[]
        {
            BuildHouseGoal(adminUser.Id, propertyId),
            BuildLoanFreeGoal(adminUser.Id, homeLoanId),
            BuildEmergencyGoal(adminUser.Id),
            BuildEducationGoal(adminUser.Id),
            BuildRetirementGoal(adminUser.Id),
        };

        await dbContext.FinancialGoals.AddRangeAsync(goals, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded {Count} sample financial goals.", goals.Length);
    }

    private static FinancialGoal BuildHouseGoal(Guid userId, Guid? propertyId) =>
        new(HouseGoalId)
        {
            UserId = userId,
            Name = "Buy second house",
            Category = GoalCategory.BuyHouse,
            TargetAmount = 90_00_000m,
            CurrentAmount = 31_50_000m,
            MonthlyContribution = 85_000m,
            TargetDate = new DateOnly(2031, 4, 1),
            StartedOn = new DateOnly(2023, 4, 1),
            Priority = GoalPriority.High,
            Status = GoalStatus.Active,
            Description = "Down payment plus registration for a 2BHK in OMR corridor.",
            CurrencyCode = "INR",
            LinkedPropertyId = propertyId,
            Milestones =
            {
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444401"))
                {
                    Label = "Site shortlisted",
                    TargetPercent = 10m,
                    TargetAmount = 9_00_000m,
                    ReachedOn = new DateOnly(2023, 9, 12),
                    IsCompleted = true,
                    SortOrder = 0,
                },
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444402"))
                {
                    Label = "25% saved",
                    TargetPercent = 25m,
                    TargetAmount = 22_50_000m,
                    ReachedOn = new DateOnly(2025, 2, 4),
                    IsCompleted = true,
                    SortOrder = 1,
                },
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444403"))
                {
                    Label = "Half way",
                    TargetPercent = 50m,
                    TargetAmount = 45_00_000m,
                    SortOrder = 2,
                },
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444404"))
                {
                    Label = "Down payment ready",
                    TargetPercent = 100m,
                    TargetAmount = 90_00_000m,
                    SortOrder = 3,
                },
            },
        };

    private static FinancialGoal BuildLoanFreeGoal(Guid userId, Guid? loanId) =>
        new(LoanFreeGoalId)
        {
            UserId = userId,
            Name = "Loan free",
            Category = GoalCategory.BecomeLoanFree,
            TargetAmount = 45_47_000m,
            CurrentAmount = 40_40_000m,
            MonthlyContribution = 92_900m,
            TargetDate = new DateOnly(2033, 5, 5),
            StartedOn = new DateOnly(2018, 6, 5),
            Priority = GoalPriority.Critical,
            Status = GoalStatus.Active,
            Description = "Every EMI and prepayment counts toward clearing all three loans.",
            CurrencyCode = "INR",
            LinkedLoanId = loanId,
            Milestones =
            {
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444411"))
                {
                    Label = "Personal loan cleared",
                    TargetPercent = 60m,
                    TargetAmount = 27_28_200m,
                    ReachedOn = new DateOnly(2025, 11, 20),
                    IsCompleted = true,
                    SortOrder = 0,
                },
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444412"))
                {
                    Label = "Jewel loan cleared",
                    TargetPercent = 80m,
                    TargetAmount = 36_37_600m,
                    ReachedOn = new DateOnly(2026, 6, 18),
                    IsCompleted = true,
                    SortOrder = 1,
                },
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444413"))
                {
                    Label = "Home loan cleared",
                    TargetPercent = 100m,
                    TargetAmount = 45_47_000m,
                    SortOrder = 2,
                },
            },
        };

    private static FinancialGoal BuildEmergencyGoal(Guid userId) =>
        new(EmergencyGoalId)
        {
            UserId = userId,
            Name = "Emergency fund",
            Category = GoalCategory.EmergencyFund,
            TargetAmount = 12_00_000m,
            CurrentAmount = 12_00_000m,
            MonthlyContribution = 0m,
            TargetDate = new DateOnly(2026, 6, 30),
            StartedOn = new DateOnly(2022, 1, 10),
            Priority = GoalPriority.High,
            Status = GoalStatus.Completed,
            Description = "Twelve months of household and business runway in liquid funds.",
            CurrencyCode = "INR",
            Milestones =
            {
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444421"))
                {
                    Label = "3 months runway",
                    TargetPercent = 25m,
                    TargetAmount = 3_00_000m,
                    ReachedOn = new DateOnly(2022, 11, 2),
                    IsCompleted = true,
                    SortOrder = 0,
                },
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444422"))
                {
                    Label = "6 months runway",
                    TargetPercent = 50m,
                    TargetAmount = 6_00_000m,
                    ReachedOn = new DateOnly(2024, 3, 15),
                    IsCompleted = true,
                    SortOrder = 1,
                },
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444423"))
                {
                    Label = "12 months runway",
                    TargetPercent = 100m,
                    TargetAmount = 12_00_000m,
                    ReachedOn = new DateOnly(2026, 6, 28),
                    IsCompleted = true,
                    SortOrder = 2,
                },
            },
        };

    private static FinancialGoal BuildEducationGoal(Guid userId) =>
        new(EducationGoalId)
        {
            UserId = userId,
            Name = "Daughter's education",
            Category = GoalCategory.ChildEducation,
            TargetAmount = 65_00_000m,
            CurrentAmount = 18_20_000m,
            MonthlyContribution = 45_000m,
            TargetDate = new DateOnly(2038, 6, 1),
            StartedOn = new DateOnly(2021, 7, 1),
            Priority = GoalPriority.High,
            Status = GoalStatus.Active,
            Description = "Undergraduate abroad corpus, indexed at 8% education inflation.",
            CurrencyCode = "INR",
            Milestones =
            {
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444431"))
                {
                    Label = "First ₹10 L",
                    TargetPercent = 15m,
                    TargetAmount = 9_75_000m,
                    ReachedOn = new DateOnly(2024, 8, 19),
                    IsCompleted = true,
                    SortOrder = 0,
                },
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444432"))
                {
                    Label = "Quarter funded",
                    TargetPercent = 25m,
                    TargetAmount = 16_25_000m,
                    SortOrder = 1,
                },
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444433"))
                {
                    Label = "Half funded",
                    TargetPercent = 50m,
                    TargetAmount = 32_50_000m,
                    SortOrder = 2,
                },
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444434"))
                {
                    Label = "Fully funded",
                    TargetPercent = 100m,
                    TargetAmount = 65_00_000m,
                    SortOrder = 3,
                },
            },
        };

    private static FinancialGoal BuildRetirementGoal(Guid userId) =>
        new(RetirementGoalId)
        {
            UserId = userId,
            Name = "Retirement corpus",
            Category = GoalCategory.Retirement,
            TargetAmount = 6_00_00_000m,
            CurrentAmount = 1_42_00_000m,
            MonthlyContribution = 1_10_000m,
            TargetDate = new DateOnly(2045, 3, 31),
            StartedOn = new DateOnly(2016, 4, 1),
            Priority = GoalPriority.Medium,
            Status = GoalStatus.Active,
            Description = "Target corpus for a ₹2 L monthly lifestyle from age 58.",
            CurrencyCode = "INR",
            Milestones =
            {
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444441"))
                {
                    Label = "First crore",
                    TargetPercent = 16m,
                    TargetAmount = 96_00_000m,
                    ReachedOn = new DateOnly(2025, 5, 30),
                    IsCompleted = true,
                    SortOrder = 0,
                },
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444442"))
                {
                    Label = "Quarter corpus",
                    TargetPercent = 25m,
                    TargetAmount = 1_50_00_000m,
                    SortOrder = 1,
                },
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444443"))
                {
                    Label = "Half corpus",
                    TargetPercent = 50m,
                    TargetAmount = 3_00_00_000m,
                    SortOrder = 2,
                },
                new GoalMilestone(Guid.Parse("dddddddd-1111-2222-3333-444444444444"))
                {
                    Label = "Financially free",
                    TargetPercent = 100m,
                    TargetAmount = 6_00_00_000m,
                    SortOrder = 3,
                },
            },
        };
}

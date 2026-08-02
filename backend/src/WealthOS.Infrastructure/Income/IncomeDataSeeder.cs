using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WealthOS.Domain.Authentication.Entities;
using WealthOS.Domain.Income.Entities;
using WealthOS.Domain.Income.Enums;
using WealthOS.Infrastructure.Persistence;

namespace WealthOS.Infrastructure.Income;

/// <summary>
/// Seeds Income &amp; Business sample data aligned with frontend <c>business-data.ts</c> themes.
/// </summary>
public static class IncomeDataSeeder
{
    public static readonly Guid ClientNorthbridgeId = Guid.Parse("cccccccc-1111-2222-3333-444444444401");
    public static readonly Guid ClientLumenId = Guid.Parse("cccccccc-1111-2222-3333-444444444402");
    public static readonly Guid ClientArkaId = Guid.Parse("cccccccc-1111-2222-3333-444444444403");
    public static readonly Guid ClientVetriId = Guid.Parse("cccccccc-1111-2222-3333-444444444404");

    public static readonly Guid ProjectNorthbridgeWebId = Guid.Parse("dddddddd-1111-2222-3333-444444444401");
    public static readonly Guid ProjectLumenMobileId = Guid.Parse("dddddddd-1111-2222-3333-444444444402");
    public static readonly Guid ProjectArkaDashboardId = Guid.Parse("dddddddd-1111-2222-3333-444444444403");

    public static readonly Guid DevArunId = Guid.Parse("eeeeeeee-1111-2222-3333-444444444401");
    public static readonly Guid DevDivyaId = Guid.Parse("eeeeeeee-1111-2222-3333-444444444402");
    public static readonly Guid DevKarthikId = Guid.Parse("eeeeeeee-1111-2222-3333-444444444403");
    public static readonly Guid DevSnehaId = Guid.Parse("eeeeeeee-1111-2222-3333-444444444404");

    public static readonly Guid SalaryMageshId = Guid.Parse("ffffffff-1111-2222-3333-444444444401");
    public static readonly Guid SalaryWifeId = Guid.Parse("ffffffff-1111-2222-3333-444444444402");

    public static readonly Guid CategoryCloudId = Guid.Parse("99999999-1111-2222-3333-444444444401");
    public static readonly Guid CategoryToolsId = Guid.Parse("99999999-1111-2222-3333-444444444402");
    public static readonly Guid CategoryOfficeId = Guid.Parse("99999999-1111-2222-3333-444444444403");
    public static readonly Guid CategoryFeesId = Guid.Parse("99999999-1111-2222-3333-444444444404");
    public static readonly Guid CategoryTravelId = Guid.Parse("99999999-1111-2222-3333-444444444405");

    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("IncomeDataSeeder");
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        if (await dbContext.BusinessClients.IgnoreQueryFilters()
                .AnyAsync(client => client.Id == ClientNorthbridgeId, cancellationToken))
        {
            logger.LogInformation("Sample income/business data already exists. Skipping seed.");
            return;
        }

        var adminUser = await userManager.Users
            .OrderBy(user => user.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (adminUser is null)
        {
            logger.LogWarning("No users found. Skipping income seed until identity seed completes.");
            return;
        }

        var userId = adminUser.Id;
        const string period = "2026-07";

        var clients = new[]
        {
            new BusinessClient(ClientNorthbridgeId)
            {
                UserId = userId,
                Name = "Northbridge Retail",
                Engagement = "Retainer · Web platform",
                Status = ClientStatus.Active,
                MonthlyRevenue = 275_000m,
            },
            new BusinessClient(ClientLumenId)
            {
                UserId = userId,
                Name = "Lumen Health",
                Engagement = "Retainer · Mobile app",
                Status = ClientStatus.Active,
                MonthlyRevenue = 210_000m,
            },
            new BusinessClient(ClientArkaId)
            {
                UserId = userId,
                Name = "Arka Logistics",
                Engagement = "Time & material · Dashboard",
                Status = ClientStatus.Active,
                MonthlyRevenue = 155_000m,
            },
            new BusinessClient(ClientVetriId)
            {
                UserId = userId,
                Name = "Vetri Motors",
                Engagement = "Support · Paused since May",
                Status = ClientStatus.Paused,
                MonthlyRevenue = 0m,
            },
        };

        var projects = new[]
        {
            new BusinessProject(ProjectNorthbridgeWebId)
            {
                UserId = userId,
                ClientId = ClientNorthbridgeId,
                Name = "Northbridge Web Platform",
                Status = ProjectStatus.Active,
                StartDate = new DateOnly(2025, 10, 1),
                MonthlyRevenue = 275_000m,
            },
            new BusinessProject(ProjectLumenMobileId)
            {
                UserId = userId,
                ClientId = ClientLumenId,
                Name = "Lumen Mobile App",
                Status = ProjectStatus.Active,
                StartDate = new DateOnly(2025, 11, 15),
                MonthlyRevenue = 210_000m,
            },
            new BusinessProject(ProjectArkaDashboardId)
            {
                UserId = userId,
                ClientId = ClientArkaId,
                Name = "Arka Ops Dashboard",
                Status = ProjectStatus.Active,
                StartDate = new DateOnly(2026, 1, 8),
                MonthlyRevenue = 155_000m,
            },
        };

        var developers = new[]
        {
            new Developer(DevArunId)
            {
                UserId = userId,
                Name = "Arun Prakash",
                Role = "Full-stack developer",
                MonthlySalary = 95_000m,
                PrimaryClientId = ClientNorthbridgeId,
            },
            new Developer(DevDivyaId)
            {
                UserId = userId,
                Name = "Divya R",
                Role = "React Native developer",
                MonthlySalary = 85_000m,
                PrimaryClientId = ClientLumenId,
            },
            new Developer(DevKarthikId)
            {
                UserId = userId,
                Name = "Karthik S",
                Role = "Backend developer",
                MonthlySalary = 78_000m,
                PrimaryClientId = ClientArkaId,
            },
            new Developer(DevSnehaId)
            {
                UserId = userId,
                Name = "Sneha M",
                Role = "QA engineer",
                MonthlySalary = 52_000m,
                PrimaryClientId = ClientNorthbridgeId,
            },
        };

        var assignments = new[]
        {
            new ProjectDeveloper
            {
                ProjectId = ProjectNorthbridgeWebId,
                DeveloperId = DevArunId,
                AssignedOn = new DateOnly(2025, 10, 1),
                RoleOnProject = "Full-stack",
            },
            new ProjectDeveloper
            {
                ProjectId = ProjectNorthbridgeWebId,
                DeveloperId = DevSnehaId,
                AssignedOn = new DateOnly(2025, 10, 15),
                RoleOnProject = "QA",
            },
            new ProjectDeveloper
            {
                ProjectId = ProjectLumenMobileId,
                DeveloperId = DevDivyaId,
                AssignedOn = new DateOnly(2025, 11, 15),
                RoleOnProject = "Mobile",
            },
            new ProjectDeveloper
            {
                ProjectId = ProjectArkaDashboardId,
                DeveloperId = DevKarthikId,
                AssignedOn = new DateOnly(2026, 1, 8),
                RoleOnProject = "Backend",
            },
        };

        var payroll = new[]
        {
            new DeveloperPayroll
            {
                DeveloperId = DevArunId,
                UserId = userId,
                Amount = 95_000m,
                Period = period,
                Status = PayrollStatus.Paid,
                PaidOn = new DateOnly(2026, 7, 5),
                ScheduledOn = new DateOnly(2026, 8, 5),
            },
            new DeveloperPayroll
            {
                DeveloperId = DevDivyaId,
                UserId = userId,
                Amount = 85_000m,
                Period = period,
                Status = PayrollStatus.Paid,
                PaidOn = new DateOnly(2026, 7, 5),
                ScheduledOn = new DateOnly(2026, 8, 5),
            },
            new DeveloperPayroll
            {
                DeveloperId = DevKarthikId,
                UserId = userId,
                Amount = 78_000m,
                Period = period,
                Status = PayrollStatus.Pending,
                ScheduledOn = new DateOnly(2026, 8, 2),
            },
            new DeveloperPayroll
            {
                DeveloperId = DevSnehaId,
                UserId = userId,
                Amount = 52_000m,
                Period = period,
                Status = PayrollStatus.Scheduled,
                ScheduledOn = new DateOnly(2026, 8, 5),
            },
        };

        var salaries = new[]
        {
            new Salary(SalaryMageshId)
            {
                UserId = userId,
                MemberName = "Magesh",
                Employer = "Zoho Corporation",
                Role = "Engineering Manager",
                MonthlyAmount = 245_000m,
                LastCreditedOn = new DateOnly(2026, 7, 31),
                NextExpectedOn = new DateOnly(2026, 8, 31),
                Status = SalaryStatus.Active,
            },
            new Salary(SalaryWifeId)
            {
                UserId = userId,
                MemberName = "Wife",
                Employer = "Freshworks",
                Role = "Senior Analyst",
                MonthlyAmount = 140_000m,
                LastCreditedOn = new DateOnly(2026, 7, 30),
                NextExpectedOn = new DateOnly(2026, 8, 30),
                Status = SalaryStatus.Active,
            },
        };

        var salaryPayments = new[]
        {
            new SalaryPayment
            {
                SalaryId = SalaryMageshId,
                UserId = userId,
                Amount = 245_000m,
                PaidOn = new DateOnly(2026, 7, 31),
                Period = period,
            },
            new SalaryPayment
            {
                SalaryId = SalaryWifeId,
                UserId = userId,
                Amount = 140_000m,
                PaidOn = new DateOnly(2026, 7, 30),
                Period = period,
            },
        };

        var categories = new[]
        {
            new ExpenseCategory(CategoryCloudId) { UserId = userId, Name = "Cloud & hosting", IsSystem = true },
            new ExpenseCategory(CategoryToolsId) { UserId = userId, Name = "Software tools", IsSystem = true },
            new ExpenseCategory(CategoryOfficeId) { UserId = userId, Name = "Co-working", IsSystem = true },
            new ExpenseCategory(CategoryFeesId) { UserId = userId, Name = "Professional fees", IsSystem = true },
            new ExpenseCategory(CategoryTravelId) { UserId = userId, Name = "Travel & misc", IsSystem = true },
        };

        var expenses = new[]
        {
            new BusinessExpense
            {
                UserId = userId,
                CategoryId = CategoryCloudId,
                Vendor = "AWS",
                Amount = 28_400m,
                PaidOn = new DateOnly(2026, 7, 3),
                IsRecurring = true,
                Period = period,
            },
            new BusinessExpense
            {
                UserId = userId,
                CategoryId = CategoryToolsId,
                Vendor = "Figma, GitHub, Slack",
                Amount = 16_200m,
                PaidOn = new DateOnly(2026, 7, 4),
                IsRecurring = true,
                Period = period,
            },
            new BusinessExpense
            {
                UserId = userId,
                CategoryId = CategoryOfficeId,
                Vendor = "IndiQube Adyar",
                Amount = 22_000m,
                PaidOn = new DateOnly(2026, 7, 1),
                IsRecurring = true,
                Period = period,
            },
            new BusinessExpense
            {
                UserId = userId,
                CategoryId = CategoryFeesId,
                Vendor = "CA & compliance",
                Amount = 8_500m,
                PaidOn = new DateOnly(2026, 7, 14),
                IsRecurring = false,
                Period = period,
            },
            new BusinessExpense
            {
                UserId = userId,
                CategoryId = CategoryTravelId,
                Vendor = "Client visits",
                Amount = 3_400m,
                PaidOn = new DateOnly(2026, 7, 19),
                IsRecurring = false,
                Period = period,
            },
        };

        var invoiceNorthbridge = new Invoice(Guid.Parse("aaaa1111-1111-2222-3333-444444444401"))
        {
            UserId = userId,
            ClientId = ClientNorthbridgeId,
            ProjectId = ProjectNorthbridgeWebId,
            InvoiceNumber = "INV-2026-0701",
            IssueDate = new DateOnly(2026, 7, 1),
            DueDate = new DateOnly(2026, 7, 15),
            Status = InvoiceStatus.Sent,
            SubTotal = 275_000m,
            AmountPaid = 0m,
            Items =
            {
                new InvoiceItem
                {
                    Description = "July retainer · Web platform",
                    Quantity = 1m,
                    UnitPrice = 275_000m,
                    LineTotal = 275_000m,
                },
            },
        };

        var invoiceLumen = new Invoice(Guid.Parse("aaaa1111-1111-2222-3333-444444444402"))
        {
            UserId = userId,
            ClientId = ClientLumenId,
            ProjectId = ProjectLumenMobileId,
            InvoiceNumber = "INV-2026-0702",
            IssueDate = new DateOnly(2026, 7, 1),
            DueDate = new DateOnly(2026, 7, 10),
            Status = InvoiceStatus.Paid,
            SubTotal = 210_000m,
            AmountPaid = 210_000m,
            Items =
            {
                new InvoiceItem
                {
                    Description = "July retainer · Mobile app",
                    Quantity = 1m,
                    UnitPrice = 210_000m,
                    LineTotal = 210_000m,
                },
            },
            Payments =
            {
                new Payment
                {
                    UserId = userId,
                    Amount = 210_000m,
                    PaidOn = new DateOnly(2026, 7, 5),
                    Method = PaymentMethod.BankTransfer,
                    Reference = "LUMEN-JUL",
                },
            },
        };

        var invoiceArka = new Invoice(Guid.Parse("aaaa1111-1111-2222-3333-444444444403"))
        {
            UserId = userId,
            ClientId = ClientArkaId,
            ProjectId = ProjectArkaDashboardId,
            InvoiceNumber = "INV-2026-0703",
            IssueDate = new DateOnly(2026, 7, 1),
            DueDate = new DateOnly(2026, 7, 20),
            Status = InvoiceStatus.PartiallyPaid,
            SubTotal = 155_000m,
            AmountPaid = 77_000m,
            Items =
            {
                new InvoiceItem
                {
                    Description = "July T&M · Dashboard",
                    Quantity = 1m,
                    UnitPrice = 155_000m,
                    LineTotal = 155_000m,
                },
            },
            Payments =
            {
                new Payment
                {
                    UserId = userId,
                    Amount = 77_000m,
                    PaidOn = new DateOnly(2026, 6, 28),
                    Method = PaymentMethod.Upi,
                    Reference = "ARKA-JUN",
                },
            },
        };

        var invoiceVetri = new Invoice(Guid.Parse("aaaa1111-1111-2222-3333-444444444404"))
        {
            UserId = userId,
            ClientId = ClientVetriId,
            InvoiceNumber = "INV-2026-0501",
            IssueDate = new DateOnly(2026, 5, 1),
            DueDate = new DateOnly(2026, 5, 20),
            Status = InvoiceStatus.Overdue,
            SubTotal = 45_000m,
            AmountPaid = 0m,
            Items =
            {
                new InvoiceItem
                {
                    Description = "Support backlog",
                    Quantity = 1m,
                    UnitPrice = 45_000m,
                    LineTotal = 45_000m,
                },
            },
        };

        // Additional June/July payments so business revenue ~640k for July period
        // Lumen 210k (Jul 5) + Northbridge prior payment recorded as Jul inflow:
        var invoiceNorthbridgePrior = new Invoice(Guid.Parse("aaaa1111-1111-2222-3333-444444444405"))
        {
            UserId = userId,
            ClientId = ClientNorthbridgeId,
            ProjectId = ProjectNorthbridgeWebId,
            InvoiceNumber = "INV-2026-0601",
            IssueDate = new DateOnly(2026, 6, 1),
            DueDate = new DateOnly(2026, 6, 15),
            Status = InvoiceStatus.Paid,
            SubTotal = 275_000m,
            AmountPaid = 275_000m,
            Items =
            {
                new InvoiceItem
                {
                    Description = "June retainer · Web platform",
                    Quantity = 1m,
                    UnitPrice = 275_000m,
                    LineTotal = 275_000m,
                },
            },
            Payments =
            {
                new Payment
                {
                    UserId = userId,
                    Amount = 275_000m,
                    PaidOn = new DateOnly(2026, 7, 8),
                    Method = PaymentMethod.BankTransfer,
                    Reference = "NB-JUN",
                },
            },
        };

        var invoiceArkaExtra = new Invoice(Guid.Parse("aaaa1111-1111-2222-3333-444444444406"))
        {
            UserId = userId,
            ClientId = ClientArkaId,
            ProjectId = ProjectArkaDashboardId,
            InvoiceNumber = "INV-2026-0602",
            IssueDate = new DateOnly(2026, 6, 1),
            DueDate = new DateOnly(2026, 6, 20),
            Status = InvoiceStatus.Paid,
            SubTotal = 155_000m,
            AmountPaid = 155_000m,
            Items =
            {
                new InvoiceItem
                {
                    Description = "June T&M · Dashboard",
                    Quantity = 1m,
                    UnitPrice = 155_000m,
                    LineTotal = 155_000m,
                },
            },
            Payments =
            {
                new Payment
                {
                    UserId = userId,
                    Amount = 155_000m,
                    PaidOn = new DateOnly(2026, 7, 12),
                    Method = PaymentMethod.BankTransfer,
                    Reference = "ARKA-JUL-CLEAR",
                },
            },
        };

        var sources = new[]
        {
            new IncomeSource
            {
                UserId = userId,
                Name = "Household salaries",
                Type = IncomeSourceType.Salary,
                MonthlyEstimate = 385_000m,
                LinkedEntityId = SalaryMageshId,
            },
            new IncomeSource
            {
                UserId = userId,
                Name = "Client retainers",
                Type = IncomeSourceType.Business,
                MonthlyEstimate = 640_000m,
            },
        };

        await dbContext.BusinessClients.AddRangeAsync(clients, cancellationToken);
        await dbContext.BusinessProjects.AddRangeAsync(projects, cancellationToken);
        await dbContext.Developers.AddRangeAsync(developers, cancellationToken);
        await dbContext.ProjectDevelopers.AddRangeAsync(assignments, cancellationToken);
        await dbContext.DeveloperPayrolls.AddRangeAsync(payroll, cancellationToken);
        await dbContext.Salaries.AddRangeAsync(salaries, cancellationToken);
        await dbContext.SalaryPayments.AddRangeAsync(salaryPayments, cancellationToken);
        await dbContext.ExpenseCategories.AddRangeAsync(categories, cancellationToken);
        await dbContext.BusinessExpenses.AddRangeAsync(expenses, cancellationToken);
        await dbContext.Invoices.AddRangeAsync(
            [invoiceNorthbridge, invoiceLumen, invoiceArka, invoiceVetri, invoiceNorthbridgePrior, invoiceArkaExtra],
            cancellationToken);
        await dbContext.IncomeSources.AddRangeAsync(sources, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded Income & Business sample data for user {UserId}.", userId);
    }
}

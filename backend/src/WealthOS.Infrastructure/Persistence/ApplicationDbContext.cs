using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Authentication.Entities;
using WealthOS.Domain.Documents.Entities;
using WealthOS.Domain.Goals.Entities;
using WealthOS.Domain.Income.Entities;
using WealthOS.Domain.Investments.Entities;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Domain.Properties.Entities;

namespace WealthOS.Infrastructure.Persistence;

public sealed class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<Property> Properties => Set<Property>();

    public DbSet<PropertyAddress> PropertyAddresses => Set<PropertyAddress>();

    public DbSet<PropertyOwner> PropertyOwners => Set<PropertyOwner>();

    public DbSet<PropertyValuation> PropertyValuations => Set<PropertyValuation>();

    public DbSet<PropertyLoanLink> PropertyLoanLinks => Set<PropertyLoanLink>();

    public DbSet<PropertyDocumentLink> PropertyDocumentLinks => Set<PropertyDocumentLink>();

    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();

    public DbSet<PropertyNote> PropertyNotes => Set<PropertyNote>();

    public DbSet<Loan> Loans => Set<Loan>();

    public DbSet<LoanProvider> LoanProviders => Set<LoanProvider>();

    public DbSet<LoanPayment> LoanPayments => Set<LoanPayment>();

    public DbSet<LoanSchedule> LoanSchedules => Set<LoanSchedule>();

    public DbSet<LoanReminder> LoanReminders => Set<LoanReminder>();

    public DbSet<LoanInterestRate> LoanInterestRates => Set<LoanInterestRate>();

    public DbSet<LoanDocumentLink> LoanDocumentLinks => Set<LoanDocumentLink>();

    public DbSet<LoanPropertyLink> LoanPropertyLinks => Set<LoanPropertyLink>();

    public DbSet<Salary> Salaries => Set<Salary>();

    public DbSet<SalaryPayment> SalaryPayments => Set<SalaryPayment>();

    public DbSet<BusinessClient> BusinessClients => Set<BusinessClient>();

    public DbSet<BusinessProject> BusinessProjects => Set<BusinessProject>();

    public DbSet<ProjectDeveloper> ProjectDevelopers => Set<ProjectDeveloper>();

    public DbSet<Developer> Developers => Set<Developer>();

    public DbSet<DeveloperPayroll> DeveloperPayrolls => Set<DeveloperPayroll>();

    public DbSet<Invoice> Invoices => Set<Invoice>();

    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();

    public DbSet<BusinessExpense> BusinessExpenses => Set<BusinessExpense>();

    public DbSet<IncomeSource> IncomeSources => Set<IncomeSource>();

    public DbSet<InvestmentProvider> InvestmentProviders => Set<InvestmentProvider>();

    public DbSet<InvestmentAccount> InvestmentAccounts => Set<InvestmentAccount>();

    public DbSet<Holding> Holdings => Set<Holding>();

    public DbSet<InvestmentTransaction> InvestmentTransactions => Set<InvestmentTransaction>();

    public DbSet<PortfolioSnapshot> PortfolioSnapshots => Set<PortfolioSnapshot>();

    public DbSet<Dividend> Dividends => Set<Dividend>();

    public DbSet<CorporateAction> CorporateActions => Set<CorporateAction>();

    public DbSet<WatchlistItem> WatchlistItems => Set<WatchlistItem>();

    public DbSet<FinancialGoal> FinancialGoals => Set<FinancialGoal>();

    public DbSet<GoalContribution> GoalContributions => Set<GoalContribution>();

    public DbSet<GoalMilestone> GoalMilestones => Set<GoalMilestone>();

    public DbSet<Document> Documents => Set<Document>();

    public DbSet<DocumentMetadata> DocumentMetadata => Set<DocumentMetadata>();

    public DbSet<DocumentTag> DocumentTags => Set<DocumentTag>();

    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();

    public DbSet<DocumentLink> DocumentLinks => Set<DocumentLink>();

    public DbSet<DocumentReminder> DocumentReminders => Set<DocumentReminder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

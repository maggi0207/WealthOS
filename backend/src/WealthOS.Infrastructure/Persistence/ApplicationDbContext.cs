using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Authentication.Entities;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

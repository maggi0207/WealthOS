using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Authentication;
using WealthOS.Application.Authentication.Interfaces;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Investments.Providers;
using WealthOS.Domain.Authentication.Entities;
using WealthOS.Domain.Authentication.Repositories;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Documents.Repositories;
using WealthOS.Domain.Goals.Repositories;
using WealthOS.Domain.Income.Repositories;
using WealthOS.Domain.Investments.Repositories;
using WealthOS.Domain.Loans.Repositories;
using WealthOS.Domain.Notifications.Repositories;
using WealthOS.Domain.Properties.Repositories;
using WealthOS.Infrastructure.AI;
using WealthOS.Infrastructure.Authentication.Services;
using WealthOS.Infrastructure.BackgroundJobs;
using WealthOS.Infrastructure.BackgroundJobs.Jobs;
using WealthOS.Infrastructure.Dashboard;
using WealthOS.Infrastructure.Documents.Repositories;
using WealthOS.Infrastructure.Identity;
using WealthOS.Infrastructure.Goals.Repositories;
using WealthOS.Infrastructure.Income.Repositories;
using WealthOS.Infrastructure.Investments.Providers;
using WealthOS.Infrastructure.Investments.Repositories;
using WealthOS.Infrastructure.Loans.Repositories;
using WealthOS.Infrastructure.Notifications.Repositories;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Persistence.Interceptors;
using WealthOS.Infrastructure.Persistence.Repositories;
using WealthOS.Infrastructure.Properties.Repositories;
using WealthOS.Infrastructure.Reports;

namespace WealthOS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                npgsql.EnableRetryOnFailure(maxRetryCount: 3);
            });

            options.AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>());
        });

        services
            .AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<ILoanProviderRepository, LoanProviderRepository>();
        services.AddScoped<ISalaryRepository, SalaryRepository>();
        services.AddScoped<IBusinessClientRepository, BusinessClientRepository>();
        services.AddScoped<IBusinessProjectRepository, BusinessProjectRepository>();
        services.AddScoped<IDeveloperRepository, DeveloperRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IBusinessExpenseRepository, BusinessExpenseRepository>();
        services.AddScoped<IIncomeSourceRepository, IncomeSourceRepository>();

        services.AddScoped<IInvestmentProviderRepository, InvestmentProviderRepository>();
        services.AddScoped<IInvestmentAccountRepository, InvestmentAccountRepository>();
        services.AddScoped<IHoldingRepository, HoldingRepository>();
        services.AddScoped<IInvestmentTransactionRepository, InvestmentTransactionRepository>();
        services.AddScoped<IPortfolioSnapshotRepository, PortfolioSnapshotRepository>();
        services.AddScoped<IWatchlistRepository, WatchlistRepository>();

        services.AddScoped<IFinancialGoalRepository, FinancialGoalRepository>();
        services.AddScoped<IGoalContributionRepository, GoalContributionRepository>();
        services.AddScoped<IGoalMilestoneRepository, GoalMilestoneRepository>();

        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IDocumentTagRepository, DocumentTagRepository>();
        services.AddScoped<IDocumentReminderRepository, DocumentReminderRepository>();

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();
        services.AddScoped<IReminderRepository, ReminderRepository>();
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
        services.AddScoped<IBackgroundJobLogRepository, BackgroundJobLogRepository>();
        services.AddScoped<INotificationScheduleRepository, NotificationScheduleRepository>();

        services.AddScoped<IInvestmentProvider, ManualInvestmentProvider>();
        services.Configure<AngelOneOptions>(configuration.GetSection(AngelOneOptions.SectionName));
        services.AddSingleton<AngelOneTokenStore>();
        services.AddHttpClient<AngelOneSmartApiClient>();
        services.AddScoped<IInvestmentProvider, AngelOneProvider>();
        services.AddScoped<IInvestmentProvider, IndiaBondsProvider>();
        services.AddScoped<AngelOneSyncJob>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddAuthenticationOptions(configuration);
        services.AddDashboardInfrastructure();
        services.AddAIInfrastructure();
        services.AddReportsInfrastructure();
        services.AddHangfireBackgroundJobs(configuration);

        return services;
    }
}

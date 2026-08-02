using System.Security.Claims;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WealthOS.Application.Notifications.Interfaces;
using WealthOS.Domain.Authentication.Constants;
using WealthOS.Infrastructure.BackgroundJobs.Jobs;

namespace WealthOS.Infrastructure.BackgroundJobs;

/// <summary>
/// Registers Hangfire with PostgreSQL storage, stub jobs, and dashboard access policy.
/// </summary>
public static class HangfireServiceCollectionExtensions
{
    public const string DashboardPath = "/hangfire";

    public static IServiceCollection AddHangfireBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(
                options => options.UseNpgsqlConnection(connectionString),
                new PostgreSqlStorageOptions
                {
                    PrepareSchemaIfNecessary = true,
                    InvisibilityTimeout = TimeSpan.FromMinutes(30),
                }));

        var configuredWorkers = configuration.GetValue<int?>("Hangfire:WorkerCount");
        var workerCount = configuredWorkers is > 0
            ? configuredWorkers.Value
            : Math.Max(1, Environment.ProcessorCount / 2);

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = workerCount;
            options.Queues = ["default"];
            options.SchedulePollingInterval = TimeSpan.FromSeconds(30);
        });

        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
        {
            Attempts = 3,
            DelaysInSeconds = [60, 300, 900],
            OnAttemptsExceeded = AttemptsExceededAction.Fail,
        });

        services.AddScoped<BackgroundJobLogWriter>();
        services.AddScoped<NotificationStubJobs>();
        services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();

        return services;
    }

    /// <summary>
    /// Maps the Hangfire dashboard. In Development it is open to local requests;
    /// in non-Development environments it requires an authenticated user.
    /// Dashboard URL: <c>/hangfire</c>
    /// </summary>
    public static IApplicationBuilder UseHangfireDashboardSecure(this IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

        app.UseHangfireDashboard(DashboardPath, new DashboardOptions
        {
            DashboardTitle = "WealthOS Background Jobs",
            Authorization = [new HangfireDashboardAuthorizationFilter(env.IsDevelopment())],
            DisplayStorageConnectionString = false,
        });

        return app;
    }

    /// <summary>
    /// Registers recurring stub jobs once the host is ready.
    /// </summary>
    public static IApplicationBuilder RegisterHangfireRecurringJobs(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var jobService = scope.ServiceProvider.GetRequiredService<IBackgroundJobService>();
        jobService.RegisterRecurringJobs();
        return app;
    }
}

/// <summary>
/// Development: allow all. Production: require authenticated Admin role (JWT Bearer).
/// </summary>
internal sealed class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly bool _allowAnonymousInDevelopment;

    public HangfireDashboardAuthorizationFilter(bool allowAnonymousInDevelopment)
    {
        _allowAnonymousInDevelopment = allowAnonymousInDevelopment;
    }

    public bool Authorize(DashboardContext context)
    {
        if (_allowAnonymousInDevelopment)
        {
            return true;
        }

        var httpContext = context.GetHttpContext();
        var user = httpContext.User;
        if (user.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        return user.IsInRole(AuthRoles.Admin)
            || user.HasClaim(ClaimTypes.Role, AuthRoles.Admin);
    }
}

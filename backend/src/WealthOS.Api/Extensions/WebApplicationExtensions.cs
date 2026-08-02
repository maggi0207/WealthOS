using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WealthOS.Api.Middleware;
using WealthOS.Infrastructure.AI;
using WealthOS.Infrastructure.BackgroundJobs;
using WealthOS.Infrastructure.Documents;
using WealthOS.Infrastructure.Goals;
using WealthOS.Infrastructure.Identity;
using WealthOS.Infrastructure.Income;
using WealthOS.Infrastructure.Investments;
using WealthOS.Infrastructure.Loans;
using WealthOS.Infrastructure.Notifications;
using WealthOS.Infrastructure.Persistence;
using WealthOS.Infrastructure.Properties;
using WealthOS.Infrastructure.Reports;
using HealthChecks.UI.Client;

namespace WealthOS.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "WealthOS API v1");
                options.RoutePrefix = "swagger";
            });
        }

        app.UseResponseCompression();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseHttpsRedirection();
        app.UseCors("WealthOsCors");
        app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();

        // Hangfire dashboard: Development = open; Production = Admin JWT only.
        // URL: /hangfire
        app.UseHangfireDashboardSecure();
        app.RegisterHangfireRecurringJobs();

        app.MapControllers();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        }).AllowAnonymous();

        return app;
    }

    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();
        await IdentityDataSeeder.SeedAsync(app.Services);
        await PropertyDataSeeder.SeedAsync(app.Services);
        await LoanDataSeeder.SeedAsync(app.Services);
        await IncomeDataSeeder.SeedAsync(app.Services);
        await InvestmentDataSeeder.SeedAsync(app.Services);
        await GoalDataSeeder.SeedAsync(app.Services);
        await DocumentDataSeeder.SeedAsync(app.Services);
        await NotificationDataSeeder.SeedAsync(app.Services);
        await AIDataSeeder.SeedAsync(app.Services);
        await ReportsDataSeeder.SeedAsync(app.Services);
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WealthOS.Domain.Notifications.Entities;
using WealthOS.Domain.Notifications.Enums;
using WealthOS.Infrastructure.Persistence;

namespace WealthOS.Infrastructure.Notifications;

/// <summary>
/// Seeds default in-app notification templates (framework placeholders).
/// </summary>
public static class NotificationDataSeeder
{
    public static readonly Guid GeneralInAppTemplateId =
        Guid.Parse("a1000001-0000-4000-8000-000000000001");

    public static readonly Guid SalaryReminderTemplateId =
        Guid.Parse("a1000001-0000-4000-8000-000000000002");

    public static readonly Guid DocumentExpiryTemplateId =
        Guid.Parse("a1000001-0000-4000-8000-000000000003");

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(NotificationDataSeeder));

        if (dbContext.NotificationTemplates.Any())
        {
            return;
        }

        dbContext.NotificationTemplates.AddRange(
            new NotificationTemplate(GeneralInAppTemplateId)
            {
                Code = "general-inapp",
                Name = "General In-App",
                Type = NotificationType.GeneralReminder,
                Channel = NotificationChannel.InApp,
                SubjectTemplate = "{{title}}",
                BodyTemplate = "{{message}}",
                IsActive = true,
            },
            new NotificationTemplate(SalaryReminderTemplateId)
            {
                Code = "salary-reminder-inapp",
                Name = "Salary Reminder In-App",
                Type = NotificationType.SalaryReminder,
                Channel = NotificationChannel.InApp,
                SubjectTemplate = "Salary reminder",
                BodyTemplate = "Your salary reminder: {{message}}",
                IsActive = true,
            },
            new NotificationTemplate(DocumentExpiryTemplateId)
            {
                Code = "document-expiry-inapp",
                Name = "Document Expiry In-App",
                Type = NotificationType.DocumentExpiry,
                Channel = NotificationChannel.InApp,
                SubjectTemplate = "Document expiry",
                BodyTemplate = "Document expiry reminder: {{message}}",
                IsActive = true,
            });

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} notification templates", 3);
    }
}

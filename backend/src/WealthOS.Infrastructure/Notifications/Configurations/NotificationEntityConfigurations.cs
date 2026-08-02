using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Notifications.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Notifications.Configurations;

public sealed class NotificationConfiguration : AuditableEntityConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        base.Configure(builder);

        builder.ToTable("Notifications");

        builder.HasIndex(notification => notification.UserId);
        builder.HasIndex(notification => new { notification.UserId, notification.Status });
        builder.HasIndex(notification => new { notification.UserId, notification.Type });
        builder.HasIndex(notification => new { notification.UserId, notification.CreatedAt });
        builder.HasIndex(notification => new { notification.ReferenceModule, notification.ReferenceId });

        builder.Property(notification => notification.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(notification => notification.Message)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(notification => notification.Type)
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(notification => notification.Channel)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(notification => notification.Priority)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(notification => notification.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(notification => notification.ReferenceModule)
            .HasMaxLength(64);

        builder.Property(notification => notification.PayloadJson)
            .HasMaxLength(8000);

        builder.HasOne(notification => notification.Template)
            .WithMany()
            .HasForeignKey(notification => notification.TemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(notification => notification.Recipients)
            .WithOne(recipient => recipient.Notification)
            .HasForeignKey(recipient => recipient.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class NotificationPreferenceConfiguration
    : AuditableEntityConfiguration<NotificationPreference>
{
    public override void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        base.Configure(builder);

        builder.ToTable("NotificationPreferences");

        builder.HasIndex(preference => new { preference.UserId, preference.NotificationType })
            .IsUnique();

        builder.Property(preference => preference.NotificationType)
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();
    }
}

public sealed class NotificationTemplateConfiguration
    : AuditableEntityConfiguration<NotificationTemplate>
{
    public override void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        base.Configure(builder);

        builder.ToTable("NotificationTemplates");

        builder.HasIndex(template => template.Code).IsUnique();
        builder.HasIndex(template => new { template.Type, template.Channel });

        builder.Property(template => template.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(template => template.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(template => template.Type)
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(template => template.Channel)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(template => template.SubjectTemplate)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(template => template.BodyTemplate)
            .HasMaxLength(8000)
            .IsRequired();
    }
}

public sealed class NotificationRecipientConfiguration
    : AuditableEntityConfiguration<NotificationRecipient>
{
    public override void Configure(EntityTypeBuilder<NotificationRecipient> builder)
    {
        base.Configure(builder);

        builder.ToTable("NotificationRecipients");

        builder.HasIndex(recipient => recipient.NotificationId);
        builder.HasIndex(recipient => new { recipient.UserId, recipient.DeliveryStatus });

        builder.Property(recipient => recipient.Channel)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(recipient => recipient.Address)
            .HasMaxLength(320);

        builder.Property(recipient => recipient.DeliveryStatus)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(recipient => recipient.FailureReason)
            .HasMaxLength(1000);
    }
}

public sealed class NotificationScheduleConfiguration
    : AuditableEntityConfiguration<NotificationSchedule>
{
    public override void Configure(EntityTypeBuilder<NotificationSchedule> builder)
    {
        base.Configure(builder);

        builder.ToTable("NotificationSchedules");

        builder.HasIndex(schedule => schedule.UserId);
        builder.HasIndex(schedule => new { schedule.IsEnabled, schedule.NextRunAt });

        builder.Property(schedule => schedule.NotificationType)
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(schedule => schedule.CronExpression)
            .HasMaxLength(100);

        builder.Property(schedule => schedule.PayloadJson)
            .HasMaxLength(8000);

        builder.Property(schedule => schedule.Notes)
            .HasMaxLength(1000);
    }
}

public sealed class ReminderConfiguration : AuditableEntityConfiguration<Reminder>
{
    public override void Configure(EntityTypeBuilder<Reminder> builder)
    {
        base.Configure(builder);

        builder.ToTable("Reminders");

        builder.HasIndex(reminder => reminder.UserId);
        builder.HasIndex(reminder => new { reminder.UserId, reminder.Status });
        builder.HasIndex(reminder => new { reminder.UserId, reminder.DueAt });
        builder.HasIndex(reminder => new { reminder.ReferenceModule, reminder.ReferenceId });

        builder.Property(reminder => reminder.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(reminder => reminder.Message)
            .HasMaxLength(4000);

        builder.Property(reminder => reminder.ReminderType)
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(reminder => reminder.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(reminder => reminder.ReferenceModule)
            .HasMaxLength(64);

        builder.Property(reminder => reminder.RecurrenceRule)
            .HasMaxLength(256);
    }
}

public sealed class BackgroundJobLogConfiguration : AuditableEntityConfiguration<BackgroundJobLog>
{
    public override void Configure(EntityTypeBuilder<BackgroundJobLog> builder)
    {
        base.Configure(builder);

        builder.ToTable("BackgroundJobLogs");

        builder.HasIndex(log => log.JobName);
        builder.HasIndex(log => log.StartedAt);
        builder.HasIndex(log => new { log.JobName, log.Status });

        builder.Property(log => log.JobName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(log => log.HangfireJobId)
            .HasMaxLength(100);

        builder.Property(log => log.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(log => log.Message)
            .HasMaxLength(2000);

        builder.Property(log => log.ErrorDetails)
            .HasMaxLength(8000);
    }
}

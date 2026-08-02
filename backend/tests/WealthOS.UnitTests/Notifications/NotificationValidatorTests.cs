using FluentAssertions;
using FluentValidation.TestHelper;
using WealthOS.Application.Notifications.DTOs.Requests;
using WealthOS.Application.Notifications.Validators;
using WealthOS.Domain.Notifications.Enums;

namespace WealthOS.UnitTests.Notifications;

/// <summary>
/// Validator coverage for notification request DTOs.
/// </summary>
public sealed class NotificationValidatorTests
{
    private readonly CreateNotificationRequestValidator _createValidator = new();
    private readonly CreateReminderRequestValidator _reminderValidator = new();
    private readonly UpdateNotificationPreferencesRequestValidator _preferencesValidator = new();

    [Fact]
    public void CreateNotification_WhenTitleEmpty_ShouldFail()
    {
        var result = _createValidator.TestValidate(new CreateNotificationRequest
        {
            Title = "",
            Message = "Hello",
            Type = NotificationType.GeneralReminder,
        });

        result.ShouldHaveValidationErrorFor(request => request.Title);
    }

    [Fact]
    public void CreateNotification_WhenValid_ShouldPass()
    {
        var result = _createValidator.TestValidate(new CreateNotificationRequest
        {
            Title = "Salary",
            Message = "Salary credited reminder",
            Type = NotificationType.SalaryReminder,
            Channel = NotificationChannel.InApp,
            Priority = NotificationPriority.Normal,
        });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateReminder_WhenDueAtMissing_ShouldFail()
    {
        var result = _reminderValidator.TestValidate(new CreateReminderRequest
        {
            Title = "Pay rent",
            DueAt = default,
        });

        result.ShouldHaveValidationErrorFor(request => request.DueAt);
    }

    [Fact]
    public void UpdatePreferences_WhenEmpty_ShouldFail()
    {
        var result = _preferencesValidator.TestValidate(new UpdateNotificationPreferencesRequest
        {
            Preferences = Array.Empty<NotificationPreferenceItemRequest>(),
        });

        result.ShouldHaveValidationErrorFor(request => request.Preferences);
    }
}

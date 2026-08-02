using FluentValidation;
using WealthOS.Application.Notifications.DTOs.Requests;
using WealthOS.Application.Notifications.Queries;

namespace WealthOS.Application.Notifications.Validators;

public sealed class CreateNotificationRequestValidator : AbstractValidator<CreateNotificationRequest>
{
    public CreateNotificationRequestValidator()
    {
        RuleFor(request => request.Title).NotEmpty().MaximumLength(300);
        RuleFor(request => request.Message).NotEmpty().MaximumLength(4000);
        RuleFor(request => request.Type).IsInEnum();
        RuleFor(request => request.Channel).IsInEnum();
        RuleFor(request => request.Priority).IsInEnum();
        RuleFor(request => request.ReferenceModule).MaximumLength(64);
        RuleFor(request => request.PayloadJson).MaximumLength(8000);

        RuleFor(request => request.ReferenceId)
            .NotEmpty()
            .When(request => !string.IsNullOrWhiteSpace(request.ReferenceModule));
    }
}

public sealed class CreateReminderRequestValidator : AbstractValidator<CreateReminderRequest>
{
    public CreateReminderRequestValidator()
    {
        RuleFor(request => request.Title).NotEmpty().MaximumLength(300);
        RuleFor(request => request.Message).MaximumLength(4000);
        RuleFor(request => request.ReminderType).IsInEnum();
        RuleFor(request => request.DueAt).NotEmpty();
        RuleFor(request => request.ReferenceModule).MaximumLength(64);
        RuleFor(request => request.RecurrenceRule).MaximumLength(256);

        RuleFor(request => request.ReferenceId)
            .NotEmpty()
            .When(request => !string.IsNullOrWhiteSpace(request.ReferenceModule));
    }
}

public sealed class UpdateNotificationPreferencesRequestValidator
    : AbstractValidator<UpdateNotificationPreferencesRequest>
{
    public UpdateNotificationPreferencesRequestValidator()
    {
        RuleFor(request => request.Preferences).NotNull().NotEmpty();
        RuleForEach(request => request.Preferences).ChildRules(item =>
        {
            item.RuleFor(preference => preference.NotificationType).IsInEnum();
            item.RuleFor(preference => preference.QuietHoursStartMinutes)
                .InclusiveBetween(0, 1439)
                .When(preference => preference.QuietHoursStartMinutes.HasValue);
            item.RuleFor(preference => preference.QuietHoursEndMinutes)
                .InclusiveBetween(0, 1439)
                .When(preference => preference.QuietHoursEndMinutes.HasValue);
        });
    }
}

public sealed class GetNotificationsQueryValidator : AbstractValidator<GetNotificationsQuery>
{
    public GetNotificationsQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.Type).IsInEnum().When(query => query.Type.HasValue);
        RuleFor(query => query.Status).IsInEnum().When(query => query.Status.HasValue);
        RuleFor(query => query.Search).MaximumLength(200);
    }
}

public sealed class GetUnreadNotificationsQueryValidator : AbstractValidator<GetUnreadNotificationsQuery>
{
    public GetUnreadNotificationsQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class GetRemindersQueryValidator : AbstractValidator<GetRemindersQuery>
{
    public GetRemindersQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.Status).IsInEnum().When(query => query.Status.HasValue);
    }
}

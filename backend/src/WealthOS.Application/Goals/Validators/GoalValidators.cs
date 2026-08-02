using FluentValidation;
using WealthOS.Application.Goals.DTOs.Requests;
using WealthOS.Application.Goals.Queries;

namespace WealthOS.Application.Goals.Validators;

public sealed class CreateGoalRequestValidator : AbstractValidator<CreateGoalRequest>
{
    public CreateGoalRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Category).IsInEnum();
        RuleFor(request => request.Priority).IsInEnum();
        RuleFor(request => request.Status).IsInEnum();

        RuleFor(request => request.TargetAmount).GreaterThan(0);
        RuleFor(request => request.CurrentAmount).GreaterThanOrEqualTo(0);
        RuleFor(request => request.CurrentAmount)
            .LessThanOrEqualTo(request => request.TargetAmount)
            .WithMessage("Current amount cannot exceed target amount.");

        RuleFor(request => request.MonthlyContribution).GreaterThanOrEqualTo(0);

        RuleFor(request => request.Description).MaximumLength(4000);

        RuleFor(request => request.CurrencyCode)
            .NotEmpty()
            .Length(3);

        RuleFor(request => request.TargetDate)
            .GreaterThanOrEqualTo(request => request.StartedOn)
            .WithMessage("Target date must be on or after the start date.");

        RuleForEach(request => request.Milestones)
            .ChildRules(milestone =>
            {
                milestone.RuleFor(m => m.Label).NotEmpty().MaximumLength(200);
                milestone.RuleFor(m => m.TargetPercent).InclusiveBetween(0m, 100m);
                milestone.RuleFor(m => m.TargetAmount).GreaterThanOrEqualTo(0).When(m => m.TargetAmount.HasValue);
                milestone.RuleFor(m => m.SortOrder).GreaterThanOrEqualTo(0);
            })
            .When(request => request.Milestones is not null);
    }
}

public sealed class UpdateGoalRequestValidator : AbstractValidator<UpdateGoalRequest>
{
    public UpdateGoalRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Category).IsInEnum();
        RuleFor(request => request.Priority).IsInEnum();
        RuleFor(request => request.Status).IsInEnum();

        RuleFor(request => request.TargetAmount).GreaterThan(0);
        RuleFor(request => request.CurrentAmount).GreaterThanOrEqualTo(0);
        RuleFor(request => request.CurrentAmount)
            .LessThanOrEqualTo(request => request.TargetAmount)
            .WithMessage("Current amount cannot exceed target amount.");

        RuleFor(request => request.MonthlyContribution).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Description).MaximumLength(4000);

        RuleFor(request => request.CurrencyCode)
            .NotEmpty()
            .Length(3);

        RuleFor(request => request.TargetDate)
            .GreaterThanOrEqualTo(request => request.StartedOn)
            .WithMessage("Target date must be on or after the start date.");
    }
}

public sealed class RecordGoalContributionRequestValidator : AbstractValidator<RecordGoalContributionRequest>
{
    public RecordGoalContributionRequestValidator()
    {
        RuleFor(request => request.Amount).GreaterThan(0);
        RuleFor(request => request.Notes).MaximumLength(1000);
        RuleFor(request => request.Source).MaximumLength(128);
    }
}

public sealed class CompleteMilestoneRequestValidator : AbstractValidator<CompleteMilestoneRequest>
{
    public CompleteMilestoneRequestValidator()
    {
        // ReachedOn is optional; when omitted the service uses UTC today.
    }
}

public sealed class GetGoalsQueryValidator : AbstractValidator<GetGoalsQuery>
{
    public GetGoalsQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.Status).IsInEnum().When(query => query.Status.HasValue);
        RuleFor(query => query.Category).IsInEnum().When(query => query.Category.HasValue);
        RuleFor(query => query.Priority).IsInEnum().When(query => query.Priority.HasValue);
        RuleFor(query => query.Search).MaximumLength(200);
    }
}

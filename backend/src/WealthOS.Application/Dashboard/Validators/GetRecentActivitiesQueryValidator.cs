using FluentValidation;
using WealthOS.Application.Dashboard.Queries;

namespace WealthOS.Application.Dashboard.Validators;

/// <summary>
/// Validates <see cref="GetRecentActivitiesQuery"/> bounds.
/// </summary>
public sealed class GetRecentActivitiesQueryValidator : AbstractValidator<GetRecentActivitiesQuery>
{
    public GetRecentActivitiesQueryValidator()
    {
        RuleFor(query => query.Limit)
            .InclusiveBetween(1, 50)
            .WithMessage("Limit must be between 1 and 50.");
    }
}

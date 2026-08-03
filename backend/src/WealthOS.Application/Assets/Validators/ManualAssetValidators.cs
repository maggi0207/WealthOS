using FluentValidation;
using WealthOS.Application.Assets.DTOs.Requests;

namespace WealthOS.Application.Assets.Validators;

public sealed class CreateManualAssetRequestValidator : AbstractValidator<CreateManualAssetRequest>
{
    public CreateManualAssetRequestValidator()
    {
        RuleFor(request => request.Type).IsInEnum();

        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.PurchaseValue).GreaterThanOrEqualTo(0);
        RuleFor(request => request.CurrentValue).GreaterThanOrEqualTo(0);

        RuleFor(request => request.Quantity)
            .GreaterThan(0)
            .When(request => request.Quantity.HasValue);

        RuleFor(request => request.Institution).MaximumLength(200);
        RuleFor(request => request.Notes).MaximumLength(4000);

        RuleFor(request => request.CurrencyCode)
            .NotEmpty()
            .Length(3);
    }
}

public sealed class UpdateManualAssetRequestValidator : AbstractValidator<UpdateManualAssetRequest>
{
    public UpdateManualAssetRequestValidator()
    {
        RuleFor(request => request.Type).IsInEnum();

        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.PurchaseValue).GreaterThanOrEqualTo(0);
        RuleFor(request => request.CurrentValue).GreaterThanOrEqualTo(0);

        RuleFor(request => request.Quantity)
            .GreaterThan(0)
            .When(request => request.Quantity.HasValue);

        RuleFor(request => request.Institution).MaximumLength(200);
        RuleFor(request => request.Notes).MaximumLength(4000);

        RuleFor(request => request.CurrencyCode)
            .NotEmpty()
            .Length(3);
    }
}

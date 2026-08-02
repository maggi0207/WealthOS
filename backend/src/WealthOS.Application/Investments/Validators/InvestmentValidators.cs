using FluentValidation;
using WealthOS.Application.Investments.DTOs.Requests;

namespace WealthOS.Application.Investments.Validators;

public sealed class CreateInvestmentAccountRequestValidator : AbstractValidator<CreateInvestmentAccountRequest>
{
    public CreateInvestmentAccountRequestValidator()
    {
        RuleFor(x => x.ProviderId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OwnerName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.KindLabel).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
        RuleFor(x => x.Notes).MaximumLength(4000);
        RuleFor(x => x.ExternalAccountReference).MaximumLength(200);
    }
}

public sealed class UpdateInvestmentAccountRequestValidator : AbstractValidator<UpdateInvestmentAccountRequest>
{
    public UpdateInvestmentAccountRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OwnerName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.KindLabel).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
        RuleFor(x => x.Notes).MaximumLength(4000);
        RuleFor(x => x.ExternalAccountReference).MaximumLength(200);
    }
}

public sealed class AddManualHoldingRequestValidator : AbstractValidator<AddManualHoldingRequest>
{
    public AddManualHoldingRequestValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Symbol).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.InvestmentType).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AverageCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.InvestedAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrentPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrentValue).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
        RuleFor(x => x.Notes).MaximumLength(4000);
    }
}

public sealed class UpdateHoldingRequestValidator : AbstractValidator<UpdateHoldingRequest>
{
    public UpdateHoldingRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Symbol).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.InvestmentType).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AverageCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.InvestedAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrentPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrentValue).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
        RuleFor(x => x.Notes).MaximumLength(4000);
    }
}

public sealed class RecordTransactionRequestValidator : AbstractValidator<RecordTransactionRequest>
{
    public RecordTransactionRequestValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.TransactionType).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Fees).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
        RuleFor(x => x.Notes).MaximumLength(2000);
        RuleFor(x => x.ExternalReference).MaximumLength(200);
    }
}

public sealed class ProviderConnectRequestValidator : AbstractValidator<ProviderConnectRequest>
{
    public ProviderConnectRequestValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty();
    }
}

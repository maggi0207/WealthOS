using FluentValidation;
using WealthOS.Application.Loans.DTOs.Requests;
using WealthOS.Application.Loans.Queries;

namespace WealthOS.Application.Loans.Validators;

public sealed class CreateLoanRequestValidator : AbstractValidator<CreateLoanRequest>
{
    public CreateLoanRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Type).IsInEnum();
        RuleFor(request => request.InterestType).IsInEnum();
        RuleFor(request => request.PaymentFrequency).IsInEnum();
        RuleFor(request => request.Status).IsInEnum();

        RuleFor(request => request.LenderName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.AccountNumber).MaximumLength(64);
        RuleFor(request => request.Notes).MaximumLength(4000);

        RuleFor(request => request.Principal).GreaterThan(0);
        RuleFor(request => request.OutstandingBalance).GreaterThanOrEqualTo(0);
        RuleFor(request => request.OutstandingBalance)
            .LessThanOrEqualTo(request => request.Principal)
            .WithMessage("Outstanding balance cannot exceed principal.");

        RuleFor(request => request.InterestRate).InclusiveBetween(0m, 100m);
        RuleFor(request => request.EmiAmount).GreaterThanOrEqualTo(0);

        RuleFor(request => request.TenureMonths).InclusiveBetween(1, 600);
        RuleFor(request => request.RemainingTenureMonths)
            .InclusiveBetween(0, 600)
            .LessThanOrEqualTo(request => request.TenureMonths);

        RuleFor(request => request.CurrencyCode)
            .NotEmpty()
            .Length(3);

        RuleFor(request => request.EndDate)
            .GreaterThanOrEqualTo(request => request.StartDate)
            .When(request => request.EndDate.HasValue);
    }
}

public sealed class UpdateLoanRequestValidator : AbstractValidator<UpdateLoanRequest>
{
    public UpdateLoanRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Type).IsInEnum();
        RuleFor(request => request.InterestType).IsInEnum();
        RuleFor(request => request.PaymentFrequency).IsInEnum();
        RuleFor(request => request.Status).IsInEnum();

        RuleFor(request => request.LenderName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.AccountNumber).MaximumLength(64);
        RuleFor(request => request.Notes).MaximumLength(4000);

        RuleFor(request => request.Principal).GreaterThan(0);
        RuleFor(request => request.OutstandingBalance).GreaterThanOrEqualTo(0);
        RuleFor(request => request.OutstandingBalance)
            .LessThanOrEqualTo(request => request.Principal)
            .WithMessage("Outstanding balance cannot exceed principal.");

        RuleFor(request => request.InterestRate).InclusiveBetween(0m, 100m);
        RuleFor(request => request.EmiAmount).GreaterThanOrEqualTo(0);

        RuleFor(request => request.TenureMonths).InclusiveBetween(1, 600);
        RuleFor(request => request.RemainingTenureMonths)
            .InclusiveBetween(0, 600)
            .LessThanOrEqualTo(request => request.TenureMonths);

        RuleFor(request => request.CurrencyCode)
            .NotEmpty()
            .Length(3);

        RuleFor(request => request.EndDate)
            .GreaterThanOrEqualTo(request => request.StartDate)
            .When(request => request.EndDate.HasValue);
    }
}

public sealed class RecordLoanPaymentRequestValidator : AbstractValidator<RecordLoanPaymentRequest>
{
    public RecordLoanPaymentRequestValidator()
    {
        RuleFor(request => request.Amount).GreaterThan(0);
        RuleFor(request => request.PrincipalComponent).GreaterThanOrEqualTo(0);
        RuleFor(request => request.InterestComponent).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Status).IsInEnum();
        RuleFor(request => request.PaymentMode).MaximumLength(64);
        RuleFor(request => request.Reference).MaximumLength(128);
        RuleFor(request => request.Notes).MaximumLength(1000);

        RuleFor(request => request)
            .Must(request => request.PrincipalComponent + request.InterestComponent <= request.Amount + 0.01m)
            .WithMessage("Principal and interest components cannot exceed payment amount.");
    }
}

public sealed class GetLoansQueryValidator : AbstractValidator<GetLoansQuery>
{
    public GetLoansQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.Search).MaximumLength(200);
        RuleFor(query => query.Status).IsInEnum().When(query => query.Status.HasValue);
        RuleFor(query => query.Type).IsInEnum().When(query => query.Type.HasValue);
    }
}

public sealed class GetUpcomingPaymentsQueryValidator : AbstractValidator<GetUpcomingPaymentsQuery>
{
    public GetUpcomingPaymentsQueryValidator()
    {
        RuleFor(query => query.DaysAhead).InclusiveBetween(1, 365);
        RuleFor(query => query.Take).InclusiveBetween(1, 100);
    }
}

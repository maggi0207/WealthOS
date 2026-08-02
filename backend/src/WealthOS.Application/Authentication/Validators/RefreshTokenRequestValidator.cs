using FluentValidation;
using WealthOS.Application.Authentication.DTOs.Requests;

namespace WealthOS.Application.Authentication.Validators;

public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(request => request.AccessToken)
            .NotEmpty();

        RuleFor(request => request.RefreshToken)
            .NotEmpty()
            .MaximumLength(512);
    }
}

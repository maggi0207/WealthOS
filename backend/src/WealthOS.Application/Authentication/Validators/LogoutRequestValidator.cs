using FluentValidation;
using WealthOS.Application.Authentication.DTOs.Requests;

namespace WealthOS.Application.Authentication.Validators;

public sealed class LogoutRequestValidator : AbstractValidator<LogoutRequest>
{
    public LogoutRequestValidator()
    {
        RuleFor(request => request.RefreshToken)
            .NotEmpty()
            .MaximumLength(512);
    }
}

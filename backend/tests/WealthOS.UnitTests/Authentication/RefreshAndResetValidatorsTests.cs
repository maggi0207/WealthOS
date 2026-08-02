using FluentAssertions;
using FluentValidation.TestHelper;
using WealthOS.Application.Authentication.DTOs.Requests;
using WealthOS.Application.Authentication.Validators;

namespace WealthOS.UnitTests.Authentication;

public sealed class RefreshAndResetValidatorsTests
{
    [Fact]
    public void RefreshTokenRequest_EmptyTokens_ShouldFail()
    {
        var validator = new RefreshTokenRequestValidator();
        var result = validator.TestValidate(new RefreshTokenRequest());

        result.ShouldHaveValidationErrorFor(x => x.AccessToken);
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Fact]
    public void LogoutRequest_EmptyToken_ShouldFail()
    {
        var validator = new LogoutRequestValidator();
        var result = validator.TestValidate(new LogoutRequest());

        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Fact]
    public void ForgotPassword_InvalidEmail_ShouldFail()
    {
        var validator = new ForgotPasswordRequestValidator();
        var result = validator.TestValidate(new ForgotPasswordRequest { Email = "bad" });

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void ResetPassword_MismatchedConfirm_ShouldFail()
    {
        var validator = new ResetPasswordRequestValidator();
        var result = validator.TestValidate(new ResetPasswordRequest
        {
            Email = "user@wealthos.local",
            Token = "reset-token",
            NewPassword = "Secure@Pass1",
            ConfirmPassword = "Secure@Pass2",
        });

        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }
}

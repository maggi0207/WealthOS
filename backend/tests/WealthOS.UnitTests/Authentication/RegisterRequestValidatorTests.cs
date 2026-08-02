using FluentAssertions;
using FluentValidation.TestHelper;
using WealthOS.Application.Authentication.DTOs.Requests;
using WealthOS.Application.Authentication.Validators;

namespace WealthOS.UnitTests.Authentication;

public sealed class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void ValidRequest_ShouldPass()
    {
        var request = new RegisterRequest
        {
            Email = "user@wealthos.local",
            Password = "Secure@Pass1",
            ConfirmPassword = "Secure@Pass1",
            FirstName = "Ada",
            LastName = "Lovelace",
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WeakPassword_ShouldFail()
    {
        var request = new RegisterRequest
        {
            Email = "user@wealthos.local",
            Password = "password",
            ConfirmPassword = "password",
            FirstName = "Ada",
            LastName = "Lovelace",
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void MismatchedPasswords_ShouldFail()
    {
        var request = new RegisterRequest
        {
            Email = "user@wealthos.local",
            Password = "Secure@Pass1",
            ConfirmPassword = "Secure@Pass2",
            FirstName = "Ada",
            LastName = "Lovelace",
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void InvalidEmail_ShouldFail(string email)
    {
        var request = new RegisterRequest
        {
            Email = email,
            Password = "Secure@Pass1",
            ConfirmPassword = "Secure@Pass1",
            FirstName = "Ada",
            LastName = "Lovelace",
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}

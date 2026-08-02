using FluentAssertions;
using FluentValidation.TestHelper;
using WealthOS.Application.Authentication.DTOs.Requests;
using WealthOS.Application.Authentication.Validators;

namespace WealthOS.UnitTests.Authentication;

public sealed class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void ValidRequest_ShouldPass()
    {
        var request = new LoginRequest
        {
            Email = "admin@wealthos.local",
            Password = "Admin@WealthOS1!",
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyPassword_ShouldFail()
    {
        var request = new LoginRequest
        {
            Email = "admin@wealthos.local",
            Password = string.Empty,
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}

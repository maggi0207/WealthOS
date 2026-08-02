using FluentAssertions;
using WealthOS.Application.Common.Models;

namespace WealthOS.UnitTests.Authentication;

public sealed class AuthResultFlowTests
{
    [Fact]
    public void UnauthorizedError_ShouldExposeCode()
    {
        var error = Error.Unauthorized("Invalid email or password.");

        error.Code.Should().Be("unauthorized");
        error.Message.Should().Be("Invalid email or password.");
    }

    [Fact]
    public void ConflictFailure_ShouldBeFailureResult()
    {
        var result = Result.Failure<string>(Error.Conflict("An account with this email address already exists."));

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("conflict");
    }

    [Fact]
    public void ValidationError_ShouldIncludeFieldMessages()
    {
        var error = Error.Validation(
            "Identity validation failed.",
            new Dictionary<string, string[]>
            {
                ["Password"] = ["Password too short."],
            });

        error.Code.Should().Be("validation_error");
        error.ValidationErrors.Should().ContainKey("Password");
        error.ValidationErrors!["Password"].Should().Contain("Password too short.");
    }

    [Fact]
    public void SuccessResult_ShouldExposeValue()
    {
        var result = Result.Success(new { Token = "abc" });

        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be("abc");
    }
}

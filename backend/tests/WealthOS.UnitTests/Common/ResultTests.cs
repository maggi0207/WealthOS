using FluentAssertions;
using WealthOS.Application.Common.Models;

namespace WealthOS.UnitTests.Common;

public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldReturnSuccessfulResult()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void SuccessT_ShouldExposeValue()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Failure_ShouldExposeError()
    {
        var error = Error.Failure("test_error", "Test message");
        var result = Result.Failure(error);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void FailureT_AccessingValue_ShouldThrow()
    {
        var result = Result.Failure<int>(Error.Failure("test_error", "Test message"));

        var action = () => _ = result.Value;

        action.Should().Throw<InvalidOperationException>();
    }
}

using FluentAssertions;
using FluentValidation.TestHelper;
using WealthOS.Application.AI.DTOs.Requests;
using WealthOS.Application.AI.Validators;
using WealthOS.Domain.AI.Enums;

namespace WealthOS.UnitTests.AI;

/// <summary>
/// Validator coverage for AI advisor request DTOs.
/// </summary>
public sealed class AIValidatorTests
{
    private readonly SendMessageRequestValidator _sendValidator = new();
    private readonly SaveMemoryRequestValidator _memoryValidator = new();

    [Fact]
    public void SendMessage_WhenMessageEmpty_ShouldFail()
    {
        var result = _sendValidator.TestValidate(new SendMessageRequest
        {
            Message = "",
        });

        result.ShouldHaveValidationErrorFor(request => request.Message);
    }

    [Fact]
    public void SendMessage_WhenValid_ShouldPass()
    {
        var result = _sendValidator.TestValidate(new SendMessageRequest
        {
            Message = "Summarize my dashboard",
        });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void SaveMemory_WhenKeyEmpty_ShouldFail()
    {
        var result = _memoryValidator.TestValidate(new SaveMemoryRequest
        {
            Key = "",
            Content = "Prefers INR",
            MemoryType = AIMemoryType.UserPreference,
        });

        result.ShouldHaveValidationErrorFor(request => request.Key);
    }

    [Fact]
    public void SaveMemory_WhenValid_ShouldPass()
    {
        var result = _memoryValidator.TestValidate(new SaveMemoryRequest
        {
            Key = "currency",
            Content = "Prefers INR",
            MemoryType = AIMemoryType.UserPreference,
            Importance = 0.8,
        });

        result.IsValid.Should().BeTrue();
    }
}

using FluentAssertions;
using WealthOS.Application.Goals.DTOs.Requests;
using WealthOS.Application.Goals.Validators;
using WealthOS.Domain.Goals.Enums;

namespace WealthOS.UnitTests.Goals;

public sealed class GoalValidatorTests
{
    private readonly CreateGoalRequestValidator _createValidator = new();
    private readonly UpdateGoalRequestValidator _updateValidator = new();
    private readonly RecordGoalContributionRequestValidator _contributionValidator = new();

    [Fact]
    public void CreateGoal_WhenValid_ShouldSucceed()
    {
        var request = new CreateGoalRequest
        {
            Name = "Buy second house",
            Category = GoalCategory.BuyHouse,
            TargetAmount = 90_00_000m,
            CurrentAmount = 31_50_000m,
            MonthlyContribution = 85_000m,
            TargetDate = new DateOnly(2031, 4, 1),
            StartedOn = new DateOnly(2023, 4, 1),
            Priority = GoalPriority.High,
            Status = GoalStatus.Active,
            CurrencyCode = "INR",
        };

        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateGoal_WhenCurrentExceedsTarget_ShouldFail()
    {
        var request = new CreateGoalRequest
        {
            Name = "Invalid",
            Category = GoalCategory.Custom,
            TargetAmount = 100m,
            CurrentAmount = 150m,
            TargetDate = new DateOnly(2030, 1, 1),
            StartedOn = new DateOnly(2026, 1, 1),
            CurrencyCode = "INR",
        };

        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateGoalRequest.CurrentAmount));
    }

    [Fact]
    public void CreateGoal_WhenNameEmpty_ShouldFail()
    {
        var request = new CreateGoalRequest
        {
            Name = "",
            TargetAmount = 100m,
            TargetDate = new DateOnly(2030, 1, 1),
            StartedOn = new DateOnly(2026, 1, 1),
            CurrencyCode = "INR",
        };

        var result = _createValidator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateGoal_WhenTargetBeforeStart_ShouldFail()
    {
        var request = new UpdateGoalRequest
        {
            Name = "Vacation",
            Category = GoalCategory.Vacation,
            TargetAmount = 2_00_000m,
            CurrentAmount = 50_000m,
            TargetDate = new DateOnly(2025, 1, 1),
            StartedOn = new DateOnly(2026, 1, 1),
            CurrencyCode = "INR",
        };

        var result = _updateValidator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void RecordContribution_WhenAmountZero_ShouldFail()
    {
        var request = new RecordGoalContributionRequest
        {
            Amount = 0m,
            ContributedOn = new DateOnly(2026, 8, 1),
        };

        var result = _contributionValidator.Validate(request);
        result.IsValid.Should().BeFalse();
    }
}

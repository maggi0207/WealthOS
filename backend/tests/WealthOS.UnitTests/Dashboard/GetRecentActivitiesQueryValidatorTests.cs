using FluentAssertions;
using FluentValidation.TestHelper;
using WealthOS.Application.Dashboard.Queries;
using WealthOS.Application.Dashboard.Validators;

namespace WealthOS.UnitTests.Dashboard;

public sealed class GetRecentActivitiesQueryValidatorTests
{
    private readonly GetRecentActivitiesQueryValidator _validator = new();

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    public void ValidLimit_ShouldPass(int limit)
    {
        var query = new GetRecentActivitiesQuery { Limit = limit };

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(51)]
    [InlineData(-1)]
    public void InvalidLimit_ShouldFail(int limit)
    {
        var query = new GetRecentActivitiesQuery { Limit = limit };

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(q => q.Limit);
    }
}

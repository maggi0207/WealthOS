using FluentAssertions;
using WealthOS.Application.Properties.DTOs.Requests;
using WealthOS.Application.Properties.Validators;
using WealthOS.Domain.Properties.Enums;

namespace WealthOS.UnitTests.Properties;

public sealed class CreatePropertyRequestValidatorTests
{
    private readonly CreatePropertyRequestValidator _validator = new();

    [Fact]
    public void Validate_WhenValid_ShouldSucceed()
    {
        var request = new CreatePropertyRequest
        {
            Name = "Ramana Flats",
            Type = PropertyType.Apartment,
            OwnershipType = OwnershipType.Sole,
            PurchasePrice = 9_600_000m,
            CurrentMarketValue = 15_000_000m,
            CurrencyCode = "INR",
            Status = PropertyStatus.Active,
            Owners =
            [
                new PropertyOwnerRequest
                {
                    Name = "Magesh",
                    OwnershipPercentage = 100m,
                    IsPrimary = true,
                },
            ],
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenNameEmpty_ShouldFail()
    {
        var request = new CreatePropertyRequest
        {
            Name = "",
            Type = PropertyType.Residential,
            PurchasePrice = 100m,
            CurrentMarketValue = 100m,
            CurrencyCode = "INR",
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreatePropertyRequest.Name));
    }

    [Fact]
    public void Validate_WhenOwnershipExceeds100_ShouldFail()
    {
        var request = new CreatePropertyRequest
        {
            Name = "Joint Villa",
            Type = PropertyType.Villa,
            PurchasePrice = 1m,
            CurrentMarketValue = 1m,
            CurrencyCode = "INR",
            Owners =
            [
                new PropertyOwnerRequest { Name = "A", OwnershipPercentage = 60m },
                new PropertyOwnerRequest { Name = "B", OwnershipPercentage = 50m },
            ],
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage.Contains("ownership percentage", StringComparison.OrdinalIgnoreCase));
    }
}

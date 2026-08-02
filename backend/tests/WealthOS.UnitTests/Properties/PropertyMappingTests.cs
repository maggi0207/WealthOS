using AutoMapper;
using FluentAssertions;
using WealthOS.Application.Properties.DTOs.Requests;
using WealthOS.Application.Properties.DTOs.Responses;
using WealthOS.Application.Properties.Mapping;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Domain.Properties.Enums;

namespace WealthOS.UnitTests.Properties;

public sealed class PropertyMappingTests
{
    private readonly IMapper _mapper;

    public PropertyMappingTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<PropertyMappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_CreateRequest_ToProperty_ShouldMapCoreFields()
    {
        var request = new CreatePropertyRequest
        {
            Name = "  Ramana Flats  ",
            Type = PropertyType.Apartment,
            OwnershipType = OwnershipType.Sole,
            PurchasePrice = 9_600_000m,
            CurrentMarketValue = 15_000_000m,
            CurrencyCode = "inr",
            Status = PropertyStatus.Active,
            Address = new PropertyAddressRequest
            {
                City = "Chennai",
                Locality = "Adyar",
            },
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

        var property = _mapper.Map<Property>(request);

        property.Name.Should().Be("Ramana Flats");
        property.CurrencyCode.Should().Be("INR");
        property.Type.Should().Be(PropertyType.Apartment);
        property.Address.Should().NotBeNull();
        property.Address!.City.Should().Be("Chennai");
        property.Owners.Should().HaveCount(1);
    }

    [Fact]
    public void Map_Property_ToResponse_ShouldMapCollections()
    {
        var property = new Property
        {
            Name = "Plot A",
            Type = PropertyType.Plot,
            PurchasePrice = 100m,
            CurrentMarketValue = 150m,
            CurrencyCode = "INR",
            Status = PropertyStatus.Active,
            Owners =
            {
                new PropertyOwner { Name = "Owner", OwnershipPercentage = 100m, IsPrimary = true },
            },
        };

        var response = _mapper.Map<PropertyResponse>(property);

        response.Name.Should().Be("Plot A");
        response.Owners.Should().HaveCount(1);
        response.Owners[0].Name.Should().Be("Owner");
    }
}

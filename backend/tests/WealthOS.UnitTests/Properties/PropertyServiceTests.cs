using AutoMapper;
using FluentAssertions;
using Moq;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Properties.DTOs.Requests;
using WealthOS.Application.Properties.Mapping;
using WealthOS.Application.Properties.Services;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Domain.Properties.Enums;
using WealthOS.Domain.Properties.Repositories;

namespace WealthOS.UnitTests.Properties;

public sealed class PropertyServiceTests
{
    private readonly Mock<IPropertyRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly IMapper _mapper;
    private readonly Guid _userId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    public PropertyServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<PropertyMappingProfile>());
        _mapper = config.CreateMapper();

        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(true);
        _currentUser.SetupGet(user => user.UserId).Returns(_userId);

        _unitOfWork
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task GetSummaryAsync_WhenAuthenticated_ShouldMapPortfolioTotals()
    {
        _repository
            .Setup(repo => repo.GetPortfolioTotalsAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PropertyPortfolioTotals
            {
                PropertyCount = 2,
                TotalPurchasePrice = 10_000_000m,
                TotalMarketValue = 15_000_000m,
                CurrencyCode = "INR",
                ActiveCount = 2,
                RentedCount = 0,
            });

        var service = CreateService();
        var result = await service.GetSummaryAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.PropertyCount.Should().Be(2);
        result.Value.TotalMarketValue.Should().Be(15_000_000m);
        result.Value.TotalAppreciation.Should().Be(5_000_000m);
        result.Value.TotalAppreciationPercent.Should().Be(50m);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ShouldReturnNotFound()
    {
        var propertyId = Guid.NewGuid();
        _repository
            .Setup(repo => repo.GetByIdWithDetailsAsync(propertyId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Property?)null);

        var service = CreateService();
        var result = await service.GetByIdAsync(propertyId);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("not_found");
    }

    [Fact]
    public async Task CreateAsync_WhenUnauthenticated_ShouldFailUnauthorized()
    {
        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(false);
        _currentUser.SetupGet(user => user.UserId).Returns((Guid?)null);

        var service = CreateService();
        var result = await service.CreateAsync(new CreatePropertyRequest
        {
            Name = "Test",
            Type = PropertyType.Land,
            PurchasePrice = 1m,
            CurrentMarketValue = 1m,
            CurrencyCode = "INR",
        });

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("unauthorized");
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ShouldSoftDeleteViaRepository()
    {
        var propertyId = Guid.NewGuid();
        var property = new Property(propertyId) { UserId = _userId, Name = "To delete" };

        _repository
            .Setup(repo => repo.GetByIdForUserAsync(propertyId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var service = CreateService();
        var result = await service.DeleteAsync(propertyId);

        result.IsSuccess.Should().BeTrue();
        _repository.Verify(repo => repo.Remove(property), Times.Once);
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDashboardAsync_WhenFound_ShouldIncludeCounts()
    {
        var propertyId = Guid.NewGuid();
        var property = new Property(propertyId)
        {
            UserId = _userId,
            Name = "Ramana Flats",
            Type = PropertyType.Apartment,
            PurchasePrice = 9_600_000m,
            CurrentMarketValue = 15_000_000m,
            CurrencyCode = "INR",
            Status = PropertyStatus.Active,
            Owners =
            {
                new PropertyOwner
                {
                    Name = "Magesh",
                    OwnershipPercentage = 100m,
                    IsPrimary = true,
                },
            },
            Valuations =
            {
                new PropertyValuation
                {
                    ValuationDate = new DateOnly(2026, 1, 1),
                    Value = 15_000_000m,
                },
            },
        };

        _repository
            .Setup(repo => repo.GetByIdWithDetailsAsync(propertyId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var service = CreateService();
        var result = await service.GetDashboardAsync(propertyId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Property.Name.Should().Be("Ramana Flats");
        result.Value.Appreciation.Should().Be(5_400_000m);
        result.Value.ValuationCount.Should().Be(1);
        result.Value.EquityEstimate.Should().Be(15_000_000m);
    }

    private PropertyService CreateService() =>
        new(_repository.Object, _unitOfWork.Object, _currentUser.Object, _mapper);
}

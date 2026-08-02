using AutoMapper;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Properties.DTOs.Requests;
using WealthOS.Application.Properties.DTOs.Responses;
using WealthOS.Application.Properties.Interfaces;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Properties.Entities;
using WealthOS.Domain.Properties.Enums;
using WealthOS.Domain.Properties.Repositories;

namespace WealthOS.Application.Properties.Services;

/// <summary>
/// Orchestrates property CRUD, summary, and dashboard use cases.
/// </summary>
public sealed class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public PropertyService(
        IPropertyRepository propertyRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<PropertyResponse>> CreateAsync(
        CreatePropertyRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<PropertyResponse>(userResult.Error!);
        }

        var property = _mapper.Map<Property>(request);
        property.UserId = userResult.Value;

        EnsurePrimaryOwner(property);

        await _propertyRepository.AddAsync(property, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _propertyRepository.GetByIdWithDetailsAsync(
            property.Id,
            userResult.Value,
            cancellationToken);

        return Result.Success(MapDetail(created!));
    }

    public async Task<Result<PropertyResponse>> UpdateAsync(
        Guid propertyId,
        UpdatePropertyRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<PropertyResponse>(userResult.Error!);
        }

        var property = await _propertyRepository.GetByIdWithDetailsAsync(
            propertyId,
            userResult.Value,
            cancellationToken);

        if (property is null)
        {
            return Result.Failure<PropertyResponse>(Error.NotFound(nameof(Property), propertyId));
        }

        ApplyUpdate(property, request);
        EnsurePrimaryOwner(property);

        _propertyRepository.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _propertyRepository.GetByIdWithDetailsAsync(
            propertyId,
            userResult.Value,
            cancellationToken);

        return Result.Success(MapDetail(updated!));
    }

    public async Task<Result> DeleteAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error!);
        }

        var property = await _propertyRepository.GetByIdForUserAsync(
            propertyId,
            userResult.Value,
            cancellationToken);

        if (property is null)
        {
            return Result.Failure(Error.NotFound(nameof(Property), propertyId));
        }

        _propertyRepository.Remove(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<PropertyResponse>> GetByIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<PropertyResponse>(userResult.Error!);
        }

        var property = await _propertyRepository.GetByIdWithDetailsAsync(
            propertyId,
            userResult.Value,
            cancellationToken);

        if (property is null)
        {
            return Result.Failure<PropertyResponse>(Error.NotFound(nameof(Property), propertyId));
        }

        return Result.Success(MapDetail(property));
    }

    public async Task<Result<PropertyListResponse>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        PropertyStatus? status,
        PropertyType? type,
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<PropertyListResponse>(userResult.Error!);
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, totalCount) = await _propertyRepository.ListForUserAsync(
            userResult.Value,
            page,
            pageSize,
            search,
            status,
            type,
            cancellationToken);

        var response = new PropertyListResponse
        {
            Items = items.Select(MapListItem).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize),
        };

        return Result.Success(response);
    }

    public async Task<Result<PropertySummaryResponse>> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var userResult = RequireUserId();
        if (userResult.IsFailure)
        {
            return Result.Failure<PropertySummaryResponse>(userResult.Error!);
        }

        var totals = await _propertyRepository.GetPortfolioTotalsAsync(
            userResult.Value,
            cancellationToken);

        var appreciation = totals.TotalMarketValue - totals.TotalPurchasePrice;
        decimal? appreciationPercent = totals.TotalPurchasePrice == 0
            ? null
            : Math.Round(appreciation / totals.TotalPurchasePrice * 100m, 2);

        return Result.Success(new PropertySummaryResponse
        {
            PropertyCount = totals.PropertyCount,
            TotalPurchasePrice = totals.TotalPurchasePrice,
            TotalMarketValue = totals.TotalMarketValue,
            TotalAppreciation = appreciation,
            TotalAppreciationPercent = appreciationPercent,
            CurrencyCode = totals.CurrencyCode,
            ActiveCount = totals.ActiveCount,
            RentedCount = totals.RentedCount,
        });
    }

    public async Task<Result<PropertyDashboardResponse>> GetDashboardAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        var detailResult = await GetByIdAsync(propertyId, cancellationToken);
        if (detailResult.IsFailure)
        {
            return Result.Failure<PropertyDashboardResponse>(detailResult.Error!);
        }

        var property = detailResult.Value;

        return Result.Success(new PropertyDashboardResponse
        {
            Property = property,
            EquityEstimate = property.CurrentMarketValue,
            Appreciation = property.Appreciation,
            AppreciationPercent = property.AppreciationPercent,
            ValuationCount = property.Valuations.Count,
            LoanLinkCount = property.LoanLinks.Count,
            DocumentLinkCount = property.DocumentLinks.Count,
            ImageCount = property.Images.Count,
            NoteCount = property.PropertyNotes.Count,
            GeneratedAt = DateTime.UtcNow,
        });
    }

    private Result<Guid> RequireUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result.Failure<Guid>(Error.Unauthorized());
        }

        return Result.Success(_currentUser.UserId.Value);
    }

    private PropertyResponse MapDetail(Property property)
    {
        var response = _mapper.Map<PropertyResponse>(property);
        ApplyComputedFields(response, property);
        return response;
    }

    private PropertyListItemResponse MapListItem(Property property)
    {
        var item = _mapper.Map<PropertyListItemResponse>(property);
        item.PrimaryOwnerName = property.Owners.FirstOrDefault(owner => owner.IsPrimary)?.Name
            ?? property.Owners.FirstOrDefault()?.Name;
        item.City = property.Address?.City;
        item.Locality = property.Address?.Locality;
        return item;
    }

    private static void ApplyComputedFields(PropertyResponse response, Property property)
    {
        response.PrimaryOwnerName = property.Owners.FirstOrDefault(owner => owner.IsPrimary)?.Name
            ?? property.Owners.FirstOrDefault()?.Name;

        response.Appreciation = property.CurrentMarketValue - property.PurchasePrice;
        response.AppreciationPercent = property.PurchasePrice == 0
            ? null
            : Math.Round(response.Appreciation / property.PurchasePrice * 100m, 2);
    }

    private static void ApplyUpdate(Property property, UpdatePropertyRequest request)
    {
        property.Name = request.Name.Trim();
        property.Type = request.Type;
        property.OwnershipType = request.OwnershipType;
        property.PurchaseDate = request.PurchaseDate;
        property.PurchasePrice = request.PurchasePrice;
        property.CurrentMarketValue = request.CurrentMarketValue;
        property.CurrencyCode = string.IsNullOrWhiteSpace(request.CurrencyCode)
            ? "INR"
            : request.CurrencyCode.Trim().ToUpperInvariant();
        property.Area = request.Area;
        property.BuiltUpArea = request.BuiltUpArea;
        property.Floor = request.Floor;
        property.Facing = request.Facing;
        property.Bedrooms = request.Bedrooms;
        property.Bathrooms = request.Bathrooms;
        property.Parking = request.Parking;
        property.Status = request.Status;
        property.Description = request.Description;
        property.Notes = request.Notes;
        property.IsRentalEnabled = request.IsRentalEnabled;

        if (request.Address is not null)
        {
            property.Address ??= new PropertyAddress();
            ApplyAddress(property.Address, request.Address);
        }

        if (request.Owners is not null)
        {
            property.Owners.Clear();
            foreach (var ownerRequest in request.Owners)
            {
                property.Owners.Add(MapOwner(ownerRequest));
            }
        }
    }

    private static void ApplyAddress(PropertyAddress address, PropertyAddressRequest request)
    {
        address.Line1 = request.Line1;
        address.Line2 = request.Line2;
        address.Locality = request.Locality;
        address.City = request.City;
        address.State = request.State;
        address.PostalCode = request.PostalCode;
        address.Country = request.Country;
        address.FullAddress = request.FullAddress;
        address.Latitude = request.Latitude;
        address.Longitude = request.Longitude;
        address.GoogleMapsUrl = request.GoogleMapsUrl;
    }

    private static PropertyOwner MapOwner(PropertyOwnerRequest request) =>
        new()
        {
            Name = request.Name.Trim(),
            OwnershipPercentage = request.OwnershipPercentage,
            OwnershipType = request.OwnershipType,
            IsPrimary = request.IsPrimary,
            LinkedUserId = request.LinkedUserId,
        };

    private static void EnsurePrimaryOwner(Property property)
    {
        if (property.Owners.Count == 0)
        {
            return;
        }

        if (!property.Owners.Any(owner => owner.IsPrimary))
        {
            property.Owners.First().IsPrimary = true;
        }
    }
}

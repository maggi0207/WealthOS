using WealthOS.Domain.Properties.Enums;

namespace WealthOS.Application.Properties.DTOs.Requests;

/// <summary>
/// Address payload for create/update property requests.
/// </summary>
public sealed class PropertyAddressRequest
{
    public string? Line1 { get; set; }

    public string? Line2 { get; set; }

    public string? Locality { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public string? FullAddress { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? GoogleMapsUrl { get; set; }
}

/// <summary>
/// Owner payload for create/update property requests.
/// </summary>
public sealed class PropertyOwnerRequest
{
    public string Name { get; set; } = string.Empty;

    public decimal OwnershipPercentage { get; set; } = 100m;

    public OwnershipType OwnershipType { get; set; } = OwnershipType.Sole;

    public bool IsPrimary { get; set; } = true;

    public Guid? LinkedUserId { get; set; }
}

/// <summary>
/// Request to create a new property.
/// </summary>
public sealed class CreatePropertyRequest
{
    public string Name { get; set; } = string.Empty;

    public PropertyType Type { get; set; }

    public OwnershipType OwnershipType { get; set; } = OwnershipType.Sole;

    public DateOnly? PurchaseDate { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal CurrentMarketValue { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public decimal? Area { get; set; }

    public decimal? BuiltUpArea { get; set; }

    public string? Floor { get; set; }

    public string? Facing { get; set; }

    public int? Bedrooms { get; set; }

    public int? Bathrooms { get; set; }

    public int? Parking { get; set; }

    public PropertyStatus Status { get; set; } = PropertyStatus.Active;

    public string? Description { get; set; }

    public string? Notes { get; set; }

    public bool IsRentalEnabled { get; set; }

    public PropertyAddressRequest? Address { get; set; }

    public IReadOnlyList<PropertyOwnerRequest>? Owners { get; set; }
}

/// <summary>
/// Request to update an existing property.
/// </summary>
public sealed class UpdatePropertyRequest
{
    public string Name { get; set; } = string.Empty;

    public PropertyType Type { get; set; }

    public OwnershipType OwnershipType { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal CurrentMarketValue { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public decimal? Area { get; set; }

    public decimal? BuiltUpArea { get; set; }

    public string? Floor { get; set; }

    public string? Facing { get; set; }

    public int? Bedrooms { get; set; }

    public int? Bathrooms { get; set; }

    public int? Parking { get; set; }

    public PropertyStatus Status { get; set; }

    public string? Description { get; set; }

    public string? Notes { get; set; }

    public bool IsRentalEnabled { get; set; }

    public PropertyAddressRequest? Address { get; set; }

    public IReadOnlyList<PropertyOwnerRequest>? Owners { get; set; }
}

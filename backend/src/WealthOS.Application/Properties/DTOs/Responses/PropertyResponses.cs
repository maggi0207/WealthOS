using WealthOS.Domain.Properties.Enums;

namespace WealthOS.Application.Properties.DTOs.Responses;

/// <summary>
/// Address response for a property.
/// </summary>
public sealed class PropertyAddressResponse
{
    public Guid Id { get; set; }

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
/// Owner response for a property.
/// </summary>
public sealed class PropertyOwnerResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal OwnershipPercentage { get; set; }

    public OwnershipType OwnershipType { get; set; }

    public bool IsPrimary { get; set; }

    public Guid? LinkedUserId { get; set; }
}

/// <summary>
/// Valuation history entry.
/// </summary>
public sealed class PropertyValuationResponse
{
    public Guid Id { get; set; }

    public DateOnly ValuationDate { get; set; }

    public decimal Value { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public string? Source { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// Future loan link stub.
/// </summary>
public sealed class PropertyLoanLinkResponse
{
    public Guid Id { get; set; }

    public Guid LoanId { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// Future document link stub.
/// </summary>
public sealed class PropertyDocumentLinkResponse
{
    public Guid Id { get; set; }

    public Guid DocumentId { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// Image metadata (no upload in Phase 4).
/// </summary>
public sealed class PropertyImageResponse
{
    public Guid Id { get; set; }

    public string? Url { get; set; }

    public string? Caption { get; set; }

    public string? Category { get; set; }

    public int SortOrder { get; set; }

    public bool IsPrimary { get; set; }
}

/// <summary>
/// Structured property note.
/// </summary>
public sealed class PropertyNoteResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;
}

/// <summary>
/// Full property detail response.
/// </summary>
public sealed class PropertyResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public PropertyType Type { get; set; }

    public OwnershipType OwnershipType { get; set; }

    public string? PrimaryOwnerName { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal CurrentMarketValue { get; set; }

    public decimal Appreciation { get; set; }

    public decimal? AppreciationPercent { get; set; }

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

    public PropertyAddressResponse? Address { get; set; }

    public IReadOnlyList<PropertyOwnerResponse> Owners { get; set; } = [];

    public IReadOnlyList<PropertyValuationResponse> Valuations { get; set; } = [];

    public IReadOnlyList<PropertyLoanLinkResponse> LoanLinks { get; set; } = [];

    public IReadOnlyList<PropertyDocumentLinkResponse> DocumentLinks { get; set; } = [];

    public IReadOnlyList<PropertyImageResponse> Images { get; set; } = [];

    public IReadOnlyList<PropertyNoteResponse> PropertyNotes { get; set; } = [];

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Lightweight list item for property collections.
/// </summary>
public sealed class PropertyListItemResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public PropertyType Type { get; set; }

    public PropertyStatus Status { get; set; }

    public string? PrimaryOwnerName { get; set; }

    public string? City { get; set; }

    public string? Locality { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal CurrentMarketValue { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public DateOnly? PurchaseDate { get; set; }
}

/// <summary>
/// Paginated property list payload.
/// </summary>
public sealed class PropertyListResponse
{
    public IReadOnlyList<PropertyListItemResponse> Items { get; set; } = [];

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}

/// <summary>
/// Portfolio-level property summary.
/// </summary>
public sealed class PropertySummaryResponse
{
    public int PropertyCount { get; set; }

    public decimal TotalPurchasePrice { get; set; }

    public decimal TotalMarketValue { get; set; }

    public decimal TotalAppreciation { get; set; }

    public decimal? TotalAppreciationPercent { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public int ActiveCount { get; set; }

    public int RentedCount { get; set; }
}

/// <summary>
/// Per-property dashboard snapshot (detail + equity + related stub counts).
/// </summary>
public sealed class PropertyDashboardResponse
{
    public PropertyResponse Property { get; set; } = null!;

    public decimal EquityEstimate { get; set; }

    public decimal Appreciation { get; set; }

    public decimal? AppreciationPercent { get; set; }

    public int ValuationCount { get; set; }

    public int LoanLinkCount { get; set; }

    public int DocumentLinkCount { get; set; }

    public int ImageCount { get; set; }

    public int NoteCount { get; set; }

    public DateTime GeneratedAt { get; set; }
}

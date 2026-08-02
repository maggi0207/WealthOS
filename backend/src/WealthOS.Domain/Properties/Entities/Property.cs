using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Properties.Enums;

namespace WealthOS.Domain.Properties.Entities;

/// <summary>
/// Aggregate root for a portfolio property. Soft-deleted via <see cref="AuditableEntity"/>.
/// </summary>
public sealed class Property : AuditableEntity
{
    public Property()
    {
    }

    public Property(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

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

    public PropertyStatus Status { get; set; } = PropertyStatus.Active;

    public string? Description { get; set; }

    public string? Notes { get; set; }

    /// <summary>
    /// Future-ready flag for rental income tracking (no rental APIs in Phase 4).
    /// </summary>
    public bool IsRentalEnabled { get; set; }

    public PropertyAddress? Address { get; set; }

    public ICollection<PropertyOwner> Owners { get; set; } = new List<PropertyOwner>();

    public ICollection<PropertyValuation> Valuations { get; set; } = new List<PropertyValuation>();

    public ICollection<PropertyLoanLink> LoanLinks { get; set; } = new List<PropertyLoanLink>();

    public ICollection<PropertyDocumentLink> DocumentLinks { get; set; } = new List<PropertyDocumentLink>();

    public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();

    public ICollection<PropertyNote> PropertyNotes { get; set; } = new List<PropertyNote>();
}

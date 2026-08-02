using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Properties.Entities;

/// <summary>
/// Physical location and map coordinates for a property (1:1 with <see cref="Property"/>).
/// </summary>
public sealed class PropertyAddress : AuditableEntity
{
    public Guid PropertyId { get; set; }

    public Property Property { get; set; } = null!;

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

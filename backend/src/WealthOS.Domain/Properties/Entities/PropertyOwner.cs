using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Properties.Enums;

namespace WealthOS.Domain.Properties.Entities;

/// <summary>
/// Named owner (or co-owner) of a property, with optional link to a system user.
/// </summary>
public sealed class PropertyOwner : AuditableEntity
{
    public Guid PropertyId { get; set; }

    public Property Property { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public decimal OwnershipPercentage { get; set; }

    public OwnershipType OwnershipType { get; set; }

    public bool IsPrimary { get; set; }

    public Guid? LinkedUserId { get; set; }
}

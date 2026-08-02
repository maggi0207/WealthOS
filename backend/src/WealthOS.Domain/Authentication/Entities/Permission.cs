using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Authentication.Entities;

/// <summary>
/// Future-ready permission definition for fine-grained authorization.
/// Not enforced in Phase 2 Authentication.
/// </summary>
public sealed class Permission : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

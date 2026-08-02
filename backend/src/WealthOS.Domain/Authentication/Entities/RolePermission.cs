using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Authentication.Entities;

/// <summary>
/// Join entity between roles and permissions (stub for future RBAC expansion).
/// </summary>
public sealed class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public Guid PermissionId { get; set; }

    public Permission Permission { get; set; } = null!;
}

using Microsoft.AspNetCore.Identity;
using WealthOS.Domain.Common.Interfaces;

namespace WealthOS.Domain.Authentication.Entities;

public class Role : IdentityRole<Guid>, IAuditableEntity, ISoftDeletable
{
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

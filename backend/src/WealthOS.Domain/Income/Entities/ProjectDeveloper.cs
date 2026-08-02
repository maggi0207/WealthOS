using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Income.Entities;

/// <summary>
/// Assignment of a developer to a project (many-to-many join).
/// </summary>
public sealed class ProjectDeveloper : AuditableEntity
{
    public ProjectDeveloper()
    {
    }

    public ProjectDeveloper(Guid id)
        : base(id)
    {
    }

    public Guid ProjectId { get; set; }

    public BusinessProject? Project { get; set; }

    public Guid DeveloperId { get; set; }

    public Developer? Developer { get; set; }

    public DateOnly AssignedOn { get; set; }

    public string? RoleOnProject { get; set; }

    public bool IsActive { get; set; } = true;
}

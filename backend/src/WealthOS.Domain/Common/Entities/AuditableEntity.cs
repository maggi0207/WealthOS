using WealthOS.Domain.Common.Interfaces;

namespace WealthOS.Domain.Common.Entities;

public abstract class AuditableEntity : BaseEntity, IAuditableEntity, ISoftDeletable
{
    protected AuditableEntity()
    {
    }

    protected AuditableEntity(Guid id)
        : base(id)
    {
    }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }
}

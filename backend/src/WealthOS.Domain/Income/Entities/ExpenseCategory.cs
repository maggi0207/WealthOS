using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Income.Entities;

/// <summary>
/// User-defined (or seeded) business expense category.
/// </summary>
public sealed class ExpenseCategory : AuditableEntity
{
    public ExpenseCategory()
    {
    }

    public ExpenseCategory(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsSystem { get; set; }

    public ICollection<BusinessExpense> Expenses { get; set; } = new List<BusinessExpense>();
}

using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Income.Entities;

/// <summary>
/// Contractor / employee paid via business payroll.
/// </summary>
public sealed class Developer : AuditableEntity
{
    public Developer()
    {
    }

    public Developer(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public decimal MonthlySalary { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    /// <summary>
    /// Optional primary client for payroll UI grouping (not a strict FK ownership).
    /// </summary>
    public Guid? PrimaryClientId { get; set; }

    public BusinessClient? PrimaryClient { get; set; }

    public bool IsActive { get; set; } = true;

    public string? Notes { get; set; }

    public ICollection<ProjectDeveloper> ProjectAssignments { get; set; } = new List<ProjectDeveloper>();

    public ICollection<DeveloperPayroll> PayrollRecords { get; set; } = new List<DeveloperPayroll>();
}

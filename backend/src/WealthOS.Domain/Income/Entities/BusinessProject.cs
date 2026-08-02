using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Income.Enums;

namespace WealthOS.Domain.Income.Entities;

/// <summary>
/// Delivery project owned by a business client.
/// </summary>
public sealed class BusinessProject : AuditableEntity
{
    public BusinessProject()
    {
    }

    public BusinessProject(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid ClientId { get; set; }

    public BusinessClient? Client { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal? MonthlyRevenue { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public ICollection<ProjectDeveloper> Developers { get; set; } = new List<ProjectDeveloper>();

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

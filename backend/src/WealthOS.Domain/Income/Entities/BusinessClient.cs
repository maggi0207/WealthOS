using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Income.Enums;

namespace WealthOS.Domain.Income.Entities;

/// <summary>
/// Business client (retainer / T&amp;M / support engagement).
/// </summary>
public sealed class BusinessClient : AuditableEntity
{
    public BusinessClient()
    {
    }

    public BusinessClient(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Short engagement label (e.g. "Retainer · Web platform").
    /// </summary>
    public string Engagement { get; set; } = string.Empty;

    public ClientStatus Status { get; set; } = ClientStatus.Active;

    /// <summary>
    /// Expected / contracted monthly revenue for dashboard estimates.
    /// </summary>
    public decimal MonthlyRevenue { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string? Notes { get; set; }

    public ICollection<BusinessProject> Projects { get; set; } = new List<BusinessProject>();

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

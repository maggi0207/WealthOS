using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Income.Enums;

namespace WealthOS.Domain.Income.Entities;

/// <summary>
/// Client invoice aggregate root.
/// </summary>
public sealed class Invoice : AuditableEntity
{
    public Invoice()
    {
    }

    public Invoice(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid ClientId { get; set; }

    public BusinessClient? Client { get; set; }

    public Guid? ProjectId { get; set; }

    public BusinessProject? Project { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public DateOnly IssueDate { get; set; }

    public DateOnly DueDate { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    public decimal SubTotal { get; set; }

    public decimal AmountPaid { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public string? Notes { get; set; }

    public decimal OutstandingAmount => Math.Max(0m, SubTotal - AmountPaid);

    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

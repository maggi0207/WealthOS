using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Income.Entities;

/// <summary>
/// Line item on an invoice.
/// </summary>
public sealed class InvoiceItem : AuditableEntity
{
    public InvoiceItem()
    {
    }

    public InvoiceItem(Guid id)
        : base(id)
    {
    }

    public Guid InvoiceId { get; set; }

    public Invoice? Invoice { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Quantity { get; set; } = 1m;

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}

using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Income.Enums;

namespace WealthOS.Domain.Income.Entities;

/// <summary>
/// Payment received against an invoice.
/// </summary>
public sealed class Payment : AuditableEntity
{
    public Payment()
    {
    }

    public Payment(Guid id)
        : base(id)
    {
    }

    public Guid InvoiceId { get; set; }

    public Invoice? Invoice { get; set; }

    public Guid UserId { get; set; }

    public decimal Amount { get; set; }

    public DateOnly PaidOn { get; set; }

    public PaymentMethod Method { get; set; } = PaymentMethod.BankTransfer;

    public string? Reference { get; set; }

    public string? Notes { get; set; }
}

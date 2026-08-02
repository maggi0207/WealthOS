using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Investments.Entities;

/// <summary>
/// Dividend received for a holding.
/// </summary>
public sealed class Dividend : AuditableEntity
{
    public Dividend()
    {
    }

    public Dividend(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid AccountId { get; set; }

    public Guid HoldingId { get; set; }

    public Holding? Holding { get; set; }

    public decimal Amount { get; set; }

    public DateOnly? ExDate { get; set; }

    public DateOnly PaymentDate { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public string? Notes { get; set; }
}

using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Investments.Enums;

namespace WealthOS.Domain.Investments.Entities;

/// <summary>
/// Cash / position movement against a holding (and its account).
/// </summary>
public sealed class InvestmentTransaction : AuditableEntity
{
    public InvestmentTransaction()
    {
    }

    public InvestmentTransaction(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid AccountId { get; set; }

    public InvestmentAccount? Account { get; set; }

    public Guid? HoldingId { get; set; }

    public Holding? Holding { get; set; }

    public InvestmentTransactionType TransactionType { get; set; }

    public decimal Quantity { get; set; }

    public decimal Price { get; set; }

    /// <summary>
    /// Signed cash amount (buys negative / sells positive depending on convention — store absolute with type).
    /// Amount is always non-negative; direction is implied by <see cref="TransactionType"/>.
    /// </summary>
    public decimal Amount { get; set; }

    public decimal Fees { get; set; }

    public DateOnly TransactionDate { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public string? Notes { get; set; }

    public string? ExternalReference { get; set; }
}

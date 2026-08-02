using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Investments.Enums;

namespace WealthOS.Domain.Investments.Entities;

/// <summary>
/// A position held inside an investment account.
/// </summary>
public sealed class Holding : AuditableEntity
{
    public Holding()
    {
    }

    public Holding(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid AccountId { get; set; }

    public InvestmentAccount? Account { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Symbol { get; set; } = string.Empty;

    public InvestmentCategory Category { get; set; } = InvestmentCategory.Other;

    public InvestmentType InvestmentType { get; set; } = InvestmentType.Other;

    public decimal Quantity { get; set; }

    /// <summary>
    /// Average acquisition cost per unit.
    /// </summary>
    public decimal AverageCost { get; set; }

    public decimal InvestedAmount { get; set; }

    /// <summary>
    /// Last known unit price (manual or last sync). No live market feed in Phase 7.
    /// </summary>
    public decimal CurrentPrice { get; set; }

    public decimal CurrentValue { get; set; }

    public decimal DayChange { get; set; }

    public decimal DayChangePercent { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public string? Notes { get; set; }

    public ICollection<InvestmentTransaction> Transactions { get; set; } = new List<InvestmentTransaction>();

    public ICollection<Dividend> Dividends { get; set; } = new List<Dividend>();

    public ICollection<CorporateAction> CorporateActions { get; set; } = new List<CorporateAction>();
}

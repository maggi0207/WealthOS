using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Investments.Entities;

/// <summary>
/// Daily (or periodic) snapshot of an account's portfolio value for history / charts.
/// </summary>
public sealed class PortfolioSnapshot : AuditableEntity
{
    public PortfolioSnapshot()
    {
    }

    public PortfolioSnapshot(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid AccountId { get; set; }

    public InvestmentAccount? Account { get; set; }

    public DateOnly SnapshotDate { get; set; }

    public decimal InvestedAmount { get; set; }

    public decimal CurrentValue { get; set; }

    public decimal DayChange { get; set; }

    public decimal DayChangePercent { get; set; }

    public string CurrencyCode { get; set; } = "INR";
}

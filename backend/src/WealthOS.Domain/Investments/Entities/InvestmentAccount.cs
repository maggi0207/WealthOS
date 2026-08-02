using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Investments.Enums;

namespace WealthOS.Domain.Investments.Entities;

/// <summary>
/// User-owned investment account linked to a provider (not a single-broker model).
/// </summary>
public sealed class InvestmentAccount : AuditableEntity
{
    public InvestmentAccount()
    {
    }

    public InvestmentAccount(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid ProviderId { get; set; }

    public InvestmentProvider? Provider { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Display owner (e.g. "Magesh", "Wife", "Household").
    /// </summary>
    public string OwnerName { get; set; } = string.Empty;

    /// <summary>
    /// Free-text kind label (e.g. "Broker · Stocks &amp; MF").
    /// </summary>
    public string KindLabel { get; set; } = string.Empty;

    public InvestmentAccountStatus Status { get; set; } = InvestmentAccountStatus.Manual;

    public DateTime? LastSyncedAt { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public string? Notes { get; set; }

    /// <summary>
    /// Optional opaque connection metadata for future broker APIs (never secrets in plaintext).
    /// </summary>
    public string? ExternalAccountReference { get; set; }

    public ICollection<Holding> Holdings { get; set; } = new List<Holding>();

    public ICollection<PortfolioSnapshot> Snapshots { get; set; } = new List<PortfolioSnapshot>();

    public ICollection<InvestmentTransaction> Transactions { get; set; } = new List<InvestmentTransaction>();
}

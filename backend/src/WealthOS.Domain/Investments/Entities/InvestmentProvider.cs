using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Investments.Enums;

namespace WealthOS.Domain.Investments.Entities;

/// <summary>
/// Catalog entry for an investment provider (Manual, Angel One, IndiaBonds, etc.).
/// Providers are shared reference data; accounts belong to users.
/// </summary>
public sealed class InvestmentProvider : AuditableEntity
{
    public InvestmentProvider()
    {
    }

    public InvestmentProvider(Guid id)
        : base(id)
    {
    }

    public ProviderKind Kind { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>
    /// When false, UI should treat the provider as unavailable / coming soon.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Indicates whether automated portfolio sync is implemented for this provider.
    /// Manual and placeholders return false / stub behaviour.
    /// </summary>
    public bool SupportsSync { get; set; }

    public ICollection<InvestmentAccount> Accounts { get; set; } = new List<InvestmentAccount>();
}

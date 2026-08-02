using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Properties.Entities;

/// <summary>
/// Historical market valuation snapshot. Schema prepared for future valuation workflows.
/// </summary>
public sealed class PropertyValuation : AuditableEntity
{
    public Guid PropertyId { get; set; }

    public Property Property { get; set; } = null!;

    public DateOnly ValuationDate { get; set; }

    public decimal Value { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public string? Source { get; set; }

    public string? Notes { get; set; }
}

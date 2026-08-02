namespace WealthOS.Application.Dashboard.Providers;

/// <summary>
/// Cross-module property totals. Implemented by Properties module later; mock in Phase 3.
/// </summary>
public interface IPropertySummaryProvider
{
    Task<PropertyModuleSummary> GetSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Property module summary shape consumed by Dashboard.
/// </summary>
public sealed class PropertyModuleSummary
{
    public decimal TotalValue { get; init; }

    public int PropertyCount { get; init; }

    public string CurrencyCode { get; init; } = "USD";
}

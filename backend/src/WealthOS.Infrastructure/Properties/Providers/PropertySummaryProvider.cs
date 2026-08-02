using WealthOS.Application.Dashboard.Providers;
using WealthOS.Domain.Properties.Repositories;

namespace WealthOS.Infrastructure.Properties.Providers;

/// <summary>
/// Dashboard property totals backed by the Properties module repository.
/// </summary>
public sealed class PropertySummaryProvider : IPropertySummaryProvider
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertySummaryProvider(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PropertyModuleSummary> GetSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var totals = await _propertyRepository.GetPortfolioTotalsAsync(userId, cancellationToken);

        return new PropertyModuleSummary
        {
            TotalValue = totals.TotalMarketValue,
            PropertyCount = totals.PropertyCount,
            CurrencyCode = string.IsNullOrWhiteSpace(totals.CurrencyCode) ? "INR" : totals.CurrencyCode,
        };
    }
}

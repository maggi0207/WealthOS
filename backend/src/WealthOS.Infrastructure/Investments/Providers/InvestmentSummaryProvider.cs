using WealthOS.Application.Dashboard.Providers;
using WealthOS.Domain.Investments.Repositories;

namespace WealthOS.Infrastructure.Investments.Providers;

/// <summary>
/// Dashboard investment totals backed by holdings in the Investments module.
/// </summary>
public sealed class InvestmentSummaryProvider : IInvestmentSummaryProvider
{
    private readonly IHoldingRepository _holdingRepository;

    public InvestmentSummaryProvider(IHoldingRepository holdingRepository)
    {
        _holdingRepository = holdingRepository;
    }

    public async Task<InvestmentModuleSummary> GetSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var holdings = await _holdingRepository.ListAllForUserAsync(userId, cancellationToken);
        var currency = holdings
            .Select(holding => holding.CurrencyCode)
            .FirstOrDefault(code => !string.IsNullOrWhiteSpace(code))
            ?? "INR";

        return new InvestmentModuleSummary
        {
            TotalValue = holdings.Sum(holding => holding.CurrentValue),
            HoldingCount = holdings.Count,
            CurrencyCode = currency,
        };
    }
}

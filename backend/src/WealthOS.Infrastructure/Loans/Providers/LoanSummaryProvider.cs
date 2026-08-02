using WealthOS.Application.Dashboard.Providers;
using WealthOS.Domain.Loans.Repositories;

namespace WealthOS.Infrastructure.Loans.Providers;

/// <summary>
/// Dashboard loan totals backed by the Loans module repository.
/// </summary>
public sealed class LoanSummaryProvider : ILoanSummaryProvider
{
    private readonly ILoanRepository _loanRepository;

    public LoanSummaryProvider(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<LoanModuleSummary> GetSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var summary = await _loanRepository.GetPortfolioSummaryAsync(userId, cancellationToken);

        return new LoanModuleSummary
        {
            TotalBalance = summary.OutstandingBalance,
            LoanCount = summary.LoanCount,
            CurrencyCode = summary.CurrencyCode,
        };
    }
}

using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Loans.Entities;
using WealthOS.Domain.Loans.Enums;
using WealthOS.Domain.Loans.Models;

namespace WealthOS.Domain.Loans.Repositories;

/// <summary>
/// Persistence abstraction for the Loan aggregate.
/// </summary>
public interface ILoanRepository : IRepository<Loan>
{
    Task<Loan?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Loan?> GetByIdWithDetailsAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Loan> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        LoanStatus? status,
        LoanType? type,
        CancellationToken cancellationToken = default);

    Task<LoanSummary> GetPortfolioSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LoanReminder>> GetUpcomingRemindersAsync(
        Guid userId,
        int daysAhead,
        int take,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Loan>> GetActiveLoansWithNextEmiAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Persistence abstraction for loan providers (lenders).
/// </summary>
public interface ILoanProviderRepository : IRepository<LoanProvider>
{
    Task<LoanProvider?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<LoanProvider?> FindByNameForUserAsync(
        Guid userId,
        string name,
        CancellationToken cancellationToken = default);
}

using WealthOS.Application.Common.Abstractions;
using WealthOS.Domain.Loans.Enums;

namespace WealthOS.Application.Loans.Queries;

/// <summary>
/// Loads a single loan by id for the authenticated user.
/// </summary>
public sealed class GetLoanQuery : IQuery
{
    public Guid LoanId { get; init; }
}

/// <summary>
/// Lists loans for the authenticated user with optional filters and paging.
/// </summary>
public sealed class GetLoansQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? Search { get; init; }

    public LoanStatus? Status { get; init; }

    public LoanType? Type { get; init; }
}

/// <summary>
/// Portfolio-level loan totals for the authenticated user.
/// </summary>
public sealed class GetLoanSummaryQuery : IQuery;

/// <summary>
/// Upcoming EMI reminders for the authenticated user.
/// </summary>
public sealed class GetUpcomingPaymentsQuery : IQuery
{
    public int DaysAhead { get; init; } = 45;

    public int Take { get; init; } = 20;
}

/// <summary>
/// Per-loan dashboard snapshot for the authenticated user.
/// </summary>
public sealed class GetLoanDashboardQuery : IQuery
{
    public Guid LoanId { get; init; }
}

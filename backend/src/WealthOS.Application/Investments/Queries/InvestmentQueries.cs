using WealthOS.Application.Common.Abstractions;
using WealthOS.Domain.Investments.Enums;

namespace WealthOS.Application.Investments.Queries;

public sealed class GetPortfolioQuery : IQuery
{
    public Guid? AccountId { get; init; }
}

public sealed class GetPortfolioSummaryQuery : IQuery;

public sealed class GetHoldingsQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 50;

    public Guid? AccountId { get; init; }

    public InvestmentCategory? Category { get; init; }

    public string? Search { get; init; }
}

public sealed class GetTransactionsQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 50;

    public Guid? AccountId { get; init; }

    public Guid? HoldingId { get; init; }

    public InvestmentTransactionType? Type { get; init; }
}

public sealed class GetAllocationQuery : IQuery
{
    public Guid? AccountId { get; init; }
}

public sealed class GetPerformanceQuery : IQuery
{
    public PerformanceRange Range { get; init; } = PerformanceRange.OneYear;
}

public sealed class GetInvestmentDashboardSummaryQuery : IQuery;

public sealed class GetAccountsQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 50;

    public InvestmentAccountStatus? Status { get; init; }

    public Guid? ProviderId { get; init; }
}

public sealed class GetAccountByIdQuery : IQuery
{
    public Guid AccountId { get; init; }
}

public sealed class GetProvidersQuery : IQuery;

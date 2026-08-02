using WealthOS.Application.Common.Abstractions;
using WealthOS.Domain.Income.Enums;

namespace WealthOS.Application.Income.Queries;

public sealed class GetIncomeDashboardQuery : IQuery
{
    public string? Period { get; init; }
}

public sealed class GetClientsQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? Search { get; init; }

    public ClientStatus? Status { get; init; }
}

public sealed class GetProjectsQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public Guid? ClientId { get; init; }

    public ProjectStatus? Status { get; init; }

    public string? Search { get; init; }
}

public sealed class GetInvoicesQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public Guid? ClientId { get; init; }

    public InvoiceStatus? Status { get; init; }

    public string? Search { get; init; }
}

public sealed class GetDevelopersQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? Search { get; init; }

    public bool? IsActive { get; init; }
}

public sealed class GetPayrollQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? Period { get; init; }

    public PayrollStatus? Status { get; init; }

    public Guid? DeveloperId { get; init; }
}

public sealed class GetCashFlowQuery : IQuery
{
    public string? Period { get; init; }
}

public sealed class GetProfitLossQuery : IQuery
{
    public string? Period { get; init; }
}

public sealed class GetMonthlyIncomeQuery : IQuery
{
    public int Months { get; init; } = 6;
}

public sealed class GetExpensesQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public Guid? CategoryId { get; init; }

    public string? Period { get; init; }
}

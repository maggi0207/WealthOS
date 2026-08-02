using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Income.Entities;
using WealthOS.Domain.Income.Enums;

namespace WealthOS.Domain.Income.Repositories;

public interface ISalaryRepository : IRepository<Salary>
{
    Task<Salary?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Salary> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<decimal> SumPaymentsForPeriodAsync(
        Guid userId,
        string period,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<(string Period, decimal Amount)>> GetMonthlySalaryTotalsAsync(
        Guid userId,
        string fromPeriod,
        string toPeriod,
        CancellationToken cancellationToken = default);
}

public interface IBusinessClientRepository : IRepository<BusinessClient>
{
    Task<BusinessClient?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<BusinessClient> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        ClientStatus? status,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}

public interface IBusinessProjectRepository : IRepository<BusinessProject>
{
    Task<BusinessProject?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<BusinessProject?> GetByIdWithDevelopersAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<BusinessProject> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        Guid? clientId,
        ProjectStatus? status,
        string? search,
        CancellationToken cancellationToken = default);
}

public interface IDeveloperRepository : IRepository<Developer>
{
    Task<Developer?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Developer> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<DeveloperPayroll> Items, int TotalCount)> ListPayrollForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? period,
        PayrollStatus? status,
        Guid? developerId,
        CancellationToken cancellationToken = default);

    Task<decimal> SumPayrollForPeriodAsync(
        Guid userId,
        string period,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<Invoice?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<Invoice?> GetByIdWithDetailsAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Invoice> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        Guid? clientId,
        InvoiceStatus? status,
        string? search,
        CancellationToken cancellationToken = default);

    Task<decimal> SumOutstandingAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<decimal> SumPaymentsForPeriodAsync(
        Guid userId,
        string period,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<(string Period, decimal Amount)>> GetMonthlyRevenueTotalsAsync(
        Guid userId,
        string fromPeriod,
        string toPeriod,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<Guid, (decimal Outstanding, decimal LastPaymentAmount, DateOnly? LastPaymentOn)>>
        GetClientPaymentStatsAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IBusinessExpenseRepository : IRepository<BusinessExpense>
{
    Task<BusinessExpense?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<BusinessExpense> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        Guid? categoryId,
        string? period,
        CancellationToken cancellationToken = default);

    Task<decimal> SumForPeriodAsync(
        Guid userId,
        string period,
        CancellationToken cancellationToken = default);

    Task<ExpenseCategory?> GetCategoryByIdForUserAsync(
        Guid categoryId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<ExpenseCategory?> FindCategoryByNameAsync(
        Guid userId,
        string name,
        CancellationToken cancellationToken = default);

    Task AddCategoryAsync(ExpenseCategory category, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExpenseCategory>> ListCategoriesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

public interface IIncomeSourceRepository : IRepository<IncomeSource>
{
    Task<(IReadOnlyList<IncomeSource> Items, int TotalCount)> ListForUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

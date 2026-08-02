using WealthOS.Domain.Income.Enums;

namespace WealthOS.Application.Income.DTOs.Responses;

public sealed class IncomeDashboardResponse
{
    public string Period { get; init; } = string.Empty;

    public decimal MonthlyIncome { get; init; }

    public decimal BusinessRevenue { get; init; }

    public decimal Salary { get; init; }

    public decimal DeveloperCost { get; init; }

    public decimal BusinessExpenses { get; init; }

    public decimal OutstandingInvoices { get; init; }

    public decimal NetProfit { get; init; }

    public decimal CashAvailable { get; init; }

    public decimal SavingsRatePercent { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

public sealed class CashFlowResponse
{
    public string Period { get; init; } = string.Empty;

    public string PeriodLabel { get; init; } = string.Empty;

    public decimal SalaryIncome { get; init; }

    public decimal BusinessRevenue { get; init; }

    public decimal DeveloperPayroll { get; init; }

    public decimal BusinessExpenses { get; init; }

    public decimal PersonalOutflow { get; init; }

    public decimal NetCashFlow { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

public sealed class ProfitLossResponse
{
    public string Period { get; init; } = string.Empty;

    public decimal BusinessRevenue { get; init; }

    public decimal DeveloperCost { get; init; }

    public decimal BusinessExpenses { get; init; }

    public decimal GrossProfit { get; init; }

    public decimal NetProfit { get; init; }

    public decimal SalaryIncome { get; init; }

    public decimal TotalIncome { get; init; }

    public decimal CashAvailable { get; init; }

    public decimal SavingsRatePercent { get; init; }

    public string CurrencyCode { get; init; } = "INR";
}

public sealed class MonthlyIncomePointResponse
{
    public string Label { get; init; } = string.Empty;

    public string Period { get; init; } = string.Empty;

    public decimal Salary { get; init; }

    public decimal Business { get; init; }
}

public sealed class MonthlyIncomeTrendResponse
{
    public IReadOnlyList<MonthlyIncomePointResponse> Points { get; init; } = [];
}

public sealed class ClientResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Engagement { get; init; } = string.Empty;

    public ClientStatus Status { get; init; }

    public decimal MonthlyRevenue { get; init; }

    public decimal OutstandingInvoice { get; init; }

    public decimal LastPaymentAmount { get; init; }

    public DateOnly? LastPaymentOn { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public string? ContactEmail { get; init; }

    public string? ContactPhone { get; init; }

    public string? Notes { get; init; }
}

public sealed class ClientListResponse
{
    public IReadOnlyList<ClientResponse> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }
}

public sealed class ProjectResponse
{
    public Guid Id { get; init; }

    public Guid ClientId { get; init; }

    public string ClientName { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public ProjectStatus Status { get; init; }

    public DateOnly StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    public decimal? MonthlyRevenue { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public IReadOnlyList<ProjectDeveloperResponse> Developers { get; init; } = [];
}

public sealed class ProjectDeveloperResponse
{
    public Guid DeveloperId { get; init; }

    public string DeveloperName { get; init; } = string.Empty;

    public string? RoleOnProject { get; init; }

    public DateOnly AssignedOn { get; init; }

    public bool IsActive { get; init; }
}

public sealed class ProjectListResponse
{
    public IReadOnlyList<ProjectResponse> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }
}

public sealed class InvoiceItemResponse
{
    public Guid Id { get; init; }

    public string Description { get; init; } = string.Empty;

    public decimal Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal LineTotal { get; init; }
}

public sealed class InvoicePaymentResponse
{
    public Guid Id { get; init; }

    public Guid InvoiceId { get; init; }

    public decimal Amount { get; init; }

    public DateOnly PaidOn { get; init; }

    public PaymentMethod Method { get; init; }

    public string? Reference { get; init; }

    public string? Notes { get; init; }
}

public sealed class InvoiceResponse
{
    public Guid Id { get; init; }

    public Guid ClientId { get; init; }

    public string ClientName { get; init; } = string.Empty;

    public Guid? ProjectId { get; init; }

    public string InvoiceNumber { get; init; } = string.Empty;

    public DateOnly IssueDate { get; init; }

    public DateOnly DueDate { get; init; }

    public InvoiceStatus Status { get; init; }

    public decimal SubTotal { get; init; }

    public decimal AmountPaid { get; init; }

    public decimal OutstandingAmount { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public string? Notes { get; init; }

    public IReadOnlyList<InvoiceItemResponse> Items { get; init; } = [];

    public IReadOnlyList<InvoicePaymentResponse> Payments { get; init; } = [];
}

public sealed class InvoiceListResponse
{
    public IReadOnlyList<InvoiceResponse> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }
}

public sealed class DeveloperResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public decimal MonthlySalary { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public Guid? PrimaryClientId { get; init; }

    public string? PrimaryClientName { get; init; }

    public bool IsActive { get; init; }

    public string? Notes { get; init; }
}

public sealed class DeveloperListResponse
{
    public IReadOnlyList<DeveloperResponse> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }
}

public sealed class PayrollResponse
{
    public Guid Id { get; init; }

    public Guid DeveloperId { get; init; }

    public string DeveloperName { get; init; } = string.Empty;

    public decimal Amount { get; init; }

    public string Period { get; init; } = string.Empty;

    public PayrollStatus Status { get; init; }

    public DateOnly? PaidOn { get; init; }

    public DateOnly? ScheduledOn { get; init; }

    public string? Notes { get; init; }
}

public sealed class PayrollListResponse
{
    public IReadOnlyList<PayrollResponse> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }
}

public sealed class ExpenseResponse
{
    public Guid Id { get; init; }

    public Guid CategoryId { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public string Vendor { get; init; } = string.Empty;

    public decimal Amount { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public DateOnly PaidOn { get; init; }

    public bool IsRecurring { get; init; }

    public string? Period { get; init; }

    public string? Notes { get; init; }
}

public sealed class ExpenseListResponse
{
    public IReadOnlyList<ExpenseResponse> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }
}

public sealed class SalaryResponse
{
    public Guid Id { get; init; }

    public string MemberName { get; init; } = string.Empty;

    public string Employer { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public decimal MonthlyAmount { get; init; }

    public string CurrencyCode { get; init; } = "INR";

    public DateOnly? LastCreditedOn { get; init; }

    public DateOnly? NextExpectedOn { get; init; }

    public SalaryStatus Status { get; init; }

    public Guid? PaymentId { get; init; }

    public string? Notes { get; init; }
}

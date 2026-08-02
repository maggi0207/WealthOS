using WealthOS.Domain.Income.Enums;

namespace WealthOS.Application.Income.DTOs.Requests;

public sealed class CreateClientRequest
{
    public string Name { get; set; } = string.Empty;

    public string Engagement { get; set; } = string.Empty;

    public ClientStatus Status { get; set; } = ClientStatus.Active;

    public decimal MonthlyRevenue { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string? Notes { get; set; }
}

public sealed class UpdateClientRequest
{
    public string Name { get; set; } = string.Empty;

    public string Engagement { get; set; } = string.Empty;

    public ClientStatus Status { get; set; }

    public decimal MonthlyRevenue { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string? Notes { get; set; }
}

public sealed class CreateProjectRequest
{
    public Guid ClientId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal? MonthlyRevenue { get; set; }

    public string CurrencyCode { get; set; } = "INR";
}

public sealed class AssignDeveloperRequest
{
    public Guid ProjectId { get; set; }

    public Guid DeveloperId { get; set; }

    public DateOnly AssignedOn { get; set; }

    public string? RoleOnProject { get; set; }
}

public sealed class CreateInvoiceRequest
{
    public Guid ClientId { get; set; }

    public Guid? ProjectId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public DateOnly IssueDate { get; set; }

    public DateOnly DueDate { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Sent;

    public string CurrencyCode { get; set; } = "INR";

    public string? Notes { get; set; }

    public List<CreateInvoiceItemRequest> Items { get; set; } = [];
}

public sealed class CreateInvoiceItemRequest
{
    public string Description { get; set; } = string.Empty;

    public decimal Quantity { get; set; } = 1m;

    public decimal UnitPrice { get; set; }
}

public sealed class RecordInvoicePaymentRequest
{
    public Guid InvoiceId { get; set; }

    public decimal Amount { get; set; }

    public DateOnly PaidOn { get; set; }

    public PaymentMethod Method { get; set; } = PaymentMethod.BankTransfer;

    public string? Reference { get; set; }

    public string? Notes { get; set; }
}

public sealed class CreateExpenseRequest
{
    public Guid? CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string Vendor { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public DateOnly PaidOn { get; set; }

    public bool IsRecurring { get; set; }

    public string? Notes { get; set; }
}

public sealed class RecordSalaryRequest
{
    public Guid? SalaryId { get; set; }

    public string MemberName { get; set; } = string.Empty;

    public string Employer { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public decimal MonthlyAmount { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public DateOnly PaidOn { get; set; }

    public string Period { get; set; } = string.Empty;

    public SalaryStatus Status { get; set; } = SalaryStatus.Active;

    public DateOnly? NextExpectedOn { get; set; }

    public string? Notes { get; set; }
}

public sealed class CreateDeveloperRequest
{
    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public decimal MonthlySalary { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public Guid? PrimaryClientId { get; set; }

    public string? Notes { get; set; }
}

public sealed class CreatePayrollRequest
{
    public Guid DeveloperId { get; set; }

    public decimal Amount { get; set; }

    public string Period { get; set; } = string.Empty;

    public PayrollStatus Status { get; set; } = PayrollStatus.Pending;

    public DateOnly? PaidOn { get; set; }

    public DateOnly? ScheduledOn { get; set; }

    public string? Notes { get; set; }
}

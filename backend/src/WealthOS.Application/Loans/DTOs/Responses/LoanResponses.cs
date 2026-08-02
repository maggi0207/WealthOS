using WealthOS.Domain.Loans.Enums;

namespace WealthOS.Application.Loans.DTOs.Responses;

/// <summary>
/// Lender summary on loan responses.
/// </summary>
public sealed class LoanProviderResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Code { get; set; }
}

/// <summary>
/// Payment history entry.
/// </summary>
public sealed class LoanPaymentResponse
{
    public Guid Id { get; set; }

    public DateOnly PaidOn { get; set; }

    public decimal Amount { get; set; }

    public decimal PrincipalComponent { get; set; }

    public decimal InterestComponent { get; set; }

    public LoanPaymentStatus Status { get; set; }

    public string? PaymentMode { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }

    public bool IsPrepayment { get; set; }
}

/// <summary>
/// Reminder entry.
/// </summary>
public sealed class LoanReminderResponse
{
    public Guid Id { get; set; }

    public Guid LoanId { get; set; }

    public string LoanName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Detail { get; set; }

    public DateOnly DueOn { get; set; }

    public decimal Amount { get; set; }

    public bool IsUrgent { get; set; }
}

/// <summary>
/// Interest rate history entry.
/// </summary>
public sealed class LoanInterestRateResponse
{
    public Guid Id { get; set; }

    public decimal RatePercent { get; set; }

    public InterestType InterestType { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public string? Reason { get; set; }
}

/// <summary>
/// Document link stub.
/// </summary>
public sealed class LoanDocumentLinkResponse
{
    public Guid Id { get; set; }

    public Guid DocumentId { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// Property link on a loan.
/// </summary>
public sealed class LoanPropertyLinkResponse
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }

    public bool IsPrimary { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// Full loan detail response with computed progress fields.
/// </summary>
public sealed class LoanResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public LoanType Type { get; set; }

    public string LenderName { get; set; } = string.Empty;

    public Guid? LoanProviderId { get; set; }

    public LoanProviderResponse? LoanProvider { get; set; }

    public string? AccountNumber { get; set; }

    public decimal Principal { get; set; }

    public decimal OutstandingBalance { get; set; }

    public decimal InterestRate { get; set; }

    public InterestType InterestType { get; set; }

    public decimal EmiAmount { get; set; }

    public int TenureMonths { get; set; }

    public int RemainingTenureMonths { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateOnly? NextEmiDate { get; set; }

    public PaymentFrequency PaymentFrequency { get; set; }

    public LoanStatus Status { get; set; }

    public Guid? LinkedPropertyId { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public bool AutoDebit { get; set; }

    public string? Notes { get; set; }

    public decimal TotalPrincipalPaid { get; set; }

    public decimal TotalInterestPaid { get; set; }

    public decimal LoanProgressPercent { get; set; }

    public decimal EmiProgressPercent { get; set; }

    public IReadOnlyList<LoanPaymentResponse> Payments { get; set; } = [];

    public IReadOnlyList<LoanReminderResponse> Reminders { get; set; } = [];

    public IReadOnlyList<LoanInterestRateResponse> InterestRates { get; set; } = [];

    public IReadOnlyList<LoanDocumentLinkResponse> DocumentLinks { get; set; } = [];

    public IReadOnlyList<LoanPropertyLinkResponse> PropertyLinks { get; set; } = [];

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Lightweight list item for loan collections.
/// </summary>
public sealed class LoanListItemResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public LoanType Type { get; set; }

    public LoanStatus Status { get; set; }

    public string LenderName { get; set; } = string.Empty;

    public decimal Principal { get; set; }

    public decimal OutstandingBalance { get; set; }

    public decimal EmiAmount { get; set; }

    public decimal InterestRate { get; set; }

    public int RemainingTenureMonths { get; set; }

    public DateOnly? NextEmiDate { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public decimal LoanProgressPercent { get; set; }

    public Guid? LinkedPropertyId { get; set; }
}

/// <summary>
/// Paginated loan list payload.
/// </summary>
public sealed class LoanListResponse
{
    public IReadOnlyList<LoanListItemResponse> Items { get; set; } = [];

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}

/// <summary>
/// Portfolio-level loan summary.
/// </summary>
public sealed class LoanSummaryResponse
{
    public int LoanCount { get; set; }

    public decimal TotalLoanAmount { get; set; }

    public decimal OutstandingBalance { get; set; }

    public decimal MonthlyEmi { get; set; }

    public decimal UpcomingEmi { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public int ActiveCount { get; set; }

    public int ClosedCount { get; set; }
}

/// <summary>
/// Upcoming EMI / reminder list.
/// </summary>
public sealed class UpcomingPaymentsResponse
{
    public IReadOnlyList<LoanReminderResponse> Items { get; set; } = [];

    public decimal TotalUpcomingAmount { get; set; }

    public string CurrencyCode { get; set; } = "INR";
}

/// <summary>
/// Per-loan dashboard snapshot.
/// </summary>
public sealed class LoanDashboardResponse
{
    public LoanResponse Loan { get; set; } = null!;

    public decimal TotalPrincipalPaid { get; set; }

    public decimal TotalInterestPaid { get; set; }

    public decimal LoanProgressPercent { get; set; }

    public decimal EmiProgressPercent { get; set; }

    public int PaymentCount { get; set; }

    public int ReminderCount { get; set; }

    public int DocumentLinkCount { get; set; }

    public int PropertyLinkCount { get; set; }

    public LoanPrepaymentScenarioResponse? SamplePrepayment { get; set; }

    public DateTime GeneratedAt { get; set; }
}

/// <summary>
/// Prepayment scenario DTO (extension point for future calculators).
/// </summary>
public sealed class LoanPrepaymentScenarioResponse
{
    public Guid LoanId { get; set; }

    public decimal LumpSum { get; set; }

    public decimal CurrentOutstanding { get; set; }

    public decimal NewOutstanding { get; set; }

    public int CurrentRemainingMonths { get; set; }

    public int EstimatedRemainingMonths { get; set; }

    public int MonthsSaved { get; set; }

    public decimal EstimatedInterestSaved { get; set; }

    public string CalculatorKey { get; set; } = "simple-emi";
}

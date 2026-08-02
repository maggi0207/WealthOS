using WealthOS.Domain.Loans.Enums;

namespace WealthOS.Application.Loans.DTOs.Requests;

/// <summary>
/// Request to create a new loan.
/// </summary>
public sealed class CreateLoanRequest
{
    public string Name { get; set; } = string.Empty;

    public LoanType Type { get; set; }

    public string LenderName { get; set; } = string.Empty;

    public Guid? LoanProviderId { get; set; }

    public string? AccountNumber { get; set; }

    public decimal Principal { get; set; }

    public decimal OutstandingBalance { get; set; }

    public decimal InterestRate { get; set; }

    public InterestType InterestType { get; set; } = InterestType.Fixed;

    public decimal EmiAmount { get; set; }

    public int TenureMonths { get; set; }

    public int RemainingTenureMonths { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateOnly? NextEmiDate { get; set; }

    public PaymentFrequency PaymentFrequency { get; set; } = PaymentFrequency.Monthly;

    public LoanStatus Status { get; set; } = LoanStatus.Active;

    public Guid? LinkedPropertyId { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public bool AutoDebit { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// Request to update an existing loan.
/// </summary>
public sealed class UpdateLoanRequest
{
    public string Name { get; set; } = string.Empty;

    public LoanType Type { get; set; }

    public string LenderName { get; set; } = string.Empty;

    public Guid? LoanProviderId { get; set; }

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
}

/// <summary>
/// Request to record a payment against a loan.
/// </summary>
public sealed class RecordLoanPaymentRequest
{
    public DateOnly PaidOn { get; set; }

    public decimal Amount { get; set; }

    public decimal PrincipalComponent { get; set; }

    public decimal InterestComponent { get; set; }

    public LoanPaymentStatus Status { get; set; } = LoanPaymentStatus.Paid;

    public string? PaymentMode { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }

    public bool IsPrepayment { get; set; }

    /// <summary>
    /// When true and status is Paid, reduces outstanding by principal component and may bump next EMI date.
    /// </summary>
    public bool ApplyToOutstanding { get; set; } = true;
}

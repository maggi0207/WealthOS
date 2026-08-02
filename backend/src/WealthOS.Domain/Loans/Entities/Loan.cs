using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Loans.Enums;
using WealthOS.Domain.Properties.Entities;

namespace WealthOS.Domain.Loans.Entities;

/// <summary>
/// Aggregate root for a loan account. Soft-deleted via <see cref="AuditableEntity"/>.
/// Optionally linked to a property; a property may have multiple loans later.
/// </summary>
public sealed class Loan : AuditableEntity
{
    public Loan()
    {
    }

    public Loan(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public LoanType Type { get; set; }

    /// <summary>
    /// Denormalized lender display name (kept even when <see cref="LoanProviderId"/> is set).
    /// </summary>
    public string LenderName { get; set; } = string.Empty;

    public Guid? LoanProviderId { get; set; }

    public LoanProvider? LoanProvider { get; set; }

    public string? AccountNumber { get; set; }

    public decimal Principal { get; set; }

    public decimal OutstandingBalance { get; set; }

    /// <summary>
    /// Annual interest rate as a percentage (e.g. 8.6 for 8.6%).
    /// </summary>
    public decimal InterestRate { get; set; }

    public InterestType InterestType { get; set; } = InterestType.Fixed;

    public decimal EmiAmount { get; set; }

    /// <summary>
    /// Original tenure in months.
    /// </summary>
    public int TenureMonths { get; set; }

    /// <summary>
    /// Remaining tenure in months (simple field; complex amortisation schedules are future work).
    /// </summary>
    public int RemainingTenureMonths { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateOnly? NextEmiDate { get; set; }

    public PaymentFrequency PaymentFrequency { get; set; } = PaymentFrequency.Monthly;

    public LoanStatus Status { get; set; } = LoanStatus.Active;

    /// <summary>
    /// Optional primary linked property (convenience FK). Prefer <see cref="PropertyLinks"/> for multi-link.
    /// </summary>
    public Guid? LinkedPropertyId { get; set; }

    public Property? LinkedProperty { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public bool AutoDebit { get; set; }

    public string? Notes { get; set; }

    public ICollection<LoanPayment> Payments { get; set; } = new List<LoanPayment>();

    public ICollection<LoanSchedule> Schedules { get; set; } = new List<LoanSchedule>();

    public ICollection<LoanReminder> Reminders { get; set; } = new List<LoanReminder>();

    public ICollection<LoanInterestRate> InterestRates { get; set; } = new List<LoanInterestRate>();

    public ICollection<LoanDocumentLink> DocumentLinks { get; set; } = new List<LoanDocumentLink>();

    public ICollection<LoanPropertyLink> PropertyLinks { get; set; } = new List<LoanPropertyLink>();
}

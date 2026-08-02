using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Loans.Entities;

/// <summary>
/// Planned instalment row. Complex amortisation generation is deferred; rows may be seeded or added later.
/// </summary>
public sealed class LoanSchedule : AuditableEntity
{
    public Guid LoanId { get; set; }

    public Loan Loan { get; set; } = null!;

    public int InstalmentNumber { get; set; }

    public DateOnly DueDate { get; set; }

    public decimal EmiAmount { get; set; }

    public decimal PrincipalComponent { get; set; }

    public decimal InterestComponent { get; set; }

    public decimal OpeningBalance { get; set; }

    public decimal ClosingBalance { get; set; }

    public bool IsPaid { get; set; }

    public Guid? LoanPaymentId { get; set; }

    public LoanPayment? LoanPayment { get; set; }
}

using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Loans.Entities;

/// <summary>
/// Link from a loan to a future Documents module record (no Documents FK yet).
/// </summary>
public sealed class LoanDocumentLink : AuditableEntity
{
    public Guid LoanId { get; set; }

    public Loan Loan { get; set; } = null!;

    public Guid DocumentId { get; set; }

    public string? Notes { get; set; }
}

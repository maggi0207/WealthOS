using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Loans.Entities;

namespace WealthOS.Domain.Properties.Entities;

/// <summary>
/// Link from a property to a loan. FK to <see cref="Loan"/> wired in Phase 5.
/// </summary>
public sealed class PropertyLoanLink : AuditableEntity
{
    public Guid PropertyId { get; set; }

    public Property Property { get; set; } = null!;

    public Guid LoanId { get; set; }

    public Loan? Loan { get; set; }

    public string? Notes { get; set; }
}

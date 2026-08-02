using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Properties.Entities;

namespace WealthOS.Domain.Loans.Entities;

/// <summary>
/// Join between a loan and a property. A property can have multiple loans later.
/// </summary>
public sealed class LoanPropertyLink : AuditableEntity
{
    public Guid LoanId { get; set; }

    public Loan Loan { get; set; } = null!;

    public Guid PropertyId { get; set; }

    public Property Property { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public string? Notes { get; set; }
}

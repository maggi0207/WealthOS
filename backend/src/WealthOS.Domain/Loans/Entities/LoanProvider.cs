using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Loans.Entities;

/// <summary>
/// Lender / financial institution. Future-ready for multiple lenders and refinancing.
/// </summary>
public sealed class LoanProvider : AuditableEntity
{
    public LoanProvider()
    {
    }

    public LoanProvider(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Code { get; set; }

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    public string? Website { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}

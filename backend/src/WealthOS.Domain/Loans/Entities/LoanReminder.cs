using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Loans.Entities;

/// <summary>
/// Upcoming EMI / payment reminder for a loan.
/// </summary>
public sealed class LoanReminder : AuditableEntity
{
    public Guid LoanId { get; set; }

    public Loan Loan { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string? Detail { get; set; }

    public DateOnly DueOn { get; set; }

    public decimal Amount { get; set; }

    public bool IsUrgent { get; set; }

    public bool IsDismissed { get; set; }
}

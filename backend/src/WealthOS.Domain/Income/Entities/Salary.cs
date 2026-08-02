using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Income.Enums;

namespace WealthOS.Domain.Income.Entities;

/// <summary>
/// Recurring personal salary definition for a household member.
/// Soft-deleted via <see cref="AuditableEntity"/>.
/// </summary>
public sealed class Salary : AuditableEntity
{
    public Salary()
    {
    }

    public Salary(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public string MemberName { get; set; } = string.Empty;

    public string Employer { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public decimal MonthlyAmount { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public DateOnly? LastCreditedOn { get; set; }

    public DateOnly? NextExpectedOn { get; set; }

    public SalaryStatus Status { get; set; } = SalaryStatus.Active;

    public string? Notes { get; set; }

    public ICollection<SalaryPayment> Payments { get; set; } = new List<SalaryPayment>();
}

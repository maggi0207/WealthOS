using WealthOS.Domain.Common.Entities;
using WealthOS.Domain.Investments.Enums;

namespace WealthOS.Domain.Investments.Entities;

/// <summary>
/// Corporate action affecting a holding (split, bonus, etc.).
/// </summary>
public sealed class CorporateAction : AuditableEntity
{
    public CorporateAction()
    {
    }

    public CorporateAction(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public Guid AccountId { get; set; }

    public Guid HoldingId { get; set; }

    public Holding? Holding { get; set; }

    public CorporateActionType ActionType { get; set; }

    public DateOnly EffectiveDate { get; set; }

    /// <summary>
    /// Optional ratio description (e.g. "1:1 bonus", "2-for-1 split").
    /// </summary>
    public string? Ratio { get; set; }

    public string Description { get; set; } = string.Empty;

    public string? Notes { get; set; }
}

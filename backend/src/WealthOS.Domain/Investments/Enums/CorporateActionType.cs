namespace WealthOS.Domain.Investments.Enums;

/// <summary>
/// Corporate action kinds affecting holdings.
/// </summary>
public enum CorporateActionType
{
    Split = 0,
    Bonus = 1,
    Merger = 2,
    Demerger = 3,
    Rights = 4,
    Buyback = 5,
    Other = 99,
}

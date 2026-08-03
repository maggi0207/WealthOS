namespace WealthOS.Domain.Assets.Enums;

/// <summary>
/// Manual asset categories that are not owned by Properties or Investments modules.
/// </summary>
public enum ManualAssetType
{
    PhysicalGold = 0,
    Cash = 1,
    BankBalance = 2,
    FixedDeposit = 3,
    Vehicle = 4,
    Jewellery = 5,
    Ppf = 6,
    Epf = 7,
    Nps = 8,
    Crypto = 9,
    Collectibles = 10,
    Other = 99,
}

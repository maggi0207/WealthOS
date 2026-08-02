namespace WealthOS.Domain.Investments.Enums;

/// <summary>
/// Types of investment cash / position movements.
/// </summary>
public enum InvestmentTransactionType
{
    Buy = 0,
    Sell = 1,
    Sip = 2,
    Dividend = 3,
    Interest = 4,
    TransferIn = 5,
    TransferOut = 6,
    CorporateAction = 7,
    Other = 99,
}

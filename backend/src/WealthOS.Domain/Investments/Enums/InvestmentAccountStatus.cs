namespace WealthOS.Domain.Investments.Enums;

/// <summary>
/// Connection / data-entry status for an investment account.
/// </summary>
public enum InvestmentAccountStatus
{
    Manual = 0,
    Connected = 1,
    ComingSoon = 2,
    Disconnected = 3,
}

namespace WealthOS.Domain.Investments.Enums;

/// <summary>
/// Instrument type within an investment category.
/// Stored as enum (not a lookup table) for Phase 7 simplicity.
/// </summary>
public enum InvestmentType
{
    Equity = 0,
    MutualFund = 1,
    Bond = 2,
    Etf = 3,
    Gold = 4,
    FixedDeposit = 5,
    Unlisted = 6,
    Cash = 7,
    Other = 99,
}

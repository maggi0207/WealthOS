namespace WealthOS.Domain.Investments.Enums;

/// <summary>
/// High-level asset category used for allocation views.
/// Stored as enum (not a lookup table) for Phase 7 simplicity.
/// </summary>
public enum InvestmentCategory
{
    Stocks = 0,
    MutualFunds = 1,
    CorporateBonds = 2,
    GoldEtfs = 3,
    Cash = 4,
    Other = 99,
}

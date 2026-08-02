namespace WealthOS.Domain.Investments.Enums;

/// <summary>
/// Supported investment provider kinds. Groww / Zerodha / Upstox / Custom are future-ready stubs.
/// </summary>
public enum ProviderKind
{
    Manual = 0,
    AngelOne = 1,
    IndiaBonds = 2,
    Groww = 3,
    Zerodha = 4,
    Upstox = 5,
    Custom = 99,
}

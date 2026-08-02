namespace WealthOS.Domain.Documents.Enums;

/// <summary>
/// Soft cross-module reference target. GUID-only — no cascade ownership.
/// </summary>
public enum DocumentReferenceModule
{
    None = 0,
    Property = 1,
    Loan = 2,
    Investment = 3,
    BusinessClient = 4,
    Goal = 5,
    Income = 6,
}

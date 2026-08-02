namespace WealthOS.Domain.Documents.Enums;

/// <summary>
/// Lifecycle / verification status for a vault document.
/// </summary>
public enum DocumentStatus
{
    Draft = 0,
    Pending = 1,
    Verified = 2,
    Expiring = 3,
    Expired = 4,
    Archived = 5,
}

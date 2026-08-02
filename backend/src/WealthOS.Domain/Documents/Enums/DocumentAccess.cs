namespace WealthOS.Domain.Documents.Enums;

/// <summary>
/// Access visibility for a document within the authenticated owner's vault.
/// Shared / Restricted are reserved for future multi-user collaboration.
/// </summary>
public enum DocumentAccess
{
    Private = 0,
    Shared = 1,
    Restricted = 2,
}

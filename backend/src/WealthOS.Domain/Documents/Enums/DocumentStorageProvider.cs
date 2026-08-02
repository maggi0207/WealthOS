namespace WealthOS.Domain.Documents.Enums;

/// <summary>
/// Storage backend placeholder. Phase 9 stores metadata only — no real file I/O.
/// </summary>
public enum DocumentStorageProvider
{
    None = 0,
    LocalPlaceholder = 1,
    AzureBlob = 2,
    S3 = 3,
}

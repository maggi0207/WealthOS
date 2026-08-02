namespace WealthOS.Application.Dashboard.Providers;

/// <summary>
/// Cross-module document counts. Implemented by Documents module later; mock in Phase 3.
/// </summary>
public interface IDocumentSummaryProvider
{
    Task<DocumentModuleSummary> GetSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Document module summary shape consumed by Dashboard.
/// </summary>
public sealed class DocumentModuleSummary
{
    public int DocumentCount { get; init; }

    public int PendingReviewCount { get; init; }
}

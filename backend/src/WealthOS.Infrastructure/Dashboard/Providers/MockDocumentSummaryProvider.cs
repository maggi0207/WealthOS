using WealthOS.Application.Dashboard.Providers;

namespace WealthOS.Infrastructure.Dashboard.Providers;

/// <summary>
/// Placeholder document counts until the Documents module exists.
/// </summary>
public sealed class MockDocumentSummaryProvider : IDocumentSummaryProvider
{
    public Task<DocumentModuleSummary> GetSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new DocumentModuleSummary
        {
            DocumentCount = 18,
            PendingReviewCount = 2,
        });
    }
}

using WealthOS.Application.Dashboard.Providers;
using WealthOS.Domain.Documents.Enums;
using WealthOS.Domain.Documents.Repositories;

namespace WealthOS.Infrastructure.Documents.Providers;

/// <summary>
/// Dashboard document counts backed by the Documents module repository.
/// </summary>
public sealed class DocumentSummaryProvider : IDocumentSummaryProvider
{
    private readonly IDocumentRepository _documentRepository;

    public DocumentSummaryProvider(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<DocumentModuleSummary> GetSummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var (_, totalCount) = await _documentRepository.ListForUserAsync(
            userId,
            page: 1,
            pageSize: 1,
            search: null,
            category: null,
            status: null,
            cancellationToken);

        var (_, pendingCount) = await _documentRepository.ListForUserAsync(
            userId,
            page: 1,
            pageSize: 1,
            search: null,
            category: null,
            status: DocumentStatus.Pending,
            cancellationToken);

        return new DocumentModuleSummary
        {
            DocumentCount = totalCount,
            PendingReviewCount = pendingCount,
        };
    }
}

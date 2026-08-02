using WealthOS.Application.Common.Models;
using WealthOS.Application.Documents.DTOs.Requests;
using WealthOS.Application.Documents.DTOs.Responses;
using WealthOS.Domain.Documents.Entities;
using WealthOS.Domain.Documents.Enums;

namespace WealthOS.Application.Documents.Services;

/// <summary>
/// Shared helpers for document orchestration (status, metadata, versions, references).
/// </summary>
internal static class DocumentServiceHelpers
{
    public static DocumentVersion CreateVersion(Document document, int versionNumber, string? notes) =>
        new()
        {
            VersionNumber = versionNumber,
            OriginalFileName = document.OriginalFileName,
            ContentType = document.ContentType,
            FileSizeBytes = document.FileSizeBytes,
            StorageProvider = document.StorageProvider,
            StoragePath = document.StoragePath,
            Notes = notes,
        };

    public static DocumentMetadata MapMetadata(UpsertDocumentMetadataRequest request) =>
        new()
        {
            DocumentNumber = request.DocumentNumber,
            IssuedBy = request.IssuedBy,
            IssuerCountry = request.IssuerCountry,
            Checksum = request.Checksum,
            PageCount = request.PageCount,
            CustomAttributesJson = request.CustomAttributesJson,
        };

    public static void ApplyMetadata(DocumentMetadata metadata, UpsertDocumentMetadataRequest request)
    {
        metadata.DocumentNumber = request.DocumentNumber;
        metadata.IssuedBy = request.IssuedBy;
        metadata.IssuerCountry = request.IssuerCountry;
        metadata.Checksum = request.Checksum;
        metadata.PageCount = request.PageCount;
        metadata.CustomAttributesJson = request.CustomAttributesJson;
    }

    public static Result ValidatePrimaryReference(
        DocumentReferenceModule module,
        Guid? referenceId)
    {
        if (module == DocumentReferenceModule.None)
        {
            return referenceId.HasValue
                ? Result.Failure(Error.Validation(
                    "Reference id requires a reference module.",
                    new Dictionary<string, string[]>
                    {
                        [nameof(CreateDocumentRequest.ReferenceId)] =
                            ["Reference id cannot be set when reference module is None."],
                    }))
                : Result.Success();
        }

        if (!referenceId.HasValue || referenceId.Value == Guid.Empty)
        {
            return Result.Failure(Error.Validation(
                "Reference id is required when a reference module is set.",
                new Dictionary<string, string[]>
                {
                    [nameof(CreateDocumentRequest.ReferenceId)] =
                        ["Reference id is required when a reference module is set."],
                }));
        }

        return Result.Success();
    }

    public static DocumentStatus ResolveStatus(DocumentStatus requested, DateOnly? expiryDate)
    {
        if (requested is DocumentStatus.Archived or DocumentStatus.Draft)
        {
            return requested;
        }

        if (!expiryDate.HasValue)
        {
            return requested;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (expiryDate.Value < today)
        {
            return DocumentStatus.Expired;
        }

        if (expiryDate.Value <= today.AddDays(30) && requested != DocumentStatus.Expired)
        {
            return DocumentStatus.Expiring;
        }

        return requested;
    }

    public static string NormalizeTag(string name) => name.Trim().ToLowerInvariant();

    public static string BuildPlaceholderPath(Guid userId, Guid documentId, string fileName) =>
        $"placeholder://documents/{userId:N}/{documentId:N}/{fileName.Trim()}";

    public static DocumentListResponse BuildListResponse(
        IReadOnlyList<DocumentListItemResponse> items,
        int page,
        int pageSize,
        int totalCount) =>
        new()
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize),
        };
}

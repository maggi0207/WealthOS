using WealthOS.Application.Assets.DTOs.Requests;
using WealthOS.Application.Assets.DTOs.Responses;
using WealthOS.Application.Common.Models;
using WealthOS.Domain.Assets.Enums;

namespace WealthOS.Application.Assets.Interfaces;

/// <summary>
/// Manual asset CRUD use cases.
/// </summary>
public interface IManualAssetService
{
    Task<Result<ManualAssetListResponse>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        ManualAssetType? type,
        CancellationToken cancellationToken = default);

    Task<Result<ManualAssetResponse>> GetByIdAsync(
        Guid assetId,
        CancellationToken cancellationToken = default);

    Task<Result<ManualAssetResponse>> CreateAsync(
        CreateManualAssetRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<ManualAssetResponse>> UpdateAsync(
        Guid assetId,
        UpdateManualAssetRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        Guid assetId,
        CancellationToken cancellationToken = default);
}

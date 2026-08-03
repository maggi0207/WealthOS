using WealthOS.Application.Assets.DTOs.Responses;
using WealthOS.Application.Assets.Interfaces;
using WealthOS.Application.Assets.Queries;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;

namespace WealthOS.Application.Assets.Queries.Handlers;

public sealed class GetManualAssetsQueryHandler
    : IQueryHandler<GetManualAssetsQuery, ManualAssetListResponse>
{
    private readonly IManualAssetService _service;

    public GetManualAssetsQueryHandler(IManualAssetService service)
    {
        _service = service;
    }

    public Task<Result<ManualAssetListResponse>> HandleAsync(
        GetManualAssetsQuery query,
        CancellationToken cancellationToken = default) =>
        _service.GetAllAsync(
            query.Page,
            query.PageSize,
            query.Search,
            query.Type,
            cancellationToken);
}

public sealed class GetManualAssetByIdQueryHandler
    : IQueryHandler<GetManualAssetByIdQuery, ManualAssetResponse>
{
    private readonly IManualAssetService _service;

    public GetManualAssetByIdQueryHandler(IManualAssetService service)
    {
        _service = service;
    }

    public Task<Result<ManualAssetResponse>> HandleAsync(
        GetManualAssetByIdQuery query,
        CancellationToken cancellationToken = default) =>
        _service.GetByIdAsync(query.AssetId, cancellationToken);
}

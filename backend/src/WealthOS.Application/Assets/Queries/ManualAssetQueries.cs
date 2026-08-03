using WealthOS.Application.Common.Abstractions;
using WealthOS.Domain.Assets.Enums;

namespace WealthOS.Application.Assets.Queries;

public sealed class GetManualAssetsQuery : IQuery
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 50;

    public string? Search { get; init; }

    public ManualAssetType? Type { get; init; }
}

public sealed class GetManualAssetByIdQuery : IQuery
{
    public Guid AssetId { get; init; }
}

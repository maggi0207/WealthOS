using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Assets.Commands;
using WealthOS.Application.Assets.Commands.Handlers;
using WealthOS.Application.Assets.DTOs.Responses;
using WealthOS.Application.Assets.Interfaces;
using WealthOS.Application.Assets.Queries;
using WealthOS.Application.Assets.Queries.Handlers;
using WealthOS.Application.Assets.Services;
using WealthOS.Application.Common.Abstractions;

namespace WealthOS.Application.Assets;

/// <summary>
/// Registers Assets (manual assets) application services and CQRS handlers.
/// </summary>
public static class AssetServiceCollectionExtensions
{
    public static IServiceCollection AddAssetsApplication(this IServiceCollection services)
    {
        services.AddScoped<IManualAssetService, ManualAssetService>();

        services.AddScoped<
            ICommandHandler<CreateManualAssetCommand, ManualAssetResponse>,
            CreateManualAssetCommandHandler>();
        services.AddScoped<
            ICommandHandler<UpdateManualAssetCommand, ManualAssetResponse>,
            UpdateManualAssetCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteManualAssetCommand>, DeleteManualAssetCommandHandler>();

        services.AddScoped<
            IQueryHandler<GetManualAssetsQuery, ManualAssetListResponse>,
            GetManualAssetsQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetManualAssetByIdQuery, ManualAssetResponse>,
            GetManualAssetByIdQueryHandler>();

        return services;
    }
}

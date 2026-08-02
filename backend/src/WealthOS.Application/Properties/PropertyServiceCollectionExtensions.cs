using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Properties.Commands;
using WealthOS.Application.Properties.Commands.Handlers;
using WealthOS.Application.Properties.DTOs.Responses;
using WealthOS.Application.Properties.Interfaces;
using WealthOS.Application.Properties.Queries;
using WealthOS.Application.Properties.Queries.Handlers;
using WealthOS.Application.Properties.Services;

namespace WealthOS.Application.Properties;

/// <summary>
/// Registers Properties application services and CQRS handlers.
/// </summary>
public static class PropertyServiceCollectionExtensions
{
    public static IServiceCollection AddPropertiesApplication(this IServiceCollection services)
    {
        services.AddScoped<IPropertyService, PropertyService>();

        services.AddScoped<
            ICommandHandler<CreatePropertyCommand, PropertyResponse>,
            CreatePropertyCommandHandler>();

        services.AddScoped<
            ICommandHandler<UpdatePropertyCommand, PropertyResponse>,
            UpdatePropertyCommandHandler>();

        services.AddScoped<
            ICommandHandler<DeletePropertyCommand>,
            DeletePropertyCommandHandler>();

        services.AddScoped<
            IQueryHandler<GetPropertyByIdQuery, PropertyResponse>,
            GetPropertyByIdQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetAllPropertiesQuery, PropertyListResponse>,
            GetAllPropertiesQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetPropertySummaryQuery, PropertySummaryResponse>,
            GetPropertySummaryQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetPropertyDashboardQuery, PropertyDashboardResponse>,
            GetPropertyDashboardQueryHandler>();

        return services;
    }
}

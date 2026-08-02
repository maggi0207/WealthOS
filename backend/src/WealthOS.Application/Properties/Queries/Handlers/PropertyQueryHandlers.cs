using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Properties.DTOs.Responses;
using WealthOS.Application.Properties.Interfaces;
using WealthOS.Application.Properties.Queries;

namespace WealthOS.Application.Properties.Queries.Handlers;

public sealed class GetPropertyByIdQueryHandler
    : IQueryHandler<GetPropertyByIdQuery, PropertyResponse>
{
    private readonly IPropertyService _propertyService;

    public GetPropertyByIdQueryHandler(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    public Task<Result<PropertyResponse>> HandleAsync(
        GetPropertyByIdQuery query,
        CancellationToken cancellationToken = default) =>
        _propertyService.GetByIdAsync(query.PropertyId, cancellationToken);
}

public sealed class GetAllPropertiesQueryHandler
    : IQueryHandler<GetAllPropertiesQuery, PropertyListResponse>
{
    private readonly IPropertyService _propertyService;

    public GetAllPropertiesQueryHandler(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    public Task<Result<PropertyListResponse>> HandleAsync(
        GetAllPropertiesQuery query,
        CancellationToken cancellationToken = default) =>
        _propertyService.GetAllAsync(
            query.Page,
            query.PageSize,
            query.Search,
            query.Status,
            query.Type,
            cancellationToken);
}

public sealed class GetPropertySummaryQueryHandler
    : IQueryHandler<GetPropertySummaryQuery, PropertySummaryResponse>
{
    private readonly IPropertyService _propertyService;

    public GetPropertySummaryQueryHandler(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    public Task<Result<PropertySummaryResponse>> HandleAsync(
        GetPropertySummaryQuery query,
        CancellationToken cancellationToken = default) =>
        _propertyService.GetSummaryAsync(cancellationToken);
}

public sealed class GetPropertyDashboardQueryHandler
    : IQueryHandler<GetPropertyDashboardQuery, PropertyDashboardResponse>
{
    private readonly IPropertyService _propertyService;

    public GetPropertyDashboardQueryHandler(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    public Task<Result<PropertyDashboardResponse>> HandleAsync(
        GetPropertyDashboardQuery query,
        CancellationToken cancellationToken = default) =>
        _propertyService.GetDashboardAsync(query.PropertyId, cancellationToken);
}

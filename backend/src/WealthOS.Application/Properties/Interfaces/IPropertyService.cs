using WealthOS.Application.Common.Models;
using WealthOS.Application.Properties.DTOs.Requests;
using WealthOS.Application.Properties.DTOs.Responses;
using WealthOS.Domain.Properties.Enums;

namespace WealthOS.Application.Properties.Interfaces;

/// <summary>
/// Application service for property portfolio management.
/// </summary>
public interface IPropertyService
{
    Task<Result<PropertyResponse>> CreateAsync(
        CreatePropertyRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<PropertyResponse>> UpdateAsync(
        Guid propertyId,
        UpdatePropertyRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Guid propertyId, CancellationToken cancellationToken = default);

    Task<Result<PropertyResponse>> GetByIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    Task<Result<PropertyListResponse>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        PropertyStatus? status,
        PropertyType? type,
        CancellationToken cancellationToken = default);

    Task<Result<PropertySummaryResponse>> GetSummaryAsync(CancellationToken cancellationToken = default);

    Task<Result<PropertyDashboardResponse>> GetDashboardAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);
}

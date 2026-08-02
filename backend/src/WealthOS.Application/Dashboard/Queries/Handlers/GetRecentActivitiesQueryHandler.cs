using FluentValidation;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.DTOs.Responses;
using WealthOS.Application.Dashboard.Interfaces;
using WealthOS.Application.Dashboard.Queries;

namespace WealthOS.Application.Dashboard.Queries.Handlers;

/// <summary>
/// Handles <see cref="GetRecentActivitiesQuery"/>.
/// </summary>
public sealed class GetRecentActivitiesQueryHandler
    : IQueryHandler<GetRecentActivitiesQuery, IReadOnlyList<RecentActivityResponse>>
{
    private readonly IDashboardService _dashboardService;
    private readonly IValidator<GetRecentActivitiesQuery> _validator;

    public GetRecentActivitiesQueryHandler(
        IDashboardService dashboardService,
        IValidator<GetRecentActivitiesQuery> validator)
    {
        _dashboardService = dashboardService;
        _validator = validator;
    }

    public async Task<Result<IReadOnlyList<RecentActivityResponse>>> HandleAsync(
        GetRecentActivitiesQuery query,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .GroupBy(failure => failure.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(failure => failure.ErrorMessage).ToArray());

            return Result.Failure<IReadOnlyList<RecentActivityResponse>>(
                Error.Validation("Validation failed.", errors));
        }

        return await _dashboardService.GetRecentActivitiesAsync(query.Limit, cancellationToken);
    }
}

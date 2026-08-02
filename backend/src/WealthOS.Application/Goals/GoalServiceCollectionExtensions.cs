using Microsoft.Extensions.DependencyInjection;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Goals.Calculations;
using WealthOS.Application.Goals.Commands;
using WealthOS.Application.Goals.Commands.Handlers;
using WealthOS.Application.Goals.DTOs.Responses;
using WealthOS.Application.Goals.Interfaces;
using WealthOS.Application.Goals.Queries;
using WealthOS.Application.Goals.Queries.Handlers;
using WealthOS.Application.Goals.Services;

namespace WealthOS.Application.Goals;

/// <summary>
/// Registers Goals application services and CQRS handlers.
/// </summary>
public static class GoalServiceCollectionExtensions
{
    public static IServiceCollection AddGoalsApplication(this IServiceCollection services)
    {
        services.AddScoped<IGoalCalculationService, GoalCalculationService>();
        services.AddScoped<IGoalService, GoalService>();
        services.AddScoped<IGoalProjectionService, GoalProjectionService>();

        services.AddScoped<ICommandHandler<CreateGoalCommand, GoalResponse>, CreateGoalCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateGoalCommand, GoalResponse>, UpdateGoalCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteGoalCommand>, DeleteGoalCommandHandler>();
        services.AddScoped<
            ICommandHandler<RecordContributionCommand, GoalContributionResponse>,
            RecordContributionCommandHandler>();
        services.AddScoped<
            ICommandHandler<CompleteMilestoneCommand, GoalMilestoneResponse>,
            CompleteMilestoneCommandHandler>();

        services.AddScoped<IQueryHandler<GetGoalsQuery, GoalListResponse>, GetGoalsQueryHandler>();
        services.AddScoped<IQueryHandler<GetGoalByIdQuery, GoalResponse>, GetGoalByIdQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetGoalProgressQuery, GoalProgressResponse>,
            GetGoalProgressQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetGoalDashboardQuery, GoalDashboardResponse>,
            GetGoalDashboardQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetGoalProjectionQuery, GoalProjectionResponse>,
            GetGoalProjectionQueryHandler>();

        return services;
    }
}

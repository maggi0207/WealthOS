using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Goals.Commands;
using WealthOS.Application.Goals.DTOs.Responses;
using WealthOS.Application.Goals.Interfaces;

namespace WealthOS.Application.Goals.Commands.Handlers;

public sealed class CreateGoalCommandHandler
    : ICommandHandler<CreateGoalCommand, GoalResponse>
{
    private readonly IGoalService _goalService;

    public CreateGoalCommandHandler(IGoalService goalService)
    {
        _goalService = goalService;
    }

    public Task<Result<GoalResponse>> HandleAsync(
        CreateGoalCommand command,
        CancellationToken cancellationToken = default) =>
        _goalService.CreateAsync(command.Request, cancellationToken);
}

public sealed class UpdateGoalCommandHandler
    : ICommandHandler<UpdateGoalCommand, GoalResponse>
{
    private readonly IGoalService _goalService;

    public UpdateGoalCommandHandler(IGoalService goalService)
    {
        _goalService = goalService;
    }

    public Task<Result<GoalResponse>> HandleAsync(
        UpdateGoalCommand command,
        CancellationToken cancellationToken = default) =>
        _goalService.UpdateAsync(command.GoalId, command.Request, cancellationToken);
}

public sealed class DeleteGoalCommandHandler : ICommandHandler<DeleteGoalCommand>
{
    private readonly IGoalService _goalService;

    public DeleteGoalCommandHandler(IGoalService goalService)
    {
        _goalService = goalService;
    }

    public Task<Result> HandleAsync(
        DeleteGoalCommand command,
        CancellationToken cancellationToken = default) =>
        _goalService.DeleteAsync(command.GoalId, cancellationToken);
}

public sealed class RecordContributionCommandHandler
    : ICommandHandler<RecordContributionCommand, GoalContributionResponse>
{
    private readonly IGoalService _goalService;

    public RecordContributionCommandHandler(IGoalService goalService)
    {
        _goalService = goalService;
    }

    public Task<Result<GoalContributionResponse>> HandleAsync(
        RecordContributionCommand command,
        CancellationToken cancellationToken = default) =>
        _goalService.RecordContributionAsync(command.GoalId, command.Request, cancellationToken);
}

public sealed class CompleteMilestoneCommandHandler
    : ICommandHandler<CompleteMilestoneCommand, GoalMilestoneResponse>
{
    private readonly IGoalService _goalService;

    public CompleteMilestoneCommandHandler(IGoalService goalService)
    {
        _goalService = goalService;
    }

    public Task<Result<GoalMilestoneResponse>> HandleAsync(
        CompleteMilestoneCommand command,
        CancellationToken cancellationToken = default) =>
        _goalService.CompleteMilestoneAsync(
            command.GoalId,
            command.MilestoneId,
            command.Request,
            cancellationToken);
}

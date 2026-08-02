using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Goals.DTOs.Requests;

namespace WealthOS.Application.Goals.Commands;

/// <summary>
/// Creates a new financial goal for the authenticated user.
/// </summary>
public sealed class CreateGoalCommand : ICommand
{
    public CreateGoalRequest Request { get; init; } = null!;
}

/// <summary>
/// Updates an existing goal owned by the authenticated user.
/// </summary>
public sealed class UpdateGoalCommand : ICommand
{
    public Guid GoalId { get; init; }

    public UpdateGoalRequest Request { get; init; } = null!;
}

/// <summary>
/// Soft-deletes a goal owned by the authenticated user.
/// </summary>
public sealed class DeleteGoalCommand : ICommand
{
    public Guid GoalId { get; init; }
}

/// <summary>
/// Records a contribution against a goal.
/// </summary>
public sealed class RecordContributionCommand : ICommand
{
    public Guid GoalId { get; init; }

    public RecordGoalContributionRequest Request { get; init; } = null!;
}

/// <summary>
/// Marks a milestone as completed.
/// </summary>
public sealed class CompleteMilestoneCommand : ICommand
{
    public Guid GoalId { get; init; }

    public Guid MilestoneId { get; init; }

    public CompleteMilestoneRequest Request { get; init; } = null!;
}

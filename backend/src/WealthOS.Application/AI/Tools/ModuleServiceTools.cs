using System.Text.Json;
using WealthOS.Application.AI.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Documents.Interfaces;
using WealthOS.Application.Goals.Interfaces;
using WealthOS.Application.Notifications.Interfaces;
using WealthOS.Domain.AI.Enums;

namespace WealthOS.Application.AI.Tools;

/// <summary>Tool: goal dashboard via <see cref="IGoalService"/> (no repository access).</summary>
public sealed class GetGoalSummaryTool : IAITool
{
    private readonly IGoalService _goalService;

    public GetGoalSummaryTool(IGoalService goalService) => _goalService = goalService;

    public string Code => "get_goal_summary";

    public string Name => "Get Goal Summary";

    public string Description => "Returns financial goal dashboard summary for the user.";

    public AIToolCategory Category => AIToolCategory.Goal;

    public async Task<Result<AIToolResultDto>> ExecuteAsync(
        AIToolExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var result = await _goalService.GetDashboardAsync(cancellationToken);
        if (result.IsFailure)
        {
            return Result.Success(new AIToolResultDto
            {
                ToolCode = Code,
                ToolName = Name,
                Succeeded = false,
                Summary = "Goal summary failed.",
                Error = result.Error!.Message,
            });
        }

        return Result.Success(new AIToolResultDto
        {
            ToolCode = Code,
            ToolName = Name,
            Succeeded = true,
            Summary = "Goal dashboard summary retrieved.",
            PayloadJson = JsonSerializer.Serialize(result.Value),
        });
    }
}

/// <summary>Tool: document search via <see cref="IDocumentSearchService"/>.</summary>
public sealed class SearchDocumentsTool : IAITool
{
    private readonly IDocumentSearchService _documentSearchService;

    public SearchDocumentsTool(IDocumentSearchService documentSearchService) =>
        _documentSearchService = documentSearchService;

    public string Code => "search_documents";

    public string Name => "Search Documents";

    public string Description => "Searches user documents by free text from the message.";

    public AIToolCategory Category => AIToolCategory.Document;

    public async Task<Result<AIToolResultDto>> ExecuteAsync(
        AIToolExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var result = await _documentSearchService.SearchAsync(
            title: null,
            category: null,
            tag: null,
            owner: null,
            referenceModule: null,
            referenceId: null,
            status: null,
            freeText: context.UserMessage,
            page: 1,
            pageSize: 5,
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Success(new AIToolResultDto
            {
                ToolCode = Code,
                ToolName = Name,
                Succeeded = false,
                Summary = "Document search failed.",
                Error = result.Error!.Message,
            });
        }

        return Result.Success(new AIToolResultDto
        {
            ToolCode = Code,
            ToolName = Name,
            Succeeded = true,
            Summary = $"Found {result.Value.TotalCount} document(s).",
            PayloadJson = JsonSerializer.Serialize(result.Value),
        });
    }
}

/// <summary>Tool: notification inbox summary via <see cref="INotificationService"/>.</summary>
public sealed class GetNotificationsTool : IAITool
{
    private readonly INotificationService _notificationService;

    public GetNotificationsTool(INotificationService notificationService) =>
        _notificationService = notificationService;

    public string Code => "get_notifications";

    public string Name => "Get Notifications";

    public string Description => "Returns notification inbox summary counts for the user.";

    public AIToolCategory Category => AIToolCategory.Notification;

    public async Task<Result<AIToolResultDto>> ExecuteAsync(
        AIToolExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var result = await _notificationService.GetSummaryAsync(cancellationToken);
        if (result.IsFailure)
        {
            return Result.Success(new AIToolResultDto
            {
                ToolCode = Code,
                ToolName = Name,
                Succeeded = false,
                Summary = "Notification summary failed.",
                Error = result.Error!.Message,
            });
        }

        return Result.Success(new AIToolResultDto
        {
            ToolCode = Code,
            ToolName = Name,
            Succeeded = true,
            Summary = "Notification summary retrieved.",
            PayloadJson = JsonSerializer.Serialize(result.Value),
        });
    }
}

using System.Text.Json;
using WealthOS.Application.AI.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.Interfaces;
using WealthOS.Domain.AI.Enums;

namespace WealthOS.Application.AI.Tools;

/// <summary>Tool: aggregates dashboard summary via <see cref="IDashboardService"/>.</summary>
public sealed class GetDashboardSummaryTool : IAITool
{
    private readonly IDashboardService _dashboardService;

    public GetDashboardSummaryTool(IDashboardService dashboardService) =>
        _dashboardService = dashboardService;

    public string Code => "get_dashboard_summary";

    public string Name => "Get Dashboard Summary";

    public string Description => "Returns the authenticated user's wealth dashboard summary.";

    public AIToolCategory Category => AIToolCategory.Dashboard;

    public async Task<Result<AIToolResultDto>> ExecuteAsync(
        AIToolExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var result = await _dashboardService.GetSummaryAsync(cancellationToken);
        if (result.IsFailure)
        {
            return Fail(result.Error!.Message);
        }

        return Succeed(JsonSerializer.Serialize(result.Value), "Dashboard summary retrieved.");
    }

    private Result<AIToolResultDto> Succeed(string payload, string summary) =>
        Result.Success(new AIToolResultDto
        {
            ToolCode = Code,
            ToolName = Name,
            Succeeded = true,
            Summary = summary,
            PayloadJson = payload,
        });

    private Result<AIToolResultDto> Fail(string error) =>
        Result.Success(new AIToolResultDto
        {
            ToolCode = Code,
            ToolName = Name,
            Succeeded = false,
            Summary = "Dashboard summary failed.",
            Error = error,
        });
}

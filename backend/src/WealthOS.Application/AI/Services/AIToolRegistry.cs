using WealthOS.Application.AI.Interfaces;
using WealthOS.Application.AI.Options;
using WealthOS.Application.Common.Models;
using Microsoft.Extensions.Options;

namespace WealthOS.Application.AI.Services;

/// <summary>
/// Discovers <see cref="IAITool"/> implementations from DI and selects relevant tools by keyword heuristics.
/// Future tools plug in by registering additional <see cref="IAITool"/> implementations.
/// </summary>
public sealed class AIToolRegistry : IAIToolRegistry
{
    private readonly IReadOnlyList<IAITool> _tools;
    private readonly AIOptions _options;

    public AIToolRegistry(IEnumerable<IAITool> tools, IOptions<AIOptions> options)
    {
        _tools = tools.ToList();
        _options = options.Value;
    }

    public IReadOnlyList<IAITool> GetAll() => _tools;

    public IAITool? GetByCode(string code) =>
        _tools.FirstOrDefault(tool =>
            string.Equals(tool.Code, code, StringComparison.OrdinalIgnoreCase));

    public async Task<IReadOnlyList<AIToolResultDto>> ExecuteRelevantAsync(
        AIToolExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        if (!_options.EnableToolExecution || _tools.Count == 0)
        {
            return Array.Empty<AIToolResultDto>();
        }

        var selected = SelectRelevantTools(context.UserMessage)
            .Take(Math.Max(1, _options.MaxToolsPerTurn))
            .ToList();

        var results = new List<AIToolResultDto>(selected.Count);
        foreach (var tool in selected)
        {
            var result = await tool.ExecuteAsync(context, cancellationToken);
            if (result.IsSuccess)
            {
                results.Add(result.Value);
            }
            else
            {
                results.Add(new AIToolResultDto
                {
                    ToolCode = tool.Code,
                    ToolName = tool.Name,
                    Succeeded = false,
                    Summary = "Tool execution failed.",
                    Error = result.Error?.Message,
                });
            }
        }

        return results;
    }

    private IEnumerable<IAITool> SelectRelevantTools(string message)
    {
        var text = message ?? string.Empty;
        var matches = new List<IAITool>();

        void AddIf(string keyword, string code)
        {
            if (text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                var tool = GetByCode(code);
                if (tool is not null && matches.All(m => m.Code != tool.Code))
                {
                    matches.Add(tool);
                }
            }
        }

        AddIf("property", "get_property_summary");
        AddIf("loan", "get_loan_summary");
        AddIf("emi", "get_loan_summary");
        AddIf("invest", "get_investment_summary");
        AddIf("portfolio", "get_investment_summary");
        AddIf("income", "get_income_summary");
        AddIf("salary", "get_income_summary");
        AddIf("cash flow", "get_income_summary");
        AddIf("goal", "get_goal_summary");
        AddIf("document", "search_documents");
        AddIf("notif", "get_notifications");
        AddIf("dashboard", "get_dashboard_summary");
        AddIf("net worth", "get_dashboard_summary");

        if (matches.Count == 0)
        {
            var dashboard = GetByCode("get_dashboard_summary");
            if (dashboard is not null)
            {
                matches.Add(dashboard);
            }
        }

        return matches;
    }
}

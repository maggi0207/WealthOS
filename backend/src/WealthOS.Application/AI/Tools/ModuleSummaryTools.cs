using System.Text.Json;
using WealthOS.Application.AI.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Dashboard.Providers;
using WealthOS.Domain.AI.Enums;

namespace WealthOS.Application.AI.Tools;

/// <summary>Tool: property totals via <see cref="IPropertySummaryProvider"/>.</summary>
public sealed class GetPropertySummaryTool : IAITool
{
    private readonly IPropertySummaryProvider _provider;

    public GetPropertySummaryTool(IPropertySummaryProvider provider) => _provider = provider;

    public string Code => "get_property_summary";

    public string Name => "Get Property Summary";

    public string Description => "Returns property portfolio totals for the user.";

    public AIToolCategory Category => AIToolCategory.Property;

    public async Task<Result<AIToolResultDto>> ExecuteAsync(
        AIToolExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var summary = await _provider.GetSummaryAsync(context.UserId, cancellationToken);
        return Result.Success(new AIToolResultDto
        {
            ToolCode = Code,
            ToolName = Name,
            Succeeded = true,
            Summary = $"Properties: {summary.PropertyCount}, value {summary.TotalValue} {summary.CurrencyCode}.",
            PayloadJson = JsonSerializer.Serialize(summary),
        });
    }
}

/// <summary>Tool: loan totals via <see cref="ILoanSummaryProvider"/>.</summary>
public sealed class GetLoanSummaryTool : IAITool
{
    private readonly ILoanSummaryProvider _provider;

    public GetLoanSummaryTool(ILoanSummaryProvider provider) => _provider = provider;

    public string Code => "get_loan_summary";

    public string Name => "Get Loan Summary";

    public string Description => "Returns loan balance totals for the user.";

    public AIToolCategory Category => AIToolCategory.Loan;

    public async Task<Result<AIToolResultDto>> ExecuteAsync(
        AIToolExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var summary = await _provider.GetSummaryAsync(context.UserId, cancellationToken);
        return Result.Success(new AIToolResultDto
        {
            ToolCode = Code,
            ToolName = Name,
            Succeeded = true,
            Summary = $"Loans: {summary.LoanCount}, balance {summary.TotalBalance} {summary.CurrencyCode}.",
            PayloadJson = JsonSerializer.Serialize(summary),
        });
    }
}

/// <summary>Tool: investment totals via <see cref="IInvestmentSummaryProvider"/>.</summary>
public sealed class GetInvestmentSummaryTool : IAITool
{
    private readonly IInvestmentSummaryProvider _provider;

    public GetInvestmentSummaryTool(IInvestmentSummaryProvider provider) => _provider = provider;

    public string Code => "get_investment_summary";

    public string Name => "Get Investment Summary";

    public string Description => "Returns investment portfolio totals for the user.";

    public AIToolCategory Category => AIToolCategory.Investment;

    public async Task<Result<AIToolResultDto>> ExecuteAsync(
        AIToolExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var summary = await _provider.GetSummaryAsync(context.UserId, cancellationToken);
        return Result.Success(new AIToolResultDto
        {
            ToolCode = Code,
            ToolName = Name,
            Succeeded = true,
            Summary = $"Holdings: {summary.HoldingCount}, value {summary.TotalValue} {summary.CurrencyCode}.",
            PayloadJson = JsonSerializer.Serialize(summary),
        });
    }
}

/// <summary>Tool: income totals via <see cref="IIncomeSummaryProvider"/>.</summary>
public sealed class GetIncomeSummaryTool : IAITool
{
    private readonly IIncomeSummaryProvider _provider;

    public GetIncomeSummaryTool(IIncomeSummaryProvider provider) => _provider = provider;

    public string Code => "get_income_summary";

    public string Name => "Get Income Summary";

    public string Description => "Returns monthly income and expense totals for the user.";

    public AIToolCategory Category => AIToolCategory.Income;

    public async Task<Result<AIToolResultDto>> ExecuteAsync(
        AIToolExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var summary = await _provider.GetSummaryAsync(context.UserId, cancellationToken);
        return Result.Success(new AIToolResultDto
        {
            ToolCode = Code,
            ToolName = Name,
            Succeeded = true,
            Summary =
                $"Income {summary.MonthlyIncome} / Expense {summary.MonthlyExpense} {summary.CurrencyCode}.",
            PayloadJson = JsonSerializer.Serialize(summary),
        });
    }
}

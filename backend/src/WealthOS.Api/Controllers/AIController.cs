using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthOS.Application.AI.Commands;
using WealthOS.Application.AI.DTOs.Requests;
using WealthOS.Application.AI.DTOs.Responses;
using WealthOS.Application.AI.Queries;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.DTOs;
using WealthOS.Application.Common.Models;

namespace WealthOS.Api.Controllers;

/// <summary>
/// AI Financial Advisor orchestration endpoints (architecture platform — placeholder providers).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/ai")]
public sealed class AIController : ControllerBase
{
    private readonly ICommandHandler<SendMessageCommand, AIChatResponse> _sendMessageHandler;
    private readonly ICommandHandler<ClearConversationCommand> _clearHandler;
    private readonly IQueryHandler<GetConversationHistoryQuery, AIConversationHistoryResponse> _historyHandler;
    private readonly IQueryHandler<GetSuggestionsQuery, AISuggestionsResponse> _suggestionsHandler;
    private readonly IQueryHandler<GetInsightsQuery, AIInsightsResponse> _insightsHandler;

    /// <summary>
    /// Creates a new <see cref="AIController"/>.
    /// </summary>
    public AIController(
        ICommandHandler<SendMessageCommand, AIChatResponse> sendMessageHandler,
        ICommandHandler<ClearConversationCommand> clearHandler,
        IQueryHandler<GetConversationHistoryQuery, AIConversationHistoryResponse> historyHandler,
        IQueryHandler<GetSuggestionsQuery, AISuggestionsResponse> suggestionsHandler,
        IQueryHandler<GetInsightsQuery, AIInsightsResponse> insightsHandler)
    {
        _sendMessageHandler = sendMessageHandler;
        _clearHandler = clearHandler;
        _historyHandler = historyHandler;
        _suggestionsHandler = suggestionsHandler;
        _insightsHandler = insightsHandler;
    }

    /// <summary>
    /// Sends a chat message through the AI orchestration pipeline (context + tools + provider stub).
    /// </summary>
    [HttpPost("chat")]
    [ProducesResponseType(typeof(ApiResponse<AIChatResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Chat(
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sendMessageHandler.HandleAsync(
            new SendMessageCommand { Request = request },
            cancellationToken);

        return ToActionResult(result, "AI chat response generated.");
    }

    /// <summary>
    /// Returns conversation history for the authenticated user.
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(ApiResponse<AIConversationHistoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _historyHandler.HandleAsync(
            new GetConversationHistoryQuery { Page = page, PageSize = pageSize },
            cancellationToken);

        return ToActionResult(result, "AI conversation history retrieved.");
    }

    /// <summary>
    /// Clears active AI conversations for the authenticated user.
    /// </summary>
    [HttpDelete("history")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ClearHistory(CancellationToken cancellationToken)
    {
        var result = await _clearHandler.HandleAsync(
            new ClearConversationCommand(),
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object>.Ok(null!, "AI conversation history cleared."));
        }

        return ToFailureResult(result.Error!);
    }

    /// <summary>
    /// Returns suggested advisor prompts.
    /// </summary>
    [HttpGet("suggestions")]
    [ProducesResponseType(typeof(ApiResponse<AISuggestionsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSuggestions(CancellationToken cancellationToken)
    {
        var result = await _suggestionsHandler.HandleAsync(
            new GetSuggestionsQuery(),
            cancellationToken);

        return ToActionResult(result, "AI suggestions retrieved.");
    }

    /// <summary>
    /// Returns AI insights and recommendations (placeholder generation when empty).
    /// </summary>
    [HttpGet("insights")]
    [ProducesResponseType(typeof(ApiResponse<AIInsightsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetInsights(CancellationToken cancellationToken)
    {
        var result = await _insightsHandler.HandleAsync(
            new GetInsightsQuery(),
            cancellationToken);

        return ToActionResult(result, "AI insights retrieved.");
    }

    private IActionResult ToActionResult<T>(Result<T> result, string successMessage)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.Ok(result.Value, successMessage));
        }

        return ToFailureResult(result.Error!);
    }

    private IActionResult ToFailureResult(Error error)
    {
        var errors = new List<ApiErrorDetail>();

        if (error.ValidationErrors is not null)
        {
            foreach (var (field, messages) in error.ValidationErrors)
            {
                errors.AddRange(messages.Select(message => new ApiErrorDetail
                {
                    Code = error.Code,
                    Message = message,
                    Field = field,
                }));
            }
        }
        else
        {
            errors.Add(new ApiErrorDetail
            {
                Code = error.Code,
                Message = error.Message,
            });
        }

        var payload = ApiResponse<object>.Fail(error.Message, errors);

        return error.Code switch
        {
            "unauthorized" => Unauthorized(payload),
            "forbidden" => StatusCode(StatusCodes.Status403Forbidden, payload),
            "not_found" => NotFound(payload),
            "conflict" => Conflict(payload),
            "validation_error" => UnprocessableEntity(payload),
            _ => BadRequest(payload),
        };
    }
}

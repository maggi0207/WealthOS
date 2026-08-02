using WealthOS.Application.AI.Commands;
using WealthOS.Application.AI.DTOs.Responses;
using WealthOS.Application.AI.Interfaces;
using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.Common.Models;

namespace WealthOS.Application.AI.Commands.Handlers;

public sealed class StartConversationCommandHandler
    : ICommandHandler<StartConversationCommand, AIChatResponse>
{
    private readonly IAIService _aiService;

    public StartConversationCommandHandler(IAIService aiService) => _aiService = aiService;

    public Task<Result<AIChatResponse>> HandleAsync(
        StartConversationCommand command,
        CancellationToken cancellationToken = default) =>
        _aiService.StartConversationAsync(command.Request, cancellationToken);
}

public sealed class SendMessageCommandHandler
    : ICommandHandler<SendMessageCommand, AIChatResponse>
{
    private readonly IAIService _aiService;

    public SendMessageCommandHandler(IAIService aiService) => _aiService = aiService;

    public Task<Result<AIChatResponse>> HandleAsync(
        SendMessageCommand command,
        CancellationToken cancellationToken = default) =>
        _aiService.SendMessageAsync(command.Request, cancellationToken);
}

public sealed class ClearConversationCommandHandler
    : ICommandHandler<ClearConversationCommand>
{
    private readonly IAIService _aiService;

    public ClearConversationCommandHandler(IAIService aiService) => _aiService = aiService;

    public Task<Result> HandleAsync(
        ClearConversationCommand command,
        CancellationToken cancellationToken = default) =>
        _aiService.ClearConversationAsync(cancellationToken);
}

public sealed class SaveMemoryCommandHandler
    : ICommandHandler<SaveMemoryCommand, AIMemoryResponse>
{
    private readonly IAIMemoryService _memoryService;

    public SaveMemoryCommandHandler(IAIMemoryService memoryService) =>
        _memoryService = memoryService;

    public Task<Result<AIMemoryResponse>> HandleAsync(
        SaveMemoryCommand command,
        CancellationToken cancellationToken = default) =>
        _memoryService.SaveAsync(command.Request, cancellationToken);
}

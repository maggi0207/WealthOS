using WealthOS.Application.Common.Abstractions;
using WealthOS.Application.AI.DTOs.Requests;

namespace WealthOS.Application.AI.Commands;

/// <summary>Starts a new AI conversation.</summary>
public sealed class StartConversationCommand : ICommand
{
    public StartConversationRequest? Request { get; init; }
}

/// <summary>Sends a user message through the AI orchestration pipeline.</summary>
public sealed class SendMessageCommand : ICommand
{
    public SendMessageRequest Request { get; init; } = null!;
}

/// <summary>Clears the active conversation for the authenticated user.</summary>
public sealed class ClearConversationCommand : ICommand;

/// <summary>Persists an AI memory item.</summary>
public sealed class SaveMemoryCommand : ICommand
{
    public SaveMemoryRequest Request { get; init; } = null!;
}

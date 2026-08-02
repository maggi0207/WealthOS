using WealthOS.Application.AI.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Domain.AI.Enums;

namespace WealthOS.Infrastructure.AI.Providers;

/// <summary>
/// Shared placeholder provider behavior — no external API calls.
/// </summary>
internal static class AIProviderStub
{
    public static Task<Result<string>> GenerateResponseAsync(
        AIProviderKind kind,
        AIProviderRequest request)
    {
        var tools = request.ToolResults is { Count: > 0 }
            ? string.Join("; ", request.ToolResults.Select(t => $"{t.ToolCode}={t.Summary}"))
            : "none";

        var reply =
            $"[Placeholder:{kind}] Orchestration complete.\n" +
            $"User: {request.UserPrompt}\n" +
            $"Tools: {tools}\n" +
            "No external LLM was invoked. Replace this stub with a real provider later.";

        return Task.FromResult(Result.Success(reply));
    }

    public static Task<Result<string>> GenerateStructuredResponseAsync(
        AIProviderKind kind,
        AIProviderRequest request,
        string schemaHint)
    {
        var json =
            $"{{\"provider\":\"{kind}\",\"schema\":\"{schemaHint}\",\"message\":{System.Text.Json.JsonSerializer.Serialize(request.UserPrompt)},\"placeholder\":true}}";
        return Task.FromResult(Result.Success(json));
    }

    public static Task<Result<IReadOnlyList<float>>> GenerateEmbeddingsAsync(string text)
    {
        // Deterministic fake embedding derived from text length — not a real model.
        var seed = Math.Max(1, text?.Length ?? 0);
        IReadOnlyList<float> vector = Enumerable.Range(0, 8)
            .Select(i => (float)((seed + i) % 97) / 97f)
            .ToList();
        return Task.FromResult(Result.Success(vector));
    }
}

/// <summary>OpenAI provider stub.</summary>
public sealed class OpenAIProvider : IAIProvider
{
    public AIProviderKind Kind => AIProviderKind.OpenAI;

    public Task<Result<string>> GenerateResponseAsync(
        AIProviderRequest request,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateResponseAsync(Kind, request);

    public Task<Result<string>> GenerateStructuredResponseAsync(
        AIProviderRequest request,
        string schemaHint,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateStructuredResponseAsync(Kind, request, schemaHint);

    public Task<Result<IReadOnlyList<float>>> GenerateEmbeddingsAsync(
        string text,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateEmbeddingsAsync(text);
}

/// <summary>Azure OpenAI provider stub.</summary>
public sealed class AzureOpenAIProvider : IAIProvider
{
    public AIProviderKind Kind => AIProviderKind.AzureOpenAI;

    public Task<Result<string>> GenerateResponseAsync(
        AIProviderRequest request,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateResponseAsync(Kind, request);

    public Task<Result<string>> GenerateStructuredResponseAsync(
        AIProviderRequest request,
        string schemaHint,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateStructuredResponseAsync(Kind, request, schemaHint);

    public Task<Result<IReadOnlyList<float>>> GenerateEmbeddingsAsync(
        string text,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateEmbeddingsAsync(text);
}

/// <summary>Anthropic provider stub.</summary>
public sealed class AnthropicProvider : IAIProvider
{
    public AIProviderKind Kind => AIProviderKind.Anthropic;

    public Task<Result<string>> GenerateResponseAsync(
        AIProviderRequest request,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateResponseAsync(Kind, request);

    public Task<Result<string>> GenerateStructuredResponseAsync(
        AIProviderRequest request,
        string schemaHint,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateStructuredResponseAsync(Kind, request, schemaHint);

    public Task<Result<IReadOnlyList<float>>> GenerateEmbeddingsAsync(
        string text,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateEmbeddingsAsync(text);
}

/// <summary>Gemini provider stub.</summary>
public sealed class GeminiProvider : IAIProvider
{
    public AIProviderKind Kind => AIProviderKind.Gemini;

    public Task<Result<string>> GenerateResponseAsync(
        AIProviderRequest request,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateResponseAsync(Kind, request);

    public Task<Result<string>> GenerateStructuredResponseAsync(
        AIProviderRequest request,
        string schemaHint,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateStructuredResponseAsync(Kind, request, schemaHint);

    public Task<Result<IReadOnlyList<float>>> GenerateEmbeddingsAsync(
        string text,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateEmbeddingsAsync(text);
}

/// <summary>Future MCP provider stub (no MCP runtime yet).</summary>
public sealed class MCPProvider : IAIProvider
{
    public AIProviderKind Kind => AIProviderKind.MCP;

    public Task<Result<string>> GenerateResponseAsync(
        AIProviderRequest request,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateResponseAsync(Kind, request);

    public Task<Result<string>> GenerateStructuredResponseAsync(
        AIProviderRequest request,
        string schemaHint,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateStructuredResponseAsync(Kind, request, schemaHint);

    public Task<Result<IReadOnlyList<float>>> GenerateEmbeddingsAsync(
        string text,
        CancellationToken cancellationToken = default) =>
        AIProviderStub.GenerateEmbeddingsAsync(text);
}

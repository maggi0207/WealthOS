namespace WealthOS.Application.AI.Options;

/// <summary>
/// AI platform options. Providers are placeholders — no external API calls.
/// </summary>
public sealed class AIOptions
{
    public const string SectionName = "AI";

    /// <summary>Default provider kind name (OpenAI, AzureOpenAI, Anthropic, Gemini, MCP).</summary>
    public string DefaultProvider { get; set; } = "OpenAI";

    /// <summary>When true, relevant tools are executed during SendMessage.</summary>
    public bool EnableToolExecution { get; set; } = true;

    /// <summary>Maximum tools invoked per chat turn.</summary>
    public int MaxToolsPerTurn { get; set; } = 4;
}

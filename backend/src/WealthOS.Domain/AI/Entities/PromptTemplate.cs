using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.AI.Entities;

/// <summary>
/// Reusable prompt template for AI orchestration (no external LLM calls yet).
/// </summary>
public sealed class PromptTemplate : AuditableEntity
{
    public PromptTemplate()
    {
    }

    public PromptTemplate(Guid id)
        : base(id)
    {
    }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public PromptTemplateCategory Category { get; set; }

    public string SystemPrompt { get; set; } = string.Empty;

    public string UserPromptTemplate { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public int Version { get; set; } = 1;
}

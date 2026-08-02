using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.AI.Entities;

/// <summary>
/// Persisted catalog entry for an extensible AI tool.
/// Runtime execution uses <c>IAITool</c> implementations registered via DI.
/// </summary>
public sealed class AITool : AuditableEntity
{
    public AITool()
    {
    }

    public AITool(Guid id)
        : base(id)
    {
    }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public AIToolCategory Category { get; set; } = AIToolCategory.General;

    public string? InputSchemaJson { get; set; }

    public bool IsEnabled { get; set; } = true;

    public int SortOrder { get; set; }
}

namespace WealthOS.Domain.Dashboard.Models;

/// <summary>
/// Shortcut action surfaced on the dashboard home screen.
/// Phase 3 read model — not mapped to EF.
/// </summary>
public sealed class QuickAction
{
    public string Key { get; init; } = string.Empty;

    public string Label { get; init; } = string.Empty;

    public string Route { get; init; } = string.Empty;

    public string Icon { get; init; } = string.Empty;
}

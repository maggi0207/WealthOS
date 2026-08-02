namespace WealthOS.Domain.Goals.Enums;

/// <summary>
/// Qualitative progress trend versus the planned monthly contribution pace.
/// </summary>
public enum ProgressTrend
{
    OnTrack = 0,
    Ahead = 1,
    Behind = 2,
    Completed = 3,
    Unknown = 4,
}

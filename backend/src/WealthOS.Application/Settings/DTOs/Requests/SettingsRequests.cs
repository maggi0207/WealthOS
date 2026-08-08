namespace WealthOS.Application.Settings.DTOs.Requests;

public sealed class UpdateProfileSettingsRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string WorkspaceName { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public string Timezone { get; set; } = "Asia/Kolkata";

    public string Country { get; set; } = "IN";
}

public sealed class UpdatePreferencesSettingsRequest
{
    public string Theme { get; set; } = "dark";

    public string LayoutDensity { get; set; } = "comfortable";

    public bool SidebarCollapsed { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public string Locale { get; set; } = "en-IN";

    public string DateFormat { get; set; } = "DD/MM/YYYY";

    public string NumberFormat { get; set; } = "indian";
}

public sealed class UpdateNotificationSettingsRequest
{
    public bool EmailNotifications { get; set; }

    public bool PushNotifications { get; set; }

    public bool GoalReminders { get; set; }

    public bool LoanEmiReminders { get; set; }

    public bool InvestmentAlerts { get; set; }

    public bool AiAdvisorInsights { get; set; }

    public bool WeeklySummary { get; set; }

    public bool MonthlyReport { get; set; }
}

public sealed class UpdateSecuritySettingsRequest
{
    public string? CurrentPassword { get; set; }

    public string? NewPassword { get; set; }

    public string? ConfirmPassword { get; set; }

    public bool? TwoFactorEnabled { get; set; }

    public bool? SignOutAllDevices { get; set; }
}

public sealed class UpdateSettingsRequest
{
    public UpdateProfileSettingsRequest? Profile { get; set; }

    public UpdatePreferencesSettingsRequest? Preferences { get; set; }

    public UpdateNotificationSettingsRequest? Notifications { get; set; }

    public string? AngelOneAction { get; set; }
}

public sealed class ExportSettingsRequest
{
    /// <summary>all | investments | loans | properties | reports</summary>
    public string Scope { get; set; } = "all";
}

public sealed class ImportSettingsRequest
{
    public string ContentBase64 { get; set; } = string.Empty;

    public string? FileName { get; set; }
}

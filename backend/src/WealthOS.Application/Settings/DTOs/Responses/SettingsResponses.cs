namespace WealthOS.Application.Settings.DTOs.Responses;

public sealed class UserSettingsResponse
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string WorkspaceName { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public string Timezone { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string Theme { get; set; } = "dark";

    public string LayoutDensity { get; set; } = "comfortable";

    public bool SidebarCollapsed { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public string Locale { get; set; } = "en-IN";

    public string DateFormat { get; set; } = "DD/MM/YYYY";

    public string NumberFormat { get; set; } = "indian";

    public bool EmailNotifications { get; set; }

    public bool PushNotifications { get; set; }

    public bool GoalReminders { get; set; }

    public bool LoanEmiReminders { get; set; }

    public bool InvestmentAlerts { get; set; }

    public bool AiAdvisorInsights { get; set; }

    public bool WeeklySummary { get; set; }

    public bool MonthlyReport { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public bool AngelOneConnected { get; set; }

    public bool IndiaBondsConnected { get; set; }

    public bool BankSyncConnected { get; set; }

    public bool UpiConnected { get; set; }

    public IReadOnlyList<ActiveSessionResponse> ActiveSessions { get; set; } = Array.Empty<ActiveSessionResponse>();
}

public sealed class ActiveSessionResponse
{
    public string Id { get; set; } = string.Empty;

    public string Device { get; set; } = "Current device";

    public string Location { get; set; } = "Unknown";

    public DateTime LastActiveAt { get; set; }

    public bool IsCurrent { get; set; }
}

public sealed class SettingsExportResponse
{
    public string FileName { get; set; } = "wealthos-export.json";

    public string ContentType { get; set; } = "application/json";

    public string ContentBase64 { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; }
}

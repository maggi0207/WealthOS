using WealthOS.Domain.Common.Entities;

namespace WealthOS.Domain.Settings.Entities;

/// <summary>
/// Per-user workspace preferences (profile extras, appearance, regional, notifications).
/// </summary>
public sealed class UserSettings : AuditableEntity
{
    public UserSettings()
    {
    }

    public UserSettings(Guid id)
        : base(id)
    {
    }

    public Guid UserId { get; set; }

    public string WorkspaceName { get; set; } = "My Workspace";

    public string? AvatarUrl { get; set; }

    public string Timezone { get; set; } = "Asia/Kolkata";

    public string Country { get; set; } = "IN";

    /// <summary>system | light | dark</summary>
    public string Theme { get; set; } = "dark";

    /// <summary>comfortable | compact</summary>
    public string LayoutDensity { get; set; } = "comfortable";

    public bool SidebarCollapsed { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public string Locale { get; set; } = "en-IN";

    /// <summary>DD/MM/YYYY | MM/DD/YYYY | YYYY-MM-DD</summary>
    public string DateFormat { get; set; } = "DD/MM/YYYY";

    /// <summary>indian | international</summary>
    public string NumberFormat { get; set; } = "indian";

    public bool EmailNotifications { get; set; } = true;

    public bool PushNotifications { get; set; }

    public bool GoalReminders { get; set; } = true;

    public bool LoanEmiReminders { get; set; } = true;

    public bool InvestmentAlerts { get; set; } = true;

    public bool AiAdvisorInsights { get; set; } = true;

    public bool WeeklySummary { get; set; } = true;

    public bool MonthlyReport { get; set; } = true;

    public bool TwoFactorEnabled { get; set; }

    public bool AngelOneConnected { get; set; }

    public bool IndiaBondsConnected { get; set; }

    public bool BankSyncConnected { get; set; }

    public bool UpiConnected { get; set; }
}

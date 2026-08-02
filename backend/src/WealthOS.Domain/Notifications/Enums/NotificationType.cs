namespace WealthOS.Domain.Notifications.Enums;

/// <summary>
/// Business notification categories produced by modules or background jobs.
/// </summary>
public enum NotificationType
{
    SalaryReminder = 0,
    EmiReminder = 1,
    SipReminder = 2,
    BondMaturity = 3,
    DocumentExpiry = 4,
    GoalMilestone = 5,
    InvestmentSync = 6,
    PropertyReminder = 7,
    BusinessInvoiceReminder = 8,
    GeneralReminder = 9,
    DailyDashboardSummary = 10,
    WeeklySummary = 11,
    MonthlySummary = 12,
    LoanReminder = 13,
    GoalProgressCheck = 14,
}

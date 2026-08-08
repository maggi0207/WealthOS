export type UserSettingsDto = {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  workspaceName: string;
  avatarUrl: string | null;
  timezone: string;
  country: string;
  theme: "system" | "light" | "dark" | string;
  layoutDensity: "comfortable" | "compact" | string;
  sidebarCollapsed: boolean;
  currencyCode: string;
  locale: string;
  dateFormat: string;
  numberFormat: "indian" | "international" | string;
  emailNotifications: boolean;
  pushNotifications: boolean;
  goalReminders: boolean;
  loanEmiReminders: boolean;
  investmentAlerts: boolean;
  aiAdvisorInsights: boolean;
  weeklySummary: boolean;
  monthlyReport: boolean;
  twoFactorEnabled: boolean;
  angelOneConnected: boolean;
  indiaBondsConnected: boolean;
  bankSyncConnected: boolean;
  upiConnected: boolean;
  activeSessions: Array<{
    id: string;
    device: string;
    location: string;
    lastActiveAt: string;
    isCurrent: boolean;
  }>;
};

export type UpdateProfileSettingsRequest = {
  firstName: string;
  lastName: string;
  workspaceName: string;
  avatarUrl?: string | null;
  timezone: string;
  country: string;
};

export type UpdatePreferencesSettingsRequest = {
  theme: string;
  layoutDensity: string;
  sidebarCollapsed: boolean;
  currencyCode: string;
  locale: string;
  dateFormat: string;
  numberFormat: string;
};

export type UpdateNotificationSettingsRequest = {
  emailNotifications: boolean;
  pushNotifications: boolean;
  goalReminders: boolean;
  loanEmiReminders: boolean;
  investmentAlerts: boolean;
  aiAdvisorInsights: boolean;
  weeklySummary: boolean;
  monthlyReport: boolean;
};

export type UpdateSecuritySettingsRequest = {
  currentPassword?: string;
  newPassword?: string;
  confirmPassword?: string;
  twoFactorEnabled?: boolean;
  signOutAllDevices?: boolean;
};

export type SettingsExportResponse = {
  fileName: string;
  contentType: string;
  contentBase64: string;
  generatedAt: string;
};

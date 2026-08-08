import { isMockApiMode } from "@/config/env";
import { BaseApiService } from "@/services/http/base-api-service";
import type {
  SettingsExportResponse,
  UpdateNotificationSettingsRequest,
  UpdatePreferencesSettingsRequest,
  UpdateProfileSettingsRequest,
  UpdateSecuritySettingsRequest,
  UserSettingsDto,
} from "@/services/settings/types";

const mockSettings: UserSettingsDto = {
  userId: "mock-user",
  email: "you@wealthos.app",
  firstName: "Alex",
  lastName: "Morgan",
  fullName: "Alex Morgan",
  workspaceName: "Alex's Workspace",
  avatarUrl: null,
  timezone: "Asia/Kolkata",
  country: "IN",
  theme: "dark",
  layoutDensity: "comfortable",
  sidebarCollapsed: false,
  currencyCode: "INR",
  locale: "en-IN",
  dateFormat: "DD/MM/YYYY",
  numberFormat: "indian",
  emailNotifications: true,
  pushNotifications: false,
  goalReminders: true,
  loanEmiReminders: true,
  investmentAlerts: true,
  aiAdvisorInsights: true,
  weeklySummary: true,
  monthlyReport: true,
  twoFactorEnabled: false,
  angelOneConnected: false,
  indiaBondsConnected: false,
  bankSyncConnected: false,
  upiConnected: false,
  activeSessions: [
    {
      id: "current",
      device: "This browser",
      location: "Current session",
      lastActiveAt: new Date().toISOString(),
      isCurrent: true,
    },
  ],
};

class SettingsService extends BaseApiService {
  protected readonly serviceName = "SettingsService";

  async get(signal?: AbortSignal): Promise<UserSettingsDto> {
    if (isMockApiMode()) return structuredClone(mockSettings);
    return this.getRequest<UserSettingsDto>("/settings", signal);
  }

  async updateProfile(body: UpdateProfileSettingsRequest): Promise<UserSettingsDto> {
    if (isMockApiMode()) {
      Object.assign(mockSettings, {
        firstName: body.firstName,
        lastName: body.lastName,
        fullName: `${body.firstName} ${body.lastName}`.trim(),
        workspaceName: body.workspaceName,
        avatarUrl: body.avatarUrl ?? null,
        timezone: body.timezone,
        country: body.country,
      });
      return structuredClone(mockSettings);
    }
    return this.put<UserSettingsDto>("/settings/profile", body);
  }

  async updatePreferences(body: UpdatePreferencesSettingsRequest): Promise<UserSettingsDto> {
    if (isMockApiMode()) {
      Object.assign(mockSettings, body);
      return structuredClone(mockSettings);
    }
    return this.put<UserSettingsDto>("/settings/preferences", body);
  }

  async updateNotifications(body: UpdateNotificationSettingsRequest): Promise<UserSettingsDto> {
    if (isMockApiMode()) {
      Object.assign(mockSettings, body);
      return structuredClone(mockSettings);
    }
    return this.put<UserSettingsDto>("/settings/notifications", body);
  }

  async updateSecurity(body: UpdateSecuritySettingsRequest): Promise<UserSettingsDto> {
    if (isMockApiMode()) {
      if (body.twoFactorEnabled != null) mockSettings.twoFactorEnabled = body.twoFactorEnabled;
      return structuredClone(mockSettings);
    }
    return this.put<UserSettingsDto>("/settings/security", body);
  }

  async connectAngelOne(connect: boolean): Promise<UserSettingsDto> {
    if (isMockApiMode()) {
      mockSettings.angelOneConnected = connect;
      return structuredClone(mockSettings);
    }
    return this.put<UserSettingsDto>("/settings", {
      angelOneAction: connect ? "connect" : "disconnect",
    });
  }

  async exportData(scope: string): Promise<SettingsExportResponse> {
    if (isMockApiMode()) {
      const json = JSON.stringify({ scope, settings: mockSettings, exportedAt: new Date().toISOString() }, null, 2);
      return {
        fileName: `wealthos-${scope}-mock.json`,
        contentType: "application/json",
        contentBase64: btoa(unescape(encodeURIComponent(json))),
        generatedAt: new Date().toISOString(),
      };
    }
    return this.post<SettingsExportResponse>("/settings/export", { scope });
  }

  async importData(contentBase64: string, fileName?: string): Promise<UserSettingsDto> {
    if (isMockApiMode()) return structuredClone(mockSettings);
    return this.post<UserSettingsDto>("/settings/import", { contentBase64, fileName });
  }

  async clearCache(): Promise<void> {
    if (isMockApiMode()) return;
    await this.post<unknown>("/settings/clear-cache");
  }

  async deleteAccount(): Promise<void> {
    if (isMockApiMode()) return;
    await this.deleteRequest<unknown>("/settings/account");
  }

  private getRequest<T>(path: string, signal?: AbortSignal) {
    return this.get<T>(path, { signal });
  }

  private deleteRequest<T>(path: string) {
    return this.delete<T>(path);
  }
}

export const settingsService = new SettingsService();

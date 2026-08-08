import { BaseApiService } from "@/services/http/base-api-service";
import type {
  SettingsExportResponse,
  UpdateNotificationSettingsRequest,
  UpdatePreferencesSettingsRequest,
  UpdateProfileSettingsRequest,
  UpdateSecuritySettingsRequest,
  UserSettingsDto,
} from "@/services/settings/types";

class SettingsService extends BaseApiService {
  protected readonly serviceName = "SettingsService";

  async get(signal?: AbortSignal): Promise<UserSettingsDto> {
    return this.getRequest<UserSettingsDto>("/settings", signal);
  }

  async updateProfile(body: UpdateProfileSettingsRequest): Promise<UserSettingsDto> {
    return this.put<UserSettingsDto>("/settings/profile", body);
  }

  async updatePreferences(body: UpdatePreferencesSettingsRequest): Promise<UserSettingsDto> {
    return this.put<UserSettingsDto>("/settings/preferences", body);
  }

  async updateNotifications(body: UpdateNotificationSettingsRequest): Promise<UserSettingsDto> {
    return this.put<UserSettingsDto>("/settings/notifications", body);
  }

  async updateSecurity(body: UpdateSecuritySettingsRequest): Promise<UserSettingsDto> {
    return this.put<UserSettingsDto>("/settings/security", body);
  }

  async connectAngelOne(connect: boolean): Promise<UserSettingsDto> {
    return this.put<UserSettingsDto>("/settings", {
      angelOneAction: connect ? "connect" : "disconnect",
    });
  }

  async exportData(scope: string): Promise<SettingsExportResponse> {
    return this.post<SettingsExportResponse>("/settings/export", { scope });
  }

  async importData(contentBase64: string, fileName?: string): Promise<UserSettingsDto> {
    return this.post<UserSettingsDto>("/settings/import", { contentBase64, fileName });
  }

  async clearCache(): Promise<void> {
    await this.post<unknown>("/settings/clear-cache");
  }

  async deleteAccount(): Promise<void> {
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

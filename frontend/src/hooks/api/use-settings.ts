import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { settingsService } from "@/services/settings/settings-service";
import type {
  UpdateNotificationSettingsRequest,
  UpdatePreferencesSettingsRequest,
  UpdateProfileSettingsRequest,
  UpdateSecuritySettingsRequest,
} from "@/services/settings/types";

export const settingsKeys = {
  all: ["settings"] as const,
  detail: () => [...settingsKeys.all, "detail"] as const,
};

export function useSettings() {
  return useQuery({
    queryKey: settingsKeys.detail(),
    queryFn: ({ signal }) => {
      const timeout = AbortSignal.timeout(15_000);
      const merged =
        typeof AbortSignal.any === "function"
          ? AbortSignal.any([signal, timeout])
          : timeout;
      return settingsService.get(merged);
    },
    retry: 1,
    staleTime: 30_000,
  });
}

function useInvalidateSettings() {
  const queryClient = useQueryClient();
  return () => void queryClient.invalidateQueries({ queryKey: settingsKeys.all });
}

export function useUpdateProfileSettings() {
  const invalidate = useInvalidateSettings();
  return useMutation({
    mutationFn: (body: UpdateProfileSettingsRequest) => settingsService.updateProfile(body),
    onSuccess: invalidate,
  });
}

export function useUpdatePreferencesSettings() {
  const invalidate = useInvalidateSettings();
  return useMutation({
    mutationFn: (body: UpdatePreferencesSettingsRequest) => settingsService.updatePreferences(body),
    onSuccess: invalidate,
  });
}

export function useUpdateNotificationSettings() {
  const invalidate = useInvalidateSettings();
  return useMutation({
    mutationFn: (body: UpdateNotificationSettingsRequest) => settingsService.updateNotifications(body),
    onSuccess: invalidate,
  });
}

export function useUpdateSecuritySettings() {
  const invalidate = useInvalidateSettings();
  return useMutation({
    mutationFn: (body: UpdateSecuritySettingsRequest) => settingsService.updateSecurity(body),
    onSuccess: invalidate,
  });
}

export function useConnectAngelOne() {
  const invalidate = useInvalidateSettings();
  return useMutation({
    mutationFn: (connect: boolean) => settingsService.connectAngelOne(connect),
    onSuccess: invalidate,
  });
}

export function useExportSettings() {
  return useMutation({
    mutationFn: (scope: string) => settingsService.exportData(scope),
  });
}

export function useImportSettings() {
  const invalidate = useInvalidateSettings();
  return useMutation({
    mutationFn: (payload: { contentBase64: string; fileName?: string }) =>
      settingsService.importData(payload.contentBase64, payload.fileName),
    onSuccess: invalidate,
  });
}

export function useClearSettingsCache() {
  return useMutation({
    mutationFn: () => settingsService.clearCache(),
  });
}

export function useDeleteAccount() {
  return useMutation({
    mutationFn: () => settingsService.deleteAccount(),
  });
}

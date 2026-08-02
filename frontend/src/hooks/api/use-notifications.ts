import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { notificationService } from "@/services/notifications/notification-service";

export const notificationKeys = {
  all: ["notifications"] as const,
  overview: () => [...notificationKeys.all, "overview"] as const,
};

export function useNotifications() {
  return useQuery({
    queryKey: notificationKeys.overview(),
    queryFn: () => notificationService.getOverview(),
  });
}

export function useMarkNotificationRead() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => notificationService.markRead(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: notificationKeys.all });
    },
  });
}

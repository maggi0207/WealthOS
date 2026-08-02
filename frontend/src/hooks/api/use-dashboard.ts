import { useQuery } from "@tanstack/react-query";

import { dashboardService } from "@/services/dashboard/dashboard-service";

export const dashboardKeys = {
  all: ["dashboard"] as const,
  summary: () => [...dashboardKeys.all, "summary"] as const,
  netWorth: () => [...dashboardKeys.all, "net-worth"] as const,
  activities: (limit: number) =>
    [...dashboardKeys.all, "activities", limit] as const,
  healthScore: () => [...dashboardKeys.all, "health-score"] as const,
  health: () => [...dashboardKeys.all, "health"] as const,
};

/** Full dashboard summary — KPIs, health score, activities. */
export function useDashboard() {
  return useQuery({
    queryKey: dashboardKeys.summary(),
    queryFn: () => dashboardService.getDashboard(),
  });
}

/** Net-worth / assets / liabilities. */
export function useNetWorth() {
  return useQuery({
    queryKey: dashboardKeys.netWorth(),
    queryFn: () => dashboardService.getNetWorth(),
  });
}

/** Recent activity feed. */
export function useRecentActivities(limit = 10) {
  return useQuery({
    queryKey: dashboardKeys.activities(limit),
    queryFn: () => dashboardService.getActivities(limit),
  });
}

/**
 * Financial health score for the home hero.
 * Shares the summary query cache when `useDashboard()` is also mounted.
 */
export function useHealthScore() {
  return useQuery({
    queryKey: dashboardKeys.summary(),
    queryFn: () => dashboardService.getDashboard(),
    select: (data) => data.healthScore,
  });
}

/** Provider readiness (`GET /dashboard/health`) — diagnostics, not the hero score. */
export function useDashboardHealth() {
  return useQuery({
    queryKey: dashboardKeys.health(),
    queryFn: () => dashboardService.getHealth(),
  });
}

/** @deprecated Prefer useNetWorth() */
export function useDashboardNetWorth() {
  return useNetWorth();
}

/** @deprecated Prefer useRecentActivities() */
export function useDashboardActivities(limit = 10) {
  return useRecentActivities(limit);
}

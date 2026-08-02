import { useQuery } from "@tanstack/react-query";

import { goalService } from "@/services/goals/goal-service";

export const goalKeys = {
  all: ["goals"] as const,
  overview: () => [...goalKeys.all, "overview"] as const,
};

export function useGoalsOverview() {
  return useQuery({
    queryKey: goalKeys.overview(),
    queryFn: () => goalService.getOverview(),
  });
}

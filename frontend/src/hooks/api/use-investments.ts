import { useQuery } from "@tanstack/react-query";

import { investmentService } from "@/services/investments/investment-service";

export const investmentKeys = {
  all: ["investments"] as const,
  overview: () => [...investmentKeys.all, "overview"] as const,
};

export function useInvestmentsOverview() {
  return useQuery({
    queryKey: investmentKeys.overview(),
    queryFn: () => investmentService.getOverview(),
  });
}

/** @deprecated Prefer useInvestmentsOverview */
export function useInvestments() {
  return useInvestmentsOverview();
}

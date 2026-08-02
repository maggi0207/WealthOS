import { useQuery } from "@tanstack/react-query";

import { incomeService } from "@/services/income/income-service";

export const incomeKeys = {
  all: ["income"] as const,
  overview: (period?: string) =>
    [...incomeKeys.all, "overview", period ?? "current"] as const,
};

/** Aggregated income & business overview used by the Income page. */
export function useIncomeOverview(period?: string) {
  return useQuery({
    queryKey: incomeKeys.overview(period),
    queryFn: () => incomeService.getOverview(period),
  });
}

/** @deprecated Prefer useIncomeOverview */
export function useIncome() {
  return useIncomeOverview();
}

/** @deprecated Prefer useIncomeOverview().data.clients */
export function useClients() {
  return useQuery({
    queryKey: [...incomeKeys.all, "clients"],
    queryFn: async () => (await incomeService.getOverview()).clients,
  });
}

/** @deprecated Prefer useIncomeOverview().data.expenses */
export function useExpenses() {
  return useQuery({
    queryKey: [...incomeKeys.all, "expenses"],
    queryFn: async () => (await incomeService.getOverview()).expenses,
  });
}

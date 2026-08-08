import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { useAuth } from "@/lib/mock-auth";
import { investmentService } from "@/services/investments/investment-service";
import type {
  AddManualHoldingRequestDto,
  CreateInvestmentAccountRequestDto,
  RecordTransactionRequestDto,
  UpdateHoldingRequestDto,
  UpdateInvestmentAccountRequestDto,
} from "@/services/investments/types";

export const investmentKeys = {
  all: ["investments"] as const,
  overview: () => [...investmentKeys.all, "overview"] as const,
  providers: () => [...investmentKeys.all, "providers"] as const,
  performance: (range: string) => [...investmentKeys.all, "performance", range] as const,
};

export function useInvestmentsOverview() {
  const { isReady, user } = useAuth();
  return useQuery({
    queryKey: investmentKeys.overview(),
    queryFn: () => investmentService.getOverview(),
    enabled: isReady && Boolean(user),
    retry: 1,
  });
}

export function useInvestmentProviders() {
  const { isReady, user } = useAuth();
  return useQuery({
    queryKey: investmentKeys.providers(),
    queryFn: () => investmentService.getProviders(),
    enabled: isReady && Boolean(user),
  });
}

export function useInvestmentPerformance(range: string) {
  const { isReady, user } = useAuth();
  return useQuery({
    queryKey: investmentKeys.performance(range),
    queryFn: () => investmentService.getPerformance(range),
    enabled: isReady && Boolean(user),
  });
}

/** @deprecated Prefer useInvestmentsOverview */
export function useInvestments() {
  return useInvestmentsOverview();
}

function invalidateInvestments(queryClient: ReturnType<typeof useQueryClient>) {
  void queryClient.invalidateQueries({ queryKey: investmentKeys.all });
  void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
}

export function useCreateInvestmentAccount() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateInvestmentAccountRequestDto) =>
      investmentService.createAccount(body),
    onSuccess: () => invalidateInvestments(queryClient),
  });
}

export function useUpdateInvestmentAccount() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      id,
      body,
    }: {
      id: string;
      body: UpdateInvestmentAccountRequestDto;
    }) => investmentService.updateAccount(id, body),
    onSuccess: () => invalidateInvestments(queryClient),
  });
}

export function useDeleteInvestmentAccount() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => investmentService.deleteAccount(id),
    onSuccess: () => invalidateInvestments(queryClient),
  });
}

export function useAddManualHolding() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: AddManualHoldingRequestDto) =>
      investmentService.addManualHolding(body),
    onSuccess: () => invalidateInvestments(queryClient),
  });
}

export function useUpdateHolding() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: UpdateHoldingRequestDto }) =>
      investmentService.updateHolding(id, body),
    onSuccess: () => invalidateInvestments(queryClient),
  });
}

export function useDeleteHolding() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => investmentService.deleteHolding(id),
    onSuccess: () => invalidateInvestments(queryClient),
  });
}

export function useRecordInvestmentTransaction() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: RecordTransactionRequestDto) =>
      investmentService.recordTransaction(body),
    onSuccess: () => invalidateInvestments(queryClient),
  });
}

export function useConnectInvestmentProvider() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (accountId: string) => investmentService.connectProvider(accountId),
    onSuccess: () => invalidateInvestments(queryClient),
  });
}

export function useSyncInvestmentProvider() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      accountId,
      target = "holdings",
    }: {
      accountId: string;
      target?: "portfolio" | "holdings" | "transactions";
    }) => investmentService.syncProvider(accountId, target),
    onSuccess: () => invalidateInvestments(queryClient),
  });
}

export function useDisconnectInvestmentProvider() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (accountId: string) => investmentService.disconnectProvider(accountId),
    onSuccess: () => invalidateInvestments(queryClient),
  });
}

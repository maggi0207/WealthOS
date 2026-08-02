import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

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
};

export function useInvestmentsOverview() {
  return useQuery({
    queryKey: investmentKeys.overview(),
    queryFn: () => investmentService.getOverview(),
  });
}

export function useInvestmentProviders() {
  return useQuery({
    queryKey: investmentKeys.providers(),
    queryFn: () => investmentService.getProviders(),
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

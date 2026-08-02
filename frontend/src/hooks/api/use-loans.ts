import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { loanService } from "@/services/loans/loan-service";
import type {
  CreateLoanRequestDto,
  LoanListQuery,
  RecordLoanPaymentRequestDto,
  UpdateLoanRequestDto,
} from "@/services/loans/types";

export const loanKeys = {
  all: ["loans"] as const,
  lists: () => [...loanKeys.all, "list"] as const,
  list: (params: LoanListQuery = {}) =>
    [...loanKeys.lists(), params] as const,
  summary: () => [...loanKeys.all, "summary"] as const,
  details: () => [...loanKeys.all, "detail"] as const,
  detail: (id: string) => [...loanKeys.details(), id] as const,
  dashboard: (id: string) => [...loanKeys.all, "dashboard", id] as const,
  upcoming: (days: number) => [...loanKeys.all, "upcoming", days] as const,
  payments: (id: string) => [...loanKeys.all, "payments", id] as const,
};

/** Paginated loan list + totals — GET /api/v1/loans + summary */
export function useLoans(params: LoanListQuery = {}) {
  return useQuery({
    queryKey: loanKeys.list(params),
    queryFn: () => loanService.list(params),
  });
}

/** Portfolio summary — GET /api/v1/loans/summary */
export function useLoanSummary() {
  return useQuery({
    queryKey: loanKeys.summary(),
    queryFn: () => loanService.getSummary(),
  });
}

/** Single loan detail — GET /api/v1/loans/{id} */
export function useLoan(id: string) {
  return useQuery({
    queryKey: loanKeys.detail(id),
    queryFn: () => loanService.getById(id),
    enabled: Boolean(id),
  });
}

/** Per-loan dashboard — GET /api/v1/loans/{id}/dashboard */
export function useLoanDashboard(id: string) {
  return useQuery({
    queryKey: loanKeys.dashboard(id),
    queryFn: () => loanService.getDashboard(id),
    enabled: Boolean(id),
  });
}

/** Upcoming EMIs — GET /api/v1/loans/upcoming */
export function useUpcomingLoanPayments(days = 45) {
  return useQuery({
    queryKey: loanKeys.upcoming(days),
    queryFn: () => loanService.getUpcoming(days),
  });
}

/** Payment history for a loan */
export function useLoanPayments(loanId: string) {
  return useQuery({
    queryKey: loanKeys.payments(loanId),
    queryFn: () => loanService.getPayments(loanId),
    enabled: Boolean(loanId),
  });
}

export function useCreateLoan() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateLoanRequestDto) => loanService.create(body),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: loanKeys.all });
      void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
    },
  });
}

export function useUpdateLoan() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      id,
      body,
    }: {
      id: string;
      body: UpdateLoanRequestDto;
    }) => loanService.update(id, body),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: loanKeys.all });
      void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
    },
  });
}

export function useDeleteLoan() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => loanService.remove(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: loanKeys.all });
      void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
    },
  });
}

export function useRecordLoanPayment() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      id,
      body,
    }: {
      id: string;
      body: RecordLoanPaymentRequestDto;
    }) => loanService.recordPayment(id, body),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: loanKeys.all });
    },
  });
}

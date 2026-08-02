import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { incomeService } from "@/services/income/income-service";
import type {
  AssignDeveloperRequestDto,
  CreateClientRequestDto,
  CreateDeveloperRequestDto,
  CreateExpenseRequestDto,
  CreateInvoiceRequestDto,
  CreateProjectRequestDto,
  RecordInvoicePaymentRequestDto,
  RecordSalaryRequestDto,
  UpdateClientRequestDto,
} from "@/services/income/requests";

export const incomeKeys = {
  all: ["income"] as const,
  overview: (period?: string) =>
    [...incomeKeys.all, "overview", period ?? "current"] as const,
  clients: () => [...incomeKeys.all, "clients"] as const,
  invoices: () => [...incomeKeys.all, "invoices"] as const,
  projects: () => [...incomeKeys.all, "projects"] as const,
};

function invalidateIncomeData(
  queryClient: ReturnType<typeof useQueryClient>,
): void {
  void queryClient.invalidateQueries({ queryKey: incomeKeys.all });
  void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
}

/** Aggregated income & business overview used by the Income page. */
export function useIncomeOverview(period?: string) {
  return useQuery({
    queryKey: incomeKeys.overview(period),
    queryFn: () => incomeService.getOverview(period),
  });
}

export function useIncomeClients() {
  return useQuery({
    queryKey: incomeKeys.clients(),
    queryFn: () => incomeService.listClients(),
  });
}

export function useIncomeInvoices() {
  return useQuery({
    queryKey: incomeKeys.invoices(),
    queryFn: () => incomeService.listInvoices(),
  });
}

export function useIncomeProjects() {
  return useQuery({
    queryKey: incomeKeys.projects(),
    queryFn: () => incomeService.listProjects(),
  });
}

export function useRecordSalary() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: RecordSalaryRequestDto) => incomeService.recordSalary(body),
    onSuccess: () => invalidateIncomeData(queryClient),
  });
}

export function useCreateClient() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateClientRequestDto) => incomeService.createClient(body),
    onSuccess: () => invalidateIncomeData(queryClient),
  });
}

export function useUpdateClient() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: UpdateClientRequestDto }) =>
      incomeService.updateClient(id, body),
    onSuccess: () => invalidateIncomeData(queryClient),
  });
}

export function useDeleteClient() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => incomeService.deleteClient(id),
    onSuccess: () => invalidateIncomeData(queryClient),
  });
}

export function useCreateProject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateProjectRequestDto) => incomeService.createProject(body),
    onSuccess: () => invalidateIncomeData(queryClient),
  });
}

export function useAssignDeveloper() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: AssignDeveloperRequestDto) =>
      incomeService.assignDeveloper(body),
    onSuccess: () => invalidateIncomeData(queryClient),
  });
}

export function useCreateDeveloper() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateDeveloperRequestDto) =>
      incomeService.createDeveloper(body),
    onSuccess: () => invalidateIncomeData(queryClient),
  });
}

export function useCreateInvoice() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateInvoiceRequestDto) => incomeService.createInvoice(body),
    onSuccess: () => invalidateIncomeData(queryClient),
  });
}

export function useRecordPayment() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: RecordInvoicePaymentRequestDto) =>
      incomeService.recordPayment(body),
    onSuccess: () => invalidateIncomeData(queryClient),
  });
}

export function useCreateExpense() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateExpenseRequestDto) => incomeService.createExpense(body),
    onSuccess: () => invalidateIncomeData(queryClient),
  });
}

/** @deprecated Prefer useIncomeOverview */
export function useIncome() {
  return useIncomeOverview();
}

/** @deprecated Prefer useIncomeOverview().data.clients */
export function useClients() {
  return useQuery({
    queryKey: [...incomeKeys.all, "clients-legacy"],
    queryFn: async () => (await incomeService.getOverview()).clients,
  });
}

/** @deprecated Prefer useIncomeOverview().data.expenses */
export function useExpenses() {
  return useQuery({
    queryKey: [...incomeKeys.all, "expenses-legacy"],
    queryFn: async () => (await incomeService.getOverview()).expenses,
  });
}

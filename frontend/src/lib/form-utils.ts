import { toast } from "sonner";

import { ApiError, isApiError } from "@/services/http/problem-details";

export function parseRequiredNumber(raw: string, label: string): number | string {
  const trimmed = raw.trim();
  if (!trimmed) return `${label} is required`;
  const n = Number(trimmed.replace(/,/g, ""));
  if (!Number.isFinite(n)) return `${label} must be a number`;
  return n;
}

export function parseOptionalNumber(raw: string): number | null {
  const trimmed = raw.trim();
  if (!trimmed) return null;
  const n = Number(trimmed.replace(/,/g, ""));
  return Number.isFinite(n) ? n : null;
}

export function requiredText(raw: string, label: string, min = 2): string | null {
  const value = raw.trim();
  if (value.length < min) return `${label} is required`;
  return null;
}

export function mutationErrorMessage(error: unknown, fallback = "Request failed"): string {
  if (isApiError(error)) {
    const firstField = Object.values(error.fieldErrors)[0]?.[0];
    return firstField || error.message || fallback;
  }
  if (error instanceof Error) return error.message;
  return fallback;
}

export function toastMutationError(error: unknown, fallback = "Request failed"): void {
  toast.error(mutationErrorMessage(error, fallback));
}

export function todayIsoDate(): string {
  return new Date().toISOString().slice(0, 10);
}

export function currentYearMonth(): string {
  const d = new Date();
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}`;
}

export function invalidateDashboard(queryClient: {
  invalidateQueries: (opts: { queryKey: readonly unknown[] }) => Promise<unknown> | unknown;
}): void {
  void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
}

export type { ApiError };

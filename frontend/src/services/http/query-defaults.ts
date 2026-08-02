import type { QueryClientConfig } from "@tanstack/react-query";

import { handleApiError } from "@/services/http/api-error-handler";
import { ApiError, isApiError } from "@/services/http/problem-details";

function shouldRetryQuery(failureCount: number, error: Error): boolean {
  if (isApiError(error)) {
    // Do not retry client errors except request timeout / too many requests
    if (error.status === 408 || error.status === 429) {
      return failureCount < 2;
    }
    if (error.status >= 400 && error.status < 500) {
      return false;
    }
  }
  return failureCount < 2;
}

/**
 * Shared TanStack Query defaults for WealthOS.
 * Wire via `new QueryClient(createQueryClientOptions())`.
 */
export function createQueryClientOptions(): QueryClientConfig {
  return {
    defaultOptions: {
      queries: {
        retry: shouldRetryQuery,
        staleTime: 30_000,
        gcTime: 5 * 60_000,
        refetchOnWindowFocus: false,
        refetchOnReconnect: true,
      },
      mutations: {
        retry: 0,
        onError: (error) => {
          handleApiError(error instanceof Error ? error : new Error(String(error)));
        },
      },
    },
  };
}

export { ApiError };

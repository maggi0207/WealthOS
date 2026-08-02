import { isMockApiMode } from "@/config/env";
import { apiClient } from "@/services/http/api-client";
import { MockApiNotConnectedError } from "@/services/http/problem-details";
import type { RequestOptions } from "@/services/http/types";

/**
 * Base class for domain API services.
 * Enforces mock-mode guard and routes all HTTP through ApiClient.
 */
export abstract class BaseApiService {
  protected abstract readonly serviceName: string;

  /** Throw when `VITE_API_MODE=mock` so callers never hit the network accidentally. */
  protected ensureApiMode(): void {
    if (isMockApiMode()) {
      throw new MockApiNotConnectedError(this.serviceName);
    }
  }

  protected get<T>(
    path: string,
    options?: Omit<RequestOptions, "method" | "body">,
  ): Promise<T> {
    this.ensureApiMode();
    return apiClient.get<T>(path, options);
  }

  protected post<T>(
    path: string,
    body?: unknown,
    options?: Omit<RequestOptions, "method" | "body">,
  ): Promise<T> {
    this.ensureApiMode();
    return apiClient.post<T>(path, body, options);
  }

  protected put<T>(
    path: string,
    body?: unknown,
    options?: Omit<RequestOptions, "method" | "body">,
  ): Promise<T> {
    this.ensureApiMode();
    return apiClient.put<T>(path, body, options);
  }

  protected patch<T>(
    path: string,
    body?: unknown,
    options?: Omit<RequestOptions, "method" | "body">,
  ): Promise<T> {
    this.ensureApiMode();
    return apiClient.patch<T>(path, body, options);
  }

  protected delete<T>(
    path: string,
    options?: Omit<RequestOptions, "method" | "body">,
  ): Promise<T> {
    this.ensureApiMode();
    return apiClient.delete<T>(path, options);
  }
}

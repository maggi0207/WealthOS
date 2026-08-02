import { apiUrl, isMockApiMode } from "@/config/env";
import { applyAuthHeaders } from "@/services/auth/auth-interceptor";
import { tokenStorage } from "@/services/auth/token-storage";
import { handleApiError } from "@/services/http/api-error-handler";
import {
  ApiError,
  MockApiNotConnectedError,
  parseProblemDetails,
  unwrapApiResponse,
} from "@/services/http/problem-details";
import type { AuthTokensResponse, RequestOptions } from "@/services/http/types";

type RefreshListener = () => void;

let refreshPromise: Promise<boolean> | null = null;
const logoutListeners = new Set<RefreshListener>();

/** Register a callback when API auth is cleared after failed refresh / logout. */
export function onApiAuthLogout(listener: RefreshListener): () => void {
  logoutListeners.add(listener);
  return () => logoutListeners.delete(listener);
}

function notifyLogout(): void {
  for (const listener of logoutListeners) {
    try {
      listener();
    } catch {
      // ignore listener errors
    }
  }
}

async function parseJsonSafe(response: Response): Promise<unknown> {
  const text = await response.text();
  if (!text) return null;
  try {
    return JSON.parse(text) as unknown;
  } catch {
    return null;
  }
}

/**
 * Central HTTP client — all backend traffic must go through this class.
 * Uses fetch (no axios). Handles Bearer injection, 401 refresh retry, and ProblemDetails.
 */
export class ApiClient {
  private buildHeaders(options: RequestOptions): Headers {
    const headers = new Headers(options.headers);

    if (!headers.has("Accept")) {
      headers.set("Accept", "application/json");
    }

    if (options.body !== undefined && !headers.has("Content-Type")) {
      headers.set("Content-Type", "application/json");
    }

    return applyAuthHeaders(headers, options.auth !== false);
  }

  async request<T>(path: string, options: RequestOptions = {}): Promise<T> {
    if (isMockApiMode()) {
      throw new MockApiNotConnectedError("ApiClient");
    }

    const url = apiUrl(path);
    const method = options.method ?? (options.body !== undefined ? "POST" : "GET");

    let response: Response;
    try {
      response = await fetch(url, {
        method,
        headers: this.buildHeaders(options),
        body: options.body !== undefined ? JSON.stringify(options.body) : undefined,
        signal: options.signal,
      });
    } catch (networkError) {
      const error = new ApiError({
        message:
          networkError instanceof Error
            ? networkError.message
            : "Network request failed",
        status: 0,
        code: "network_error",
      });
      if (!options.silent) handleApiError(error);
      throw error;
    }

    if (
      response.status === 401 &&
      options.auth !== false &&
      !options.skipRefresh
    ) {
      const refreshed = await this.tryRefreshTokens();
      if (refreshed) {
        return this.request<T>(path, { ...options, skipRefresh: true });
      }
      tokenStorage.clear();
      notifyLogout();
    }

    const payload = await parseJsonSafe(response);

    if (!response.ok) {
      const error = parseProblemDetails(
        payload,
        response.status,
        response.statusText || "Request failed",
      );
      if (!options.silent) handleApiError(error);
      throw error;
    }

    if (response.status === 204) {
      return undefined as T;
    }

    return unwrapApiResponse<T>(payload, response.status);
  }

  get<T>(path: string, options: Omit<RequestOptions, "method" | "body"> = {}) {
    return this.request<T>(path, { ...options, method: "GET" });
  }

  post<T>(
    path: string,
    body?: unknown,
    options: Omit<RequestOptions, "method" | "body"> = {},
  ) {
    return this.request<T>(path, { ...options, method: "POST", body });
  }

  put<T>(
    path: string,
    body?: unknown,
    options: Omit<RequestOptions, "method" | "body"> = {},
  ) {
    return this.request<T>(path, { ...options, method: "PUT", body });
  }

  patch<T>(
    path: string,
    body?: unknown,
    options: Omit<RequestOptions, "method" | "body"> = {},
  ) {
    return this.request<T>(path, { ...options, method: "PATCH", body });
  }

  delete<T>(path: string, options: Omit<RequestOptions, "method" | "body"> = {}) {
    return this.request<T>(path, { ...options, method: "DELETE" });
  }

  /**
   * Refresh access token using stored refresh token.
   * Single-flight: concurrent 401s share one refresh call.
   */
  async tryRefreshTokens(): Promise<boolean> {
    if (refreshPromise) return refreshPromise;

    refreshPromise = this.executeRefresh().finally(() => {
      refreshPromise = null;
    });

    return refreshPromise;
  }

  private async executeRefresh(): Promise<boolean> {
    const accessToken = tokenStorage.getAccessToken();
    const refreshToken = tokenStorage.getRefreshToken();
    if (!accessToken || !refreshToken) return false;

    try {
      const url = apiUrl("/auth/refresh");
      const response = await fetch(url, {
        method: "POST",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ accessToken, refreshToken }),
      });

      const payload = await parseJsonSafe(response);
      if (!response.ok) return false;

      const data = unwrapApiResponse<AuthTokensResponse>(payload, response.status);
      tokenStorage.setTokens({
        accessToken: data.accessToken,
        refreshToken: data.refreshToken,
        expiresAtUtc: data.expiresAtUtc,
      });
      return true;
    } catch {
      return false;
    }
  }
}

export const apiClient = new ApiClient();

/**
 * Vite environment configuration for WealthOS API integration.
 * Dashboard supports transparent mock ↔ api switching via VITE_API_MODE.
 *
 * Production builds default to `api` so a missing `.env.production` cannot
 * accidentally ship mock fixtures. Set `VITE_ALLOW_MOCK_PROD=true` only for
 * intentional demo/mock production builds.
 */

export type ApiMode = "mock" | "api";

function readMode(raw: string | undefined): ApiMode {
  const allowMockProd = import.meta.env.VITE_ALLOW_MOCK_PROD === "true";
  if (import.meta.env.PROD && !allowMockProd) {
    return "api";
  }
  return raw === "api" ? "api" : "mock";
}

function trimTrailingSlash(url: string): string {
  return url.replace(/\/+$/, "");
}

export const env = {
  apiMode: readMode(import.meta.env.VITE_API_MODE),
  apiBaseUrl: trimTrailingSlash(
    import.meta.env.VITE_API_BASE_URL || "http://localhost:5080",
  ),
  apiVersion: import.meta.env.VITE_API_VERSION || "1",
} as const;

export function isMockApiMode(): boolean {
  return env.apiMode === "mock";
}

export function isApiMode(): boolean {
  return env.apiMode === "api";
}

/** Base path for versioned REST resources, e.g. `/api/v1`. */
export function apiPrefix(): string {
  return `/api/v${env.apiVersion}`;
}

/** Absolute URL for a versioned API path (path may start with `/` or omit it). */
export function apiUrl(path: string): string {
  const normalized = path.startsWith("/") ? path : `/${path}`;
  if (normalized.startsWith("/api/")) {
    return `${env.apiBaseUrl}${normalized}`;
  }
  return `${env.apiBaseUrl}${apiPrefix()}${normalized}`;
}

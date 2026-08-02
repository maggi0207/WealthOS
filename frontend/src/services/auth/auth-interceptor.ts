import { tokenStorage } from "@/services/auth/token-storage";
import { isAccessTokenExpired } from "@/services/auth/jwt";

/**
 * Authentication interceptor helpers used by ApiClient.
 * Keeps Bearer attachment and pre-flight expiry checks out of domain services.
 */

export function getAuthorizationHeader(): Record<string, string> | null {
  const accessToken = tokenStorage.getAccessToken();
  if (!accessToken) return null;
  return { Authorization: `Bearer ${accessToken}` };
}

/** True when a refresh should be attempted before a protected call. */
export function shouldProactivelyRefresh(): boolean {
  const accessToken = tokenStorage.getAccessToken();
  const refreshToken = tokenStorage.getRefreshToken();
  if (!accessToken || !refreshToken) return false;
  return isAccessTokenExpired(accessToken);
}

export function applyAuthHeaders(
  headers: Headers,
  enabled: boolean,
): Headers {
  if (!enabled) return headers;
  const auth = getAuthorizationHeader();
  if (auth?.Authorization) {
    headers.set("Authorization", auth.Authorization);
  }
  return headers;
}

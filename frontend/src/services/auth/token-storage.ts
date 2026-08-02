/**
 * Access / refresh token persistence for backend API auth.
 * Separate from mock-auth session (`wealthos.session`) so mock login keeps working.
 */

const ACCESS_KEY = "wealthos.access_token";
const REFRESH_KEY = "wealthos.refresh_token";
const EXPIRES_KEY = "wealthos.token_expires_at";

function canUseStorage(): boolean {
  return typeof window !== "undefined" && typeof window.localStorage !== "undefined";
}

export type StoredTokens = {
  accessToken: string;
  refreshToken: string;
  expiresAtUtc: string | null;
};

export const tokenStorage = {
  getAccessToken(): string | null {
    if (!canUseStorage()) return null;
    return window.localStorage.getItem(ACCESS_KEY);
  },

  getRefreshToken(): string | null {
    if (!canUseStorage()) return null;
    return window.localStorage.getItem(REFRESH_KEY);
  },

  getExpiresAtUtc(): string | null {
    if (!canUseStorage()) return null;
    return window.localStorage.getItem(EXPIRES_KEY);
  },

  getTokens(): StoredTokens | null {
    const accessToken = this.getAccessToken();
    const refreshToken = this.getRefreshToken();
    if (!accessToken || !refreshToken) return null;
    return {
      accessToken,
      refreshToken,
      expiresAtUtc: this.getExpiresAtUtc(),
    };
  },

  setTokens(tokens: {
    accessToken: string;
    refreshToken: string;
    expiresAtUtc?: string | null;
  }): void {
    if (!canUseStorage()) return;
    window.localStorage.setItem(ACCESS_KEY, tokens.accessToken);
    window.localStorage.setItem(REFRESH_KEY, tokens.refreshToken);
    if (tokens.expiresAtUtc) {
      window.localStorage.setItem(EXPIRES_KEY, tokens.expiresAtUtc);
    } else {
      window.localStorage.removeItem(EXPIRES_KEY);
    }
  },

  clear(): void {
    if (!canUseStorage()) return;
    window.localStorage.removeItem(ACCESS_KEY);
    window.localStorage.removeItem(REFRESH_KEY);
    window.localStorage.removeItem(EXPIRES_KEY);
  },

  hasTokens(): boolean {
    return Boolean(this.getAccessToken() && this.getRefreshToken());
  },
};

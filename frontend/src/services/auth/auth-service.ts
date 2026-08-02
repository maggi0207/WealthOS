import { BaseApiService } from "@/services/http/base-api-service";
import type {
  AuthTokensResponse,
  UserProfile,
} from "@/services/http/types";
import { tokenStorage } from "@/services/auth/token-storage";
import { onApiAuthLogout } from "@/services/http/api-client";

export type LoginRequest = {
  email: string;
  password: string;
};

export type RegisterRequest = {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
};

export type LogoutRequest = {
  refreshToken: string;
};

/**
 * Authentication API service.
 * Token machinery is ready; login pages still use mock-auth until explicitly wired.
 */
class AuthService extends BaseApiService {
  protected readonly serviceName = "AuthService";

  async login(request: LoginRequest): Promise<AuthTokensResponse> {
    const data = await this.post<AuthTokensResponse>("/auth/login", request, {
      auth: false,
    });
    this.persistSession(data);
    return data;
  }

  async register(request: RegisterRequest): Promise<AuthTokensResponse> {
    const data = await this.post<AuthTokensResponse>("/auth/register", request, {
      auth: false,
    });
    this.persistSession(data);
    return data;
  }

  async refresh(): Promise<AuthTokensResponse> {
    const accessToken = tokenStorage.getAccessToken();
    const refreshToken = tokenStorage.getRefreshToken();
    if (!accessToken || !refreshToken) {
      throw new Error("No refresh token available");
    }

    const data = await this.post<AuthTokensResponse>(
      "/auth/refresh",
      { accessToken, refreshToken },
      { auth: false, skipRefresh: true },
    );
    this.persistSession(data);
    return data;
  }

  async logout(): Promise<void> {
    const refreshToken = tokenStorage.getRefreshToken();
    try {
      if (refreshToken) {
        await this.post<unknown>(
          "/auth/logout",
          { refreshToken } satisfies LogoutRequest,
          { auth: false, silent: true, skipRefresh: true },
        );
      }
    } finally {
      tokenStorage.clear();
    }
  }

  async me(): Promise<UserProfile> {
    return this.get<UserProfile>("/auth/me");
  }

  /** Clear local API tokens without calling the backend. */
  clearLocalSession(): void {
    tokenStorage.clear();
  }

  private persistSession(data: AuthTokensResponse): void {
    tokenStorage.setTokens({
      accessToken: data.accessToken,
      refreshToken: data.refreshToken,
      expiresAtUtc: data.expiresAtUtc,
    });
  }
}

export const authService = new AuthService();

export { onApiAuthLogout, tokenStorage };

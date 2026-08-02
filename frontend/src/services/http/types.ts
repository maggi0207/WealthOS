/** Shared HTTP / API envelope types aligned with WealthOS backend contracts. */

export type ApiErrorDetail = {
  code: string;
  message: string;
  field?: string | null;
};

export type ApiResponse<T> = {
  success: boolean;
  message: string;
  data: T | null;
  errors: ApiErrorDetail[];
};

/** RFC 7807 ProblemDetails as returned by GlobalExceptionMiddleware. */
export type ProblemDetails = {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  code?: string;
  success?: boolean;
  errors?: ApiErrorDetail[];
  validationErrors?: Record<string, string[]>;
  [key: string]: unknown;
};

export type AuthTokens = {
  accessToken: string;
  refreshToken: string;
  expiresAtUtc: string;
};

export type UserProfile = {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  displayName?: string | null;
  roles: string[];
  emailConfirmed: boolean;
  isActive: boolean;
};

export type AuthTokensResponse = AuthTokens & {
  user: UserProfile;
};

export type HttpMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE";

export type RequestOptions = {
  method?: HttpMethod;
  body?: unknown;
  headers?: Record<string, string>;
  /** Attach Bearer token (default true). */
  auth?: boolean;
  /** Skip 401 → refresh → retry (default false). */
  skipRefresh?: boolean;
  /** Suppress global toast on error (default false). */
  silent?: boolean;
  signal?: AbortSignal;
};

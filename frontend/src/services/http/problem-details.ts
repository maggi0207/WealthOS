import type { ApiErrorDetail, ApiResponse, ProblemDetails } from "./types";

export class ApiError extends Error {
  readonly status: number;
  readonly code: string;
  readonly details: ApiErrorDetail[];
  readonly problem: ProblemDetails | null;
  readonly fieldErrors: Record<string, string[]>;

  constructor(params: {
    message: string;
    status: number;
    code?: string;
    details?: ApiErrorDetail[];
    problem?: ProblemDetails | null;
    fieldErrors?: Record<string, string[]>;
  }) {
    super(params.message);
    this.name = "ApiError";
    this.status = params.status;
    this.code = params.code ?? "unknown_error";
    this.details = params.details ?? [];
    this.problem = params.problem ?? null;
    this.fieldErrors = params.fieldErrors ?? {};
  }

  get isUnauthorized(): boolean {
    return this.status === 401;
  }

  get isForbidden(): boolean {
    return this.status === 403;
  }

  get isNotFound(): boolean {
    return this.status === 404;
  }

  get isValidation(): boolean {
    return this.status === 422 || this.code === "validation_error";
  }
}

/** Thrown when a service is invoked while `VITE_API_MODE=mock`. */
export class MockApiNotConnectedError extends Error {
  constructor(serviceName: string) {
    super(
      `${serviceName} is not connected — set VITE_API_MODE=api to call the backend.`,
    );
    this.name = "MockApiNotConnectedError";
  }
}

export function isApiError(error: unknown): error is ApiError {
  return error instanceof ApiError;
}

export function isMockApiNotConnectedError(
  error: unknown,
): error is MockApiNotConnectedError {
  return error instanceof MockApiNotConnectedError;
}

/**
 * Parse ASP.NET ProblemDetails (`application/problem+json`) or ApiResponse failures.
 */
export function parseProblemDetails(
  payload: unknown,
  status: number,
  fallbackMessage = "Request failed",
): ApiError {
  if (!payload || typeof payload !== "object") {
    return new ApiError({ message: fallbackMessage, status, code: "http_error" });
  }

  const record = payload as Record<string, unknown>;

  // ApiResponse failure shape
  if ("success" in record && record.success === false) {
    const errors = normalizeErrorDetails(record.errors);
    const message =
      (typeof record.message === "string" && record.message) ||
      errors[0]?.message ||
      fallbackMessage;
    const code = errors[0]?.code || "api_error";
    return new ApiError({
      message,
      status,
      code,
      details: errors,
      fieldErrors: collectFieldErrors(errors),
    });
  }

  // ProblemDetails shape
  const problem = payload as ProblemDetails;
  const extensionErrors = normalizeErrorDetails(problem.errors);
  const validationErrors =
    problem.validationErrors && typeof problem.validationErrors === "object"
      ? (problem.validationErrors as Record<string, string[]>)
      : collectFieldErrors(extensionErrors);

  const code =
    (typeof problem.code === "string" && problem.code) ||
    extensionErrors[0]?.code ||
    "problem_details";

  const message =
    problem.title ||
    problem.detail ||
    extensionErrors[0]?.message ||
    fallbackMessage;

  return new ApiError({
    message,
    status: problem.status ?? status,
    code,
    details: extensionErrors,
    problem,
    fieldErrors: validationErrors,
  });
}

export function unwrapApiResponse<T>(payload: unknown, status: number): T {
  if (!payload || typeof payload !== "object") {
    throw new ApiError({
      message: "Empty response from server",
      status,
      code: "empty_response",
    });
  }

  const response = payload as ApiResponse<T>;

  if ("success" in response) {
    if (!response.success) {
      throw parseProblemDetails(payload, status, response.message || "Request failed");
    }
    return response.data as T;
  }

  // Non-envelope JSON (rare) — return as-is
  return payload as T;
}

function normalizeErrorDetails(value: unknown): ApiErrorDetail[] {
  if (!Array.isArray(value)) return [];
  return value
    .filter((item): item is Record<string, unknown> => !!item && typeof item === "object")
    .map((item) => ({
      code: typeof item.code === "string" ? item.code : "error",
      message: typeof item.message === "string" ? item.message : "Error",
      field: typeof item.field === "string" ? item.field : null,
    }));
}

function collectFieldErrors(details: ApiErrorDetail[]): Record<string, string[]> {
  const map: Record<string, string[]> = {};
  for (const detail of details) {
    if (!detail.field) continue;
    const list = map[detail.field] ?? [];
    list.push(detail.message);
    map[detail.field] = list;
  }
  return map;
}

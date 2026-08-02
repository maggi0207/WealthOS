import { toast } from "sonner";

import { ApiError, isApiError, isMockApiNotConnectedError } from "./problem-details";

export type ApiErrorHandlerOptions = {
  /** Skip toast notification. */
  silent?: boolean;
  /** Override toast message. */
  message?: string;
};

/**
 * Global API error handler — maps ApiError / ProblemDetails to user-facing toasts.
 * Call from ApiClient failures and TanStack Query mutation defaults.
 */
export function handleApiError(
  error: unknown,
  options: ApiErrorHandlerOptions = {},
): ApiError | null {
  if (isMockApiNotConnectedError(error)) {
    if (!options.silent) {
      toast.message("API offline", { description: error.message });
    }
    return null;
  }

  if (!isApiError(error)) {
    if (!options.silent) {
      const message =
        error instanceof Error ? error.message : "Something went wrong";
      toast.error(options.message ?? message);
    }
    return null;
  }

  if (options.silent) return error;

  const message = options.message ?? userFacingMessage(error);

  if (error.isUnauthorized) {
    toast.error(message);
  } else if (error.isValidation) {
    toast.error(message);
  } else if (error.status >= 500) {
    toast.error(message);
  } else {
    toast.error(message);
  }

  return error;
}

function userFacingMessage(error: ApiError): string {
  if (error.isUnauthorized) return "Session expired. Please sign in again.";
  if (error.isForbidden) return "You do not have permission for this action.";
  if (error.isNotFound) return "The requested resource was not found.";
  if (error.isValidation) {
    const firstField = Object.values(error.fieldErrors)[0]?.[0];
    return firstField || error.message || "Please check your input.";
  }
  if (error.status === 0) return "Unable to reach the server. Check your connection.";
  return error.message || "Request failed";
}

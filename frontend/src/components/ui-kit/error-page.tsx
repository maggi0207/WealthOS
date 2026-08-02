import { Link } from "@tanstack/react-router";
import { AlertTriangle, Home, RotateCcw } from "lucide-react";

/**
 * Reusable full-page error surface. Shared by the router's default error
 * boundary, the root error boundary and any route-level errorComponent.
 */
export function ErrorPage({
  title = "This page didn't load",
  description = "Something went wrong on our end. You can retry, or head back home.",
  error,
  onRetry,
}: {
  title?: string;
  description?: string;
  error?: Error;
  onRetry?: () => void;
}) {
  return (
    <main className="flex min-h-svh items-center justify-center bg-background page-gutter py-10">
      <div className="w-full max-w-md text-center">
        <span className="mx-auto grid size-16 place-items-center rounded-3xl bg-destructive/12 text-destructive">
          <AlertTriangle className="size-7" aria-hidden="true" />
        </span>
        <h1 className="mt-5 font-display text-fluid-xl font-semibold tracking-tight text-foreground">{title}</h1>
        <p className="mt-2 text-sm leading-relaxed text-muted-foreground">{description}</p>

        {error?.message && (
          <p className="mt-3 truncate rounded-xl border border-border/60 bg-secondary/40 px-3 py-2 text-xs text-muted-foreground">
            {error.message}
          </p>
        )}

        <div className="mt-6 flex flex-wrap items-center justify-center gap-2">
          {onRetry && (
            <button
              type="button"
              onClick={onRetry}
              className="press inline-flex min-h-11 items-center gap-2 rounded-full bg-primary px-5 text-sm font-semibold text-primary-foreground"
            >
              <RotateCcw className="size-4" aria-hidden="true" />
              Try again
            </button>
          )}
          <Link
            to="/dashboard"
            className="press inline-flex min-h-11 items-center gap-2 rounded-full border border-border/70 bg-secondary/50 px-5 text-sm font-semibold text-foreground"
          >
            <Home className="size-4" aria-hidden="true" />
            Go to home
          </Link>
        </div>
      </div>
    </main>
  );
}

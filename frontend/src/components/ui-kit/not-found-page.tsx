import { Link } from "@tanstack/react-router";
import { Bot, Compass, Home } from "lucide-react";

/** Global 404 surface — polished, on-brand, with a clear route back. */
export function NotFoundPage() {
  return (
    <main className="flex min-h-svh items-center justify-center bg-background page-gutter py-10">
      <div className="w-full max-w-md text-center">
        <span className="mx-auto grid size-16 place-items-center rounded-3xl bg-primary/10 text-primary">
          <Compass className="size-7" aria-hidden="true" />
        </span>
        <p className="mt-5 font-display text-5xl font-bold tracking-tight text-gradient-brand">404</p>
        <h1 className="mt-2 font-display text-fluid-xl font-semibold tracking-tight text-foreground">
          We couldn't find that page
        </h1>
        <p className="mt-2 text-sm leading-relaxed text-muted-foreground">
          The link may be broken, or the screen has moved. Everything else in your workspace is right where you left it.
        </p>

        <div className="mt-6 flex flex-wrap items-center justify-center gap-2">
          <Link
            to="/dashboard"
            className="press inline-flex min-h-11 items-center gap-2 rounded-full bg-primary px-5 text-sm font-semibold text-primary-foreground"
          >
            <Home className="size-4" aria-hidden="true" />
            Go to home
          </Link>
          <Link
            to="/ai-advisor"
            className="press inline-flex min-h-11 items-center gap-2 rounded-full border border-border/70 bg-secondary/50 px-5 text-sm font-semibold text-foreground"
          >
            <Bot className="size-4" aria-hidden="true" />
            Ask the AI Advisor
          </Link>
        </div>
      </div>
    </main>
  );
}

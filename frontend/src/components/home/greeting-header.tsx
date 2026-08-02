import { useEffect, useState } from "react";

import { useAuth } from "@/lib/mock-auth";

function greetingFor(hour: number) {
  if (hour < 12) return "Good morning";
  if (hour < 18) return "Good afternoon";
  return "Good evening";
}

/** Personal, time-aware greeting — the emotional entry point of the home screen. */
export function GreetingHeader() {
  const { user } = useAuth();
  const [now, setNow] = useState<Date | null>(null);

  useEffect(() => setNow(new Date()), []);

  const raw = (user?.name ?? "there").split(" ")[0] ?? "there";
  const firstName = raw.charAt(0).toUpperCase() + raw.slice(1);
  const greeting = now ? greetingFor(now.getHours()) : "Welcome back";
  const dateLabel = now
    ? now.toLocaleDateString("en-US", { weekday: "long", day: "numeric", month: "short" })
    : "\u00a0";

  return (
    <header className="grid grid-cols-[minmax(0,1fr)_auto] items-center gap-3">
      <div className="min-w-0">
        <p className="truncate text-[11px] font-medium uppercase tracking-[0.14em] text-muted-foreground">
          {dateLabel}
        </p>
        <h1 className="mt-0.5 truncate font-display text-fluid-2xl font-semibold leading-tight">
          {greeting}, {firstName}
        </h1>
      </div>
      <span className="grid size-11 shrink-0 place-items-center rounded-2xl bg-primary/12 font-display text-sm font-semibold text-primary">
        {user?.initials ?? "WO"}
      </span>
    </header>
  );
}

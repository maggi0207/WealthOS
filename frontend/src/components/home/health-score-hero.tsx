import { Link } from "@tanstack/react-router";
import { ChevronRight, ShieldCheck } from "lucide-react";

import { Progress } from "@/components/ui/progress";
import { healthScore } from "@/lib/dashboard-data";
import { cn } from "@/lib/utils";

const CIRC = 2 * Math.PI * 42;

/** Hero card: financial health score as a ring, with the driving factors below. */
export function HealthScoreHero() {
  const pct = Math.min(100, Math.max(0, healthScore.score));
  const dash = (pct / 100) * CIRC * 0.75;

  return (
    <section className="surface-hero relative overflow-hidden p-4 sm:p-5">
      <div className="grid grid-cols-[auto_minmax(0,1fr)] items-center gap-4">
        <div className="relative grid size-24 shrink-0 place-items-center sm:size-28">
          <svg viewBox="0 0 100 100" className="size-full -rotate-[135deg]" aria-hidden>
            <circle
              cx="50"
              cy="50"
              r="42"
              fill="none"
              stroke="var(--color-muted)"
              strokeWidth="8"
              strokeLinecap="round"
              strokeDasharray={`${CIRC * 0.75} ${CIRC}`}
            />
            <circle
              cx="50"
              cy="50"
              r="42"
              fill="none"
              stroke="var(--color-primary)"
              strokeWidth="8"
              strokeLinecap="round"
              strokeDasharray={`${dash} ${CIRC}`}
            />
          </svg>
          <div className="absolute inset-0 grid place-items-center">
            <div className="text-center">
              <p className="font-display text-2xl font-semibold leading-none tabular-nums sm:text-3xl">
                {healthScore.score}
              </p>
              <p className="mt-0.5 text-[10px] uppercase tracking-[0.12em] text-muted-foreground">/ 100</p>
            </div>
          </div>
        </div>

        <div className="min-w-0">
          <p className="flex items-center gap-1.5 text-[11px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
            <ShieldCheck className="size-3.5 text-primary" /> Health score
          </p>
          <p className="mt-1 font-display text-lg font-semibold leading-tight">{healthScore.grade}</p>
          <p className="mt-0.5 text-xs text-success">+{healthScore.changePts} pts this month</p>
          <Link
            to="/reports"
            className="press mt-1 inline-flex min-h-11 items-center gap-0.5 text-xs font-medium text-primary"
          >
            See breakdown <ChevronRight className="size-3.5" />
          </Link>
        </div>
      </div>

      <ul className="mt-4 grid gap-2.5 sm:grid-cols-2 sm:gap-x-5">
        {healthScore.factors.map((factor) => (
          <li key={factor.label} className="space-y-1">
            <div className="grid grid-cols-[minmax(0,1fr)_auto] items-center gap-2 text-[11px]">
              <span className="min-w-0 truncate text-muted-foreground">{factor.label}</span>
              <span className={cn("shrink-0 font-medium tabular-nums", factor.value >= 75 ? "text-success" : "")}>
                {factor.value}
              </span>
            </div>
            <Progress value={factor.value} className="h-1" />
          </li>
        ))}
      </ul>
    </section>
  );
}

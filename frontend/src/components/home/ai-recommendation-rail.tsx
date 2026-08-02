import { Link } from "@tanstack/react-router";
import { AlertTriangle, ChevronRight, Sparkles, Wand2, type LucideIcon } from "lucide-react";

import { aiInsights, type Insight } from "@/lib/dashboard-data";
import { cn } from "@/lib/utils";

const TONE: Record<Insight["tone"], { icon: LucideIcon; className: string; label: string }> = {
  opportunity: { icon: Sparkles, className: "bg-primary/12 text-primary", label: "Opportunity" },
  optimisation: { icon: Wand2, className: "bg-chart-2/15 text-chart-2", label: "Optimisation" },
  risk: { icon: AlertTriangle, className: "bg-warning/15 text-warning", label: "Risk" },
};

/** Swipeable AI recommendation rail — the app's advisory voice on the home screen. */
export function AiRecommendationRail() {
  return (
    <div className="no-scrollbar bleed-gutter page-gutter flex snap-x snap-mandatory gap-3 overflow-x-auto pb-1 sm:mx-0 sm:grid sm:grid-cols-3 sm:px-0">
      {aiInsights.map((insight) => {
        const tone = TONE[insight.tone];
        return (
          <article
            key={insight.id}
            className="surface-tile press flex w-[85%] shrink-0 snap-start flex-col gap-2 p-4 sm:w-auto"
          >
            <div className="flex items-center gap-2">
              <span className={cn("grid size-7 shrink-0 place-items-center rounded-lg", tone.className)}>
                <tone.icon className="size-3.5" />
              </span>
              <span className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
                {tone.label}
              </span>
            </div>
            <h3 className="font-display text-sm font-semibold leading-snug">{insight.title}</h3>
            <p className="line-clamp-3 text-xs leading-relaxed text-muted-foreground">{insight.body}</p>
            <div className="mt-auto flex items-center justify-between pt-1">
              <span className="text-xs font-semibold tabular-nums text-primary">{insight.impact}</span>
              <Link
                to="/ai-advisor"
                className="-my-2 inline-flex min-h-11 items-center gap-0.5 text-[11px] font-medium text-muted-foreground"
              >
                Act <ChevronRight className="size-3" />
              </Link>
            </div>
          </article>
        );
      })}
    </div>
  );
}

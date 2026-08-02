import { Link } from "@tanstack/react-router";
import { ArrowUpRight } from "lucide-react";

import { advisorInsights } from "@/lib/advisor-data";
import { cn } from "@/lib/utils";

const toneClass = {
  positive: "bg-primary/12 text-primary",
  caution: "bg-amber-500/12 text-amber-500",
  neutral: "bg-secondary text-muted-foreground",
} as const;

/** Actionable AI insight cards, snap-scrolled on mobile. */
export function InsightRail() {
  return (
    <div className="bleed-gutter no-scrollbar snap-x snap-mandatory overflow-x-auto">
      <div className="flex w-max gap-3 px-[max(var(--page-gutter),env(safe-area-inset-left))]">
        {advisorInsights.map((insight) => (
          <article
            key={insight.id}
            className="surface-tile flex w-[268px] snap-start flex-col p-4 sm:w-[300px]"
          >
            <span
              className={cn(
                "inline-flex w-fit rounded-full px-2 py-0.5 text-[10px] font-semibold uppercase tracking-[0.12em]",
                toneClass[insight.tone],
              )}
            >
              {insight.tag}
            </span>
            <h3 className="mt-2.5 font-display text-[15px] font-semibold leading-snug">{insight.title}</h3>
            <p className="mt-1.5 flex-1 text-[13px] leading-relaxed text-muted-foreground">{insight.body}</p>
            <div className="mt-3 flex items-center justify-between gap-2 border-t border-border/60 pt-3">
              <span className="truncate text-[12.5px] font-semibold text-primary">{insight.impact}</span>
              <Link
                to={insight.action.to}
                className="press inline-flex min-h-11 items-center gap-1 rounded-full bg-secondary/70 px-3 text-[12px] font-semibold"
              >
                {insight.action.label}
                <ArrowUpRight className="size-3.5 text-primary" />
              </Link>
            </div>
          </article>
        ))}
      </div>
    </div>
  );
}

import { ArrowRight, Sparkles } from "lucide-react";
import { toast } from "sonner";

import { goalInsights, type GoalInsight } from "@/lib/goals-data";
import { cn } from "@/lib/utils";

const toneStyle: Record<GoalInsight["tone"], string> = {
  positive: "bg-success/12 text-success",
  caution: "bg-amber-500/12 text-amber-500",
  neutral: "bg-primary/12 text-primary",
};

/** AI suggestions for goal pacing and reallocation. */
export function GoalInsights() {
  return (
    <div className="no-scrollbar bleed-gutter page-gutter flex snap-x snap-mandatory gap-3 overflow-x-auto pb-1 sm:mx-0 sm:grid sm:grid-cols-2 sm:px-0 xl:grid-cols-3">
      {goalInsights.map((insight) => (
        <article
          key={insight.id}
          className="surface-tile flex w-[80vw] max-w-[320px] shrink-0 snap-start flex-col p-4 sm:w-auto sm:max-w-none"
        >
          <div className="flex items-center gap-2">
            <span className="grid size-7 place-items-center rounded-lg bg-primary/10 text-primary">
              <Sparkles className="size-3.5" />
            </span>
            <span className={cn("rounded-full px-2 py-0.5 text-[10px] font-semibold", toneStyle[insight.tone])}>
              {insight.tag}
            </span>
          </div>
          <h3 className="mt-2.5 text-[14px] font-semibold leading-snug">{insight.title}</h3>
          <p className="mt-1 flex-1 text-[12px] leading-relaxed text-muted-foreground">{insight.body}</p>
          <div className="mt-3 flex items-center justify-between gap-2 border-t border-border/60 pt-3">
            <span className="truncate text-[11px] font-semibold tabular-nums text-success">{insight.impact}</span>
            <button
              type="button"
              onClick={() => toast.success(`${insight.action} — mock action queued`)}
              className="press inline-flex min-h-11 shrink-0 items-center gap-1 text-[12px] font-semibold text-primary"
            >
              {insight.action}
              <ArrowRight className="size-3.5" />
            </button>
          </div>
        </article>
      ))}
    </div>
  );
}

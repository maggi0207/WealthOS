import { Lightbulb, Sparkles, TrendingUp } from "lucide-react";

import { aiInsights, type Insight } from "@/lib/property-data";

const tones: Record<Insight["tone"], { icon: typeof Sparkles; className: string }> = {
  positive: { icon: TrendingUp, className: "bg-success/12 text-success" },
  neutral: { icon: Sparkles, className: "bg-primary/12 text-primary" },
  action: { icon: Lightbulb, className: "bg-warning/12 text-warning" },
};

/** Snap-scrolling AI insight cards. */
export function PropertyInsights() {
  return (
    <div className="bleed-gutter no-scrollbar flex snap-x snap-mandatory gap-2.5 overflow-x-auto px-[var(--page-gutter)] pb-0.5">
      {aiInsights.map((insight) => {
        const tone = tones[insight.tone];
        const Icon = tone.icon;
        return (
          <article
            key={insight.id}
            className="surface-tile w-[78%] max-w-[19rem] shrink-0 snap-start p-4 sm:w-[20rem]"
          >
            <span className={`grid size-9 place-items-center rounded-xl ${tone.className}`}>
              <Icon className="size-4" />
            </span>
            <p className="mt-3 text-[13px] leading-snug">{insight.text}</p>
          </article>
        );
      })}
    </div>
  );
}

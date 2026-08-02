import { Banknote, FileCheck2, Home, TrendingUp, Wrench, type LucideIcon } from "lucide-react";

import { timeline, type TimelineEvent } from "@/lib/property-data";

const icons: Record<TimelineEvent["kind"], LucideIcon> = {
  purchase: Home,
  loan: Banknote,
  legal: FileCheck2,
  upkeep: Wrench,
  value: TrendingUp,
};

/** Vertical timeline of important property events. */
export function PropertyTimeline() {
  return (
    <section className="surface-tile p-4">
      <ol className="relative space-y-4 before:absolute before:bottom-3 before:left-[15px] before:top-3 before:w-px before:bg-border">
        {timeline.map((event) => {
          const Icon = icons[event.kind];
          return (
            <li key={event.id} className="relative grid grid-cols-[32px_minmax(0,1fr)] gap-3">
              <span className="z-10 grid size-8 place-items-center rounded-full border border-border bg-card text-muted-foreground">
                <Icon className="size-3.5" />
              </span>
              <div className="min-w-0 pt-0.5">
                <p className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
                  {event.date}
                </p>
                <p className="mt-0.5 text-[14px] font-medium leading-snug">{event.title}</p>
                <p className="text-[12px] leading-snug text-muted-foreground">{event.detail}</p>
              </div>
            </li>
          );
        })}
      </ol>
    </section>
  );
}

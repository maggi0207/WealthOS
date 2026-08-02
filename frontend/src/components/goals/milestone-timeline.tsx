import { PartyPopper } from "lucide-react";

import { fmtDate, goalProgressPct, goals } from "@/lib/goals-data";

/** Milestone timeline across every goal, newest achievement first. */
export function MilestoneTimeline() {
  const items = goals
    .flatMap((goal) =>
      goal.milestones
        .filter((m) => m.reachedOn)
        .map((m) => ({ id: `${goal.id}-${m.id}`, goal: goal.name, label: m.label, on: m.reachedOn!, pct: goalProgressPct(goal) })),
    )
    .sort((a, b) => (a.on > b.on ? -1 : 1))
    .slice(0, 6);

  return (
    <ol className="surface-tile space-y-0 divide-y divide-border/50 overflow-hidden">
      {items.map((item) => (
        <li key={item.id} className="grid grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-3 px-4 py-3">
          <span className="grid size-9 shrink-0 place-items-center rounded-xl bg-success/12 text-success">
            <PartyPopper className="size-4" />
          </span>
          <div className="min-w-0">
            <p className="truncate text-[14px] font-medium">{item.label}</p>
            <p className="truncate text-[11px] text-muted-foreground">{item.goal}</p>
          </div>
          <p className="shrink-0 text-[11px] tabular-nums text-muted-foreground">{fmtDate(item.on)}</p>
        </li>
      ))}
    </ol>
  );
}

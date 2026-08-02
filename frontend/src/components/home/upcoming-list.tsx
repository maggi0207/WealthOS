import { Link } from "@tanstack/react-router";
import { Landmark, Repeat, ShieldCheck, TrendingUp, type LucideIcon } from "lucide-react";

import { SectionHeader } from "@/components/ui-kit/section-header";
import { fmtCurrency, upcomingTasks, type Task } from "@/lib/dashboard-data";
import { cn } from "@/lib/utils";

const ICONS: Record<Task["type"], LucideIcon> = {
  EMI: Landmark,
  SIP: Repeat,
  Bond: TrendingUp,
  Insurance: ShieldCheck,
};

export function UpcomingList({ limit = 4 }: { limit?: number }) {
  const items = upcomingTasks.slice(0, limit);

  return (
    <section>
      <SectionHeader title="Upcoming" action={<Link to="/goals" className="press -my-2 inline-flex min-h-11 items-center">View all</Link>} />
      <ul className="surface-tile divide-y divide-border/50 overflow-hidden">
        {items.map((task) => {
          const urgent = task.dueInDays <= 3;
          const Icon = ICONS[task.type];
          return (
            <li key={task.id} className="grid grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-3 px-4 py-3">
              <span
                className={cn(
                  "grid size-9 shrink-0 place-items-center rounded-xl",
                  urgent ? "bg-warning/15 text-warning" : "bg-muted text-muted-foreground",
                )}
              >
                <Icon className="size-4" />
              </span>
              <div className="min-w-0">
                <p className="truncate text-[13px] font-medium leading-tight">{task.title}</p>
                <p className="mt-0.5 truncate text-[11px] text-muted-foreground">{task.detail}</p>
              </div>
              <div className="shrink-0 text-right">
                {task.amount !== undefined && (
                  <p className="text-[13px] font-semibold tabular-nums">{fmtCurrency(task.amount)}</p>
                )}
                <p className={cn("text-[11px]", urgent ? "text-warning" : "text-muted-foreground")}>
                  {task.due} · {task.dueInDays}d
                </p>
              </div>
            </li>
          );
        })}
      </ul>
    </section>
  );
}

import { Link } from "@tanstack/react-router";
import { ArrowDownLeft, ArrowUpRight, Inbox } from "lucide-react";

import { EmptyState } from "@/components/ui-kit/empty-state";
import { SectionHeader } from "@/components/ui-kit/section-header";
import { ListSkeleton } from "@/components/ui-kit/skeletons";
import { useRecentActivities } from "@/hooks/api/use-dashboard";
import { fmtCurrency } from "@/lib/dashboard-data";
import { cn } from "@/lib/utils";

export function ActivityList({ limit = 5 }: { limit?: number }) {
  const { data, isPending, isError, refetch, isFetching } =
    useRecentActivities(limit);

  return (
    <section>
      <SectionHeader
        title="Recent activity"
        action={
          <Link
            to="/expenses"
            className="press -my-2 inline-flex min-h-11 items-center"
          >
            View all
          </Link>
        }
      />

      {isPending ? (
        <ListSkeleton rows={limit} />
      ) : isError ? (
        <div className="surface-tile px-4 py-6 text-center">
          <p className="text-sm font-medium">Unable to load activity</p>
          <p className="mt-1 text-xs text-muted-foreground">
            Check your connection and try again.
          </p>
          <button
            type="button"
            onClick={() => void refetch()}
            disabled={isFetching}
            className="press mt-3 inline-flex min-h-11 items-center rounded-full bg-primary px-4 text-xs font-semibold text-primary-foreground"
          >
            Retry
          </button>
        </div>
      ) : !data || data.length === 0 ? (
        <EmptyState
          icon={Inbox}
          title="No recent activity"
          description="Income, investments, and payments will show up here."
        />
      ) : (
        <ul className="surface-tile divide-y divide-border/50 overflow-hidden">
          {data.map((item) => {
            const incoming = item.direction === "in";
            return (
              <li
                key={item.id}
                className="grid grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-3 px-4 py-3"
              >
                <span
                  className={cn(
                    "grid size-9 shrink-0 place-items-center rounded-xl",
                    incoming
                      ? "bg-success/12 text-success"
                      : "bg-muted text-muted-foreground",
                  )}
                >
                  {incoming ? (
                    <ArrowDownLeft className="size-4" />
                  ) : (
                    <ArrowUpRight className="size-4" />
                  )}
                </span>
                <div className="min-w-0">
                  <p className="truncate text-[13px] font-medium leading-tight">
                    {item.title}
                  </p>
                  <p className="mt-0.5 truncate text-[11px] text-muted-foreground">
                    {item.category} · {item.time}
                  </p>
                </div>
                <p
                  className={cn(
                    "shrink-0 text-[13px] font-semibold tabular-nums",
                    incoming ? "text-success" : "text-foreground",
                  )}
                >
                  {incoming ? "+" : "−"}
                  {fmtCurrency(item.amount)}
                </p>
              </li>
            );
          })}
        </ul>
      )}
    </section>
  );
}

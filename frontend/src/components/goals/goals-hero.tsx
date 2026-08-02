import { Target } from "lucide-react";

import { HeroSkeleton } from "@/components/ui-kit/skeletons";
import { useGoalsOverview } from "@/hooks/api/use-goals";
import { useCountUp } from "@/hooks/use-count-up";
import { fmtINR, fmtINRShort } from "@/lib/goals-data";

/** Goals overview hero — total saved against target with overall progress. */
export function GoalsHero() {
  const { data, isPending, isError, refetch, isFetching } = useGoalsOverview();
  const summary = data?.summary;
  const value = useCountUp(summary?.totalSaved ?? 0);

  if (isPending) return <HeroSkeleton className="min-h-[12rem]" />;
  if (isError || !summary) {
    return (
      <section className="surface-hero p-4 sm:p-5">
        <p className="text-sm font-medium">Unable to load goals</p>
        <button
          type="button"
          onClick={() => void refetch()}
          disabled={isFetching}
          className="press mt-3 inline-flex min-h-11 items-center rounded-full bg-primary px-4 text-xs font-semibold text-primary-foreground"
        >
          Retry
        </button>
      </section>
    );
  }

  return (
    <section className="surface-hero p-4 sm:p-5">
      <div className="grid grid-cols-[minmax(0,1fr)_auto] items-start gap-3">
        <div className="min-w-0">
          <p className="text-[10px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
            Saved toward goals
          </p>
          <p className="mt-1 font-display text-fluid-2xl font-semibold tabular-nums">
            {fmtINRShort(Math.round(value))}
          </p>
          <p className="mt-1 text-[12px] text-muted-foreground">
            of {fmtINRShort(summary.totalTarget)} across{" "}
            {summary.completed ? `${summary.completed} completed · ` : ""}
            {summary.goalCount} goals
          </p>
        </div>
        <div className="shrink-0 rounded-xl bg-primary/10 px-3 py-2 text-right">
          <p className="text-[10px] font-semibold uppercase tracking-[0.1em] text-muted-foreground">Overall</p>
          <p className="text-[15px] font-semibold tabular-nums text-primary">{summary.overallPct}%</p>
        </div>
      </div>

      <div className="mt-3.5 h-2.5 w-full overflow-hidden rounded-full bg-muted">
        <div
          className="h-full rounded-full bg-gradient-to-r from-primary/70 to-primary transition-[width] duration-700 ease-out"
          style={{ width: `${summary.overallPct}%` }}
        />
      </div>

      <div className="mt-4 grid grid-cols-3 gap-2">
        {[
          { label: "Monthly", value: fmtINR(summary.monthlyCommitted) },
          {
            label: "Completed",
            value: `${summary.completed} of ${summary.goalCount}`,
          },
          {
            label: "Remaining",
            value: fmtINRShort(summary.totalTarget - summary.totalSaved),
          },
        ].map((cell) => (
          <div key={cell.label} className="rounded-xl bg-muted/40 px-3 py-2">
            <p className="truncate text-[10px] font-semibold uppercase tracking-[0.1em] text-muted-foreground">
              {cell.label}
            </p>
            <p className="mt-0.5 truncate text-[13px] font-semibold tabular-nums sm:text-sm">{cell.value}</p>
          </div>
        ))}
      </div>

      <p className="mt-3 flex items-center gap-1.5 text-[11px] text-muted-foreground">
        <Target className="size-3.5 shrink-0 text-primary" />
        Progress updates from your goals ledger.
      </p>
    </section>
  );
}

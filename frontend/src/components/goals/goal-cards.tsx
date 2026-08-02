import { CheckCircle2, GraduationCap, Home, PartyPopper, PiggyBank, Pencil, ShieldCheck, Sunrise, type LucideIcon } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";

import { ContributionFormSheet } from "@/components/goals/contribution-form-sheet";
import { GoalFormSheet } from "@/components/goals/goal-form-sheet";
import { ListSkeleton } from "@/components/ui-kit/skeletons";
import { useGoalsOverview } from "@/hooks/api/use-goals";
import {
  fmtDate,
  fmtINR,
  fmtINRShort,
  goalProgressPct,
  goalCategoryLabel,
  monthsToTarget,
  requiredMonthly,
  type Goal,
  type GoalCategory,
} from "@/lib/goals-data";

const icons: Record<GoalCategory, LucideIcon> = {
  property: Home,
  debt: PiggyBank,
  safety: ShieldCheck,
  education: GraduationCap,
  retirement: Sunrise,
};

/** Goal cards with progress bars, milestones and celebration for completions. */
export function GoalCards() {
  const { data, isPending, isError, refetch, isFetching } = useGoalsOverview();

  if (isPending) return <ListSkeleton rows={4} />;
  if (isError || !data) {
    return (
      <div className="surface-tile px-4 py-6 text-center">
        <p className="text-sm font-medium">Unable to load goals</p>
        <button
          type="button"
          onClick={() => void refetch()}
          disabled={isFetching}
          className="press mt-3 inline-flex min-h-11 items-center rounded-full bg-primary px-4 text-xs font-semibold text-primary-foreground"
        >
          Retry
        </button>
      </div>
    );
  }

  return (
    <div className="grid gap-3 lg:grid-cols-2">
      {data.goals.map((goal) => (
        <GoalCard key={goal.id} goal={goal} />
      ))}
    </div>
  );
}

function GoalCard({ goal }: { goal: Goal }) {
  const Icon = icons[goal.category];
  const pct = goalProgressPct(goal);
  const done = pct >= 100;
  const months = monthsToTarget(goal);
  const needed = requiredMonthly(goal);
  const behind = !done && needed > goal.monthlyContribution;
  const [contribOpen, setContribOpen] = useState(false);
  const [editOpen, setEditOpen] = useState(false);

  return (
    <>
      <article className={`surface-tile p-4 ${done ? "celebrate-glow" : ""}`}>
        <div className="grid grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-3">
          <span
            className={`grid size-10 shrink-0 place-items-center rounded-xl ${
              done ? "bg-success/12 text-success" : "bg-primary/10 text-primary"
            }`}
          >
            {done ? <PartyPopper className="size-4 celebrate-pop" /> : <Icon className="size-4" />}
          </span>
          <div className="min-w-0">
            <p className="truncate text-[15px] font-semibold">{goal.name}</p>
            <p className="truncate text-[11px] text-muted-foreground">
              {goalCategoryLabel[goal.category]} · target {fmtDate(goal.targetDate)}
            </p>
          </div>
          <div className="flex shrink-0 items-center gap-1">
            <button
              type="button"
              aria-label={`Edit ${goal.name}`}
              onClick={() => setEditOpen(true)}
              className="press grid size-8 place-items-center rounded-lg text-muted-foreground"
            >
              <Pencil className="size-3.5" />
            </button>
            <span
              className={`rounded-full px-2 py-0.5 text-[10px] font-semibold ${
                done
                  ? "bg-success/12 text-success"
                  : behind
                    ? "bg-warning/12 text-warning"
                    : "bg-primary/12 text-primary"
              }`}
            >
              {done ? "Achieved" : behind ? "Behind" : "On track"}
            </span>
          </div>
        </div>

        <div className="mt-3 grid grid-cols-[minmax(0,1fr)_auto] items-baseline gap-2">
          <p className="min-w-0 truncate font-display text-lg font-semibold tabular-nums">
            {fmtINRShort(goal.saved)}
          </p>
          <p className="shrink-0 text-[12px] text-muted-foreground tabular-nums">
            of {fmtINRShort(goal.target)} · {pct}%
          </p>
        </div>
        <div className="mt-1.5 h-2 w-full overflow-hidden rounded-full bg-muted">
          <div
            className={`h-full rounded-full transition-[width] duration-700 ease-out ${
              done ? "bg-gradient-to-r from-success/80 to-success" : "bg-primary/75"
            }`}
            style={{ width: `${pct}%` }}
          />
        </div>

        <dl className="mt-3 grid grid-cols-3 gap-2 text-[11px]">
          <div className="min-w-0">
            <dt className="truncate text-muted-foreground">Monthly</dt>
            <dd className="truncate text-[12px] font-semibold tabular-nums">
              {goal.monthlyContribution ? fmtINR(goal.monthlyContribution) : "—"}
            </dd>
          </div>
          <div className="min-w-0">
            <dt className="truncate text-muted-foreground">Needed</dt>
            <dd
              className={`truncate text-[12px] font-semibold tabular-nums ${behind ? "text-warning" : ""}`}
            >
              {needed ? fmtINR(needed) : "—"}
            </dd>
          </div>
          <div className="min-w-0">
            <dt className="truncate text-muted-foreground">Time left</dt>
            <dd className="truncate text-[12px] font-semibold tabular-nums">{months ? `${months} mo` : "Done"}</dd>
          </div>
        </dl>

        <ul className="mt-3 space-y-1.5 border-t border-border/60 pt-3">
          {goal.milestones.map((milestone) => {
            const reached = Boolean(milestone.reachedOn) || pct >= milestone.atPct;
            return (
              <li key={milestone.id} className="grid grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-2 text-[12px]">
                <CheckCircle2
                  className={`size-3.5 shrink-0 ${reached ? "text-success celebrate-pop" : "text-muted-foreground/40"}`}
                />
                <span className={`min-w-0 truncate ${reached ? "" : "text-muted-foreground"}`}>{milestone.label}</span>
                <span className="shrink-0 text-[11px] tabular-nums text-muted-foreground">
                  {milestone.reachedOn ? fmtDate(milestone.reachedOn) : `${milestone.atPct}%`}
                </span>
              </li>
            );
          })}
        </ul>

        <p className="mt-3 rounded-xl bg-muted/40 px-3 py-2 text-[11px] leading-relaxed text-muted-foreground">
          {goal.note}
        </p>

        <button
          type="button"
          onClick={() =>
            done ? toast.success(`🎉 ${goal.name} is fully funded!`) : setContribOpen(true)
          }
          className="press mt-3 inline-flex min-h-11 w-full items-center justify-center rounded-xl bg-muted/60 px-4 text-[13px] font-semibold"
        >
          {done ? "Celebrate" : "Add funds"}
        </button>
      </article>

      <ContributionFormSheet
        open={contribOpen}
        onOpenChange={setContribOpen}
        goal={goal}
      />
      <GoalFormSheet open={editOpen} onOpenChange={setEditOpen} mode="edit" goal={goal} />
    </>
  );
}

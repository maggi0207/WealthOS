import { BadgeIndianRupee, CalendarClock } from "lucide-react";

import { fmtDate, fmtINR, salaryMembers } from "@/lib/business-data";
import { EmptyState } from "@/components/ui-kit/empty-state";

/** Salary card — supports multiple salaried household members. */
export function SalaryCards() {
  if (salaryMembers.length === 0) {
    return (
      <EmptyState
        icon={BadgeIndianRupee}
        title="No salary added"
        description="Add a salaried member to track monthly credits and expected pay dates."
      />
    );
  }

  return (
    <div className="grid gap-3 sm:grid-cols-2">
      {salaryMembers.map((member) => (
        <article key={member.id} className="surface-tile press p-4">
          <div className="grid grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-3">
            <span className="grid size-10 shrink-0 place-items-center rounded-xl bg-primary/10 text-primary">
              <BadgeIndianRupee className="size-4.5" />
            </span>
            <div className="min-w-0">
              <h3 className="truncate text-[15px] font-semibold">{member.memberName}</h3>
              <p className="truncate text-[11px] text-muted-foreground">
                {member.employer} · {member.role}
              </p>
            </div>
            <p className="shrink-0 font-display text-lg font-semibold tabular-nums">{fmtINR(member.monthlySalary)}</p>
          </div>

          <dl className="mt-3 grid grid-cols-2 gap-2 border-t border-border/60 pt-3 text-[11px]">
            <div className="min-w-0">
              <dt className="text-muted-foreground">Last credited</dt>
              <dd className="mt-0.5 truncate font-medium tabular-nums text-success">
                {fmtDate(member.lastCreditedOn)}
              </dd>
            </div>
            <div className="min-w-0">
              <dt className="flex items-center gap-1 text-muted-foreground">
                <CalendarClock className="size-3" /> Next expected
              </dt>
              <dd className="mt-0.5 truncate font-medium tabular-nums">{fmtDate(member.nextExpectedOn)}</dd>
            </div>
          </dl>
        </article>
      ))}
    </div>
  );
}

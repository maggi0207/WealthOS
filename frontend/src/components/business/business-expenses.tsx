import { Repeat } from "lucide-react";

import { businessExpenses, fmtDateShort, fmtINR, totalBusinessExpenses } from "@/lib/business-data";

/** Business expenses — recurring and one-off costs for the month. */
export function BusinessExpenses() {
  return (
    <section className="surface-tile overflow-hidden">
      <ul className="divide-y divide-border/50">
        {businessExpenses.map((expense) => (
          <li key={expense.id} className="grid grid-cols-[minmax(0,1fr)_auto] items-center gap-3 px-4 py-3">
            <div className="min-w-0">
              <p className="flex min-w-0 items-center gap-1.5 text-[14px] font-semibold">
                <span className="truncate">{expense.category}</span>
                {expense.recurring && <Repeat className="size-3 shrink-0 text-muted-foreground" />}
              </p>
              <p className="truncate text-[11px] text-muted-foreground">
                {expense.vendor} · {fmtDateShort(expense.paidOn)}
              </p>
            </div>
            <p className="shrink-0 text-[14px] font-semibold tabular-nums">{fmtINR(expense.amount)}</p>
          </li>
        ))}
      </ul>
      <div className="flex items-center justify-between gap-3 border-t border-border/60 bg-secondary/30 px-4 py-3">
        <span className="text-[11px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
          Total this month
        </span>
        <span className="font-display text-[15px] font-semibold tabular-nums">{fmtINR(totalBusinessExpenses)}</span>
      </div>
    </section>
  );
}

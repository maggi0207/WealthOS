import { ShieldCheck, TrendingDown } from "lucide-react";

import { useCountUp } from "@/hooks/use-count-up";
import { fmtINR, fmtINRShort, loansRepaidPct, loansTotals } from "@/lib/loans-data";

/** Debt overview hero — outstanding, repaid progress and EMI outflow. */
export function DebtHero() {
  const value = useCountUp(loansTotals.outstanding);

  return (
    <section className="surface-hero p-4 sm:p-5">
      <div className="grid grid-cols-[minmax(0,1fr)_auto] items-start gap-3">
        <div className="min-w-0">
          <p className="text-[10px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
            Total outstanding
          </p>
          <p className="mt-1 font-display text-fluid-2xl font-semibold tabular-nums">
            {fmtINRShort(Math.round(value))}
          </p>
          <p className="mt-1 inline-flex items-center gap-1 text-[12px] font-medium text-success">
            <TrendingDown className="size-3.5" />
            {loansRepaidPct}% of {fmtINRShort(loansTotals.borrowed)} repaid
          </p>
        </div>
        <div className="shrink-0 rounded-xl bg-primary/10 px-3 py-2 text-right">
          <p className="text-[10px] font-semibold uppercase tracking-[0.1em] text-muted-foreground">Debt free</p>
          <p className="text-[13px] font-semibold">{loansTotals.debtFreeBy}</p>
        </div>
      </div>

      <div className="mt-3.5 h-2.5 w-full overflow-hidden rounded-full bg-muted">
        <div
          className="h-full rounded-full bg-gradient-to-r from-success/80 to-success transition-[width] duration-700 ease-out"
          style={{ width: `${loansRepaidPct}%` }}
        />
      </div>

      <div className="mt-4 grid grid-cols-3 gap-2">
        {[
          { label: "Monthly EMI", value: fmtINR(loansTotals.monthlyEmi) },
          { label: "Borrowed", value: fmtINRShort(loansTotals.borrowed) },
          { label: "Loans", value: "3" },
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
        <ShieldCheck className="size-3.5 shrink-0 text-success" />
        Mock data — no lender is connected in this demo.
      </p>
    </section>
  );
}

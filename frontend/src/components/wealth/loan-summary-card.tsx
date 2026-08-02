import { fmtINR, fmtINRShort, loanRepaidPct, loanSummary, loans } from "@/lib/wealth-data";

/** Debt-free progress plus per-loan outstanding bars. */
export function LoanSummaryCard() {
  return (
    <section className="surface-tile p-4">
      <div className="grid grid-cols-[minmax(0,1fr)_auto] items-end gap-3">
        <div className="min-w-0">
          <p className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
            Outstanding debt
          </p>
          <p className="mt-1 font-display text-2xl font-semibold tabular-nums">
            {fmtINRShort(loanSummary.outstanding)}
          </p>
        </div>
        <p className="shrink-0 text-right text-[11px] text-muted-foreground">
          Debt-free by
          <span className="block text-[13px] font-semibold text-foreground">{loanSummary.debtFreeBy}</span>
        </p>
      </div>

      <div className="mt-3">
        <div className="flex items-center justify-between text-[11px] font-medium">
          <span className="text-success">{loanRepaidPct}% repaid</span>
          <span className="text-muted-foreground">of {fmtINRShort(loanSummary.borrowed)} borrowed</span>
        </div>
        <div className="mt-1.5 h-2.5 w-full overflow-hidden rounded-full bg-muted">
          <div
            className="h-full rounded-full bg-gradient-to-r from-success/80 to-success transition-[width] duration-700 ease-out"
            style={{ width: `${loanRepaidPct}%` }}
          />
        </div>
      </div>

      <ul className="mt-4 grid gap-3">
        {loans.map((loan) => {
          const paidPct = Math.round(((loan.principal - loan.outstanding) / loan.principal) * 100);
          return (
            <li key={loan.id}>
              <div className="grid grid-cols-[minmax(0,1fr)_auto] items-baseline gap-2">
                <p className="min-w-0 truncate text-[13px] font-medium">{loan.name}</p>
                <p className="shrink-0 text-[13px] font-semibold tabular-nums">{fmtINRShort(loan.outstanding)}</p>
              </div>
              <div className="mt-1.5 h-1.5 w-full overflow-hidden rounded-full bg-muted">
                <div
                  className="h-full rounded-full bg-primary/70 transition-[width] duration-700 ease-out"
                  style={{ width: `${paidPct}%` }}
                />
              </div>
              <p className="mt-1 text-[11px] tabular-nums text-muted-foreground">
                {fmtINR(loan.emi)}/mo · {loan.rate}% · closes {loan.closesIn}
              </p>
            </li>
          );
        })}
      </ul>

      <p className="mt-4 rounded-xl bg-muted/50 px-3 py-2 text-[12px] text-muted-foreground">
        Total EMI outflow{" "}
        <span className="font-semibold tabular-nums text-foreground">{fmtINR(loanSummary.monthlyEmi)}</span> every
        month.
      </p>
    </section>
  );
}

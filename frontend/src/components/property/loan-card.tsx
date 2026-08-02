import { Link } from "@tanstack/react-router";
import { Sparkles } from "lucide-react";
import { toast } from "sonner";

import { fmtINR, fmtINRShort, homeLoan, loanRepaidPct, prepaymentInsight } from "@/lib/property-data";

const rows = [
  { label: "Monthly EMI", value: fmtINR(homeLoan.emi) },
  { label: "Outstanding", value: fmtINRShort(homeLoan.outstanding) },
  { label: "Interest rate", value: `${homeLoan.ratePct}% p.a.` },
  { label: "Payoff date", value: homeLoan.payoffDate },
];

/** Home loan summary + AI prepayment insight. */
export function PropertyLoanCard() {
  return (
    <section className="surface-tile overflow-hidden">
      <div className="p-4">
        <div className="flex items-start justify-between gap-3">
          <div className="min-w-0">
            <h3 className="truncate text-[15px] font-semibold">{homeLoan.lender}</h3>
            <p className="text-[12px] text-muted-foreground">
              {homeLoan.accountMask} · next EMI {homeLoan.nextEmiOn}
            </p>
          </div>
          <Link
            to="/loans"
            className="press -my-2 inline-flex min-h-11 shrink-0 items-center text-xs font-medium text-primary"
          >
            Details
          </Link>
        </div>

        <div className="mt-3">
          <div className="flex items-center justify-between text-[11px] font-medium text-muted-foreground">
            <span>{loanRepaidPct}% repaid</span>
            <span>{homeLoan.tenureMonthsLeft} months left</span>
          </div>
          <div className="mt-1.5 h-2 w-full overflow-hidden rounded-full bg-muted">
            <div
              className="h-full rounded-full bg-primary transition-[width] duration-700 ease-out"
              style={{ width: `${loanRepaidPct}%` }}
            />
          </div>
        </div>

        <dl className="mt-4 grid grid-cols-2 gap-x-3 gap-y-3">
          {rows.map((row) => (
            <div key={row.label} className="min-w-0">
              <dt className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
                {row.label}
              </dt>
              <dd className="mt-0.5 truncate font-display text-[15px] font-semibold tabular-nums">
                {row.value}
              </dd>
            </div>
          ))}
        </dl>
      </div>

      <div className="border-t border-border/70 bg-primary/[0.06] p-4">
        <p className="flex items-center gap-1.5 text-[11px] font-semibold uppercase tracking-[0.12em] text-primary">
          <Sparkles className="size-3.5" />
          AI insight
        </p>
        <p className="mt-1.5 text-[13px] font-semibold leading-snug">{prepaymentInsight.headline}</p>
        <p className="mt-1 text-[12px] leading-relaxed text-muted-foreground">{prepaymentInsight.body}</p>
        <div className="mt-3 flex flex-wrap gap-2">
          <button
            type="button"
            onClick={() => toast.success("Prepayment plan saved to your goals (mock)")}
            className="press inline-flex h-11 items-center rounded-full bg-primary px-4 text-[13px] font-semibold text-primary-foreground"
          >
            Simulate prepayment
          </button>
          <Link
            to="/ai-advisor"
            className="press inline-flex h-11 items-center rounded-full bg-muted px-4 text-[13px] font-semibold"
          >
            Ask advisor
          </Link>
        </div>
      </div>
    </section>
  );
}

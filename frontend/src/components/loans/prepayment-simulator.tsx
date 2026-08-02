import { useState } from "react";
import { Sparkles } from "lucide-react";
import { toast } from "sonner";

import { fmtINR, fmtINRShort, prepaymentPresets, simulatePrepayment, type LoanAccount } from "@/lib/loans-data";

/** Prepayment simulator placeholder — client-side estimate, no backend. */
export function PrepaymentSimulator({ loan }: { loan: LoanAccount }) {
  const [lumpSum, setLumpSum] = useState<number>(prepaymentPresets[0]);
  const result = simulatePrepayment(loan, lumpSum);

  return (
    <section className="surface-tile p-4">
      <div className="flex items-center gap-2">
        <span className="grid size-7 place-items-center rounded-lg bg-primary/10 text-primary">
          <Sparkles className="size-3.5" />
        </span>
        <p className="min-w-0 truncate text-[14px] font-semibold">Prepayment simulator</p>
      </div>
      <p className="mt-1 text-[12px] leading-relaxed text-muted-foreground">
        Estimate for {loan.name} at {loan.ratePct}%. Indicative only — the full simulator ships with the loan editor.
      </p>

      <div className="mt-3 flex flex-wrap gap-2">
        {prepaymentPresets.map((amount) => (
          <button
            key={amount}
            type="button"
            onClick={() => setLumpSum(amount)}
            aria-pressed={amount === lumpSum}
            className={`press inline-flex min-h-11 items-center rounded-full px-4 text-[13px] font-semibold tabular-nums transition-colors ${
              amount === lumpSum
                ? "bg-primary text-primary-foreground"
                : "bg-muted/60 text-muted-foreground"
            }`}
          >
            {fmtINRShort(amount)}
          </button>
        ))}
      </div>

      <div className="mt-3 grid grid-cols-3 gap-2">
        {[
          { label: "Balance", value: fmtINRShort(result.newBalance) },
          { label: "Months", value: `${result.monthsSaved}` },
          { label: "Saved", value: fmtINRShort(result.interestSaved) },
        ].map((cell) => (
          <div key={cell.label} className="rounded-xl bg-muted/40 px-3 py-2">
            <p className="truncate text-[10px] font-semibold uppercase tracking-[0.1em] text-muted-foreground">
              {cell.label}
            </p>
            <p className="mt-0.5 truncate text-[13px] font-semibold tabular-nums">{cell.value}</p>
          </div>
        ))}
      </div>

      <button
        type="button"
        onClick={() => toast.success(`Prepayment of ${fmtINR(lumpSum)} — scheduling arrives with the loan editor`)}
        className="press mt-3 inline-flex min-h-11 w-full items-center justify-center rounded-xl bg-primary px-4 text-[14px] font-semibold text-primary-foreground"
      >
        Plan this prepayment
      </button>
    </section>
  );
}

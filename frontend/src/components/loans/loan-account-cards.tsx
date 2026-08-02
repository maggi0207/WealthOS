import { CalendarClock, Coins, Home, Landmark, type LucideIcon } from "lucide-react";

import {
  fmtDateShort,
  fmtINR,
  fmtINRShort,
  loanAccounts,
  loanKindLabel,
  loanPaidPct,
  type LoanKind,
} from "@/lib/loans-data";

const icons: Record<LoanKind, LucideIcon> = {
  home: Home,
  jewel: Coins,
  personal: Landmark,
};

/** Per-loan cards: balance, EMI, rate, tenure and payoff progress. */
export function LoanAccountCards({
  selectedId,
  onSelect,
}: {
  selectedId: string;
  onSelect: (id: string) => void;
}) {
  return (
    <div className="grid gap-2.5 lg:grid-cols-3">
      {loanAccounts.map((loan) => {
        const Icon = icons[loan.kind];
        const paid = loanPaidPct(loan);
        const active = loan.id === selectedId;
        return (
          <button
            key={loan.id}
            type="button"
            onClick={() => onSelect(loan.id)}
            aria-pressed={active}
            className={`surface-tile press w-full p-4 text-left transition-colors ${
              active ? "ring-1 ring-primary/50" : ""
            }`}
          >
            <div className="grid grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-3">
              <span className="grid size-9 shrink-0 place-items-center rounded-xl bg-primary/10 text-primary">
                <Icon className="size-4" />
              </span>
              <div className="min-w-0">
                <p className="truncate text-[14px] font-semibold">{loanKindLabel[loan.kind]}</p>
                <p className="truncate text-[11px] text-muted-foreground">
                  {loan.lender} · {loan.accountMask}
                </p>
              </div>
              <p className="shrink-0 text-right text-[14px] font-semibold tabular-nums">
                {fmtINRShort(loan.outstanding)}
              </p>
            </div>

            <div className="mt-3 h-1.5 w-full overflow-hidden rounded-full bg-muted">
              <div
                className="h-full rounded-full bg-primary/70 transition-[width] duration-700 ease-out"
                style={{ width: `${paid}%` }}
              />
            </div>
            <div className="mt-1 flex items-center justify-between text-[11px] text-muted-foreground">
              <span className="tabular-nums">{paid}% repaid</span>
              <span className="tabular-nums">of {fmtINRShort(loan.principal)}</span>
            </div>

            <dl className="mt-3 grid grid-cols-3 gap-2 border-t border-border/60 pt-3 text-[11px]">
              <div className="min-w-0">
                <dt className="truncate text-muted-foreground">EMI</dt>
                <dd className="truncate text-[12px] font-semibold tabular-nums">{fmtINR(loan.emi)}</dd>
              </div>
              <div className="min-w-0">
                <dt className="truncate text-muted-foreground">Rate</dt>
                <dd className="truncate text-[12px] font-semibold tabular-nums">{loan.ratePct}%</dd>
              </div>
              <div className="min-w-0">
                <dt className="truncate text-muted-foreground">Tenure left</dt>
                <dd className="truncate text-[12px] font-semibold tabular-nums">{loan.remainingMonths} mo</dd>
              </div>
            </dl>

            <p className="mt-2.5 inline-flex items-center gap-1.5 rounded-full bg-muted/50 px-2.5 py-1 text-[11px] text-muted-foreground">
              <CalendarClock className="size-3" />
              Next EMI {fmtDateShort(loan.nextEmiOn)}
              {loan.autoDebit ? " · auto debit" : " · manual"}
            </p>
          </button>
        );
      })}
    </div>
  );
}

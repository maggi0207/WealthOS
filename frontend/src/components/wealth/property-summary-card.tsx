import { ArrowUpRight, Building2 } from "lucide-react";

import { fmtINR, fmtINRShort, property } from "@/lib/wealth-data";

const equity = property.estimatedValue - property.loanBalance;
const equityPct = Math.round((equity / property.estimatedValue) * 100);
const appreciation = ((property.estimatedValue - property.purchasePrice) / property.purchasePrice) * 100;

/** Property snapshot: value, equity built and the loan still riding on it. */
export function PropertySummaryCard() {
  return (
    <section className="surface-tile overflow-hidden">
      <div className="grid grid-cols-[auto_minmax(0,1fr)] items-start gap-3 p-4">
        <span className="grid size-10 shrink-0 place-items-center rounded-xl bg-primary/12 text-primary">
          <Building2 className="size-5" />
        </span>
        <div className="min-w-0">
          <h3 className="truncate text-[15px] font-semibold">{property.name}</h3>
          <p className="truncate text-[12px] text-muted-foreground">
            {property.locality} · {property.carpetArea} · {property.uds}
          </p>
          <p className="mt-2 font-display text-2xl font-semibold tabular-nums">
            {fmtINRShort(property.estimatedValue)}
          </p>
          <p className="mt-0.5 inline-flex items-center gap-0.5 text-[12px] font-medium tabular-nums text-success">
            <ArrowUpRight className="size-3.5" />
            {appreciation.toFixed(0)}% since {property.purchaseYear}
          </p>
        </div>
      </div>

      <div className="px-4">
        <div className="flex items-center justify-between text-[11px] font-medium">
          <span className="text-muted-foreground">Equity {equityPct}%</span>
          <span className="text-muted-foreground">Loan {fmtINRShort(property.loanBalance)}</span>
        </div>
        <div className="mt-1.5 h-2 w-full overflow-hidden rounded-full bg-muted">
          <div
            className="h-full rounded-full bg-primary transition-[width] duration-700 ease-out"
            style={{ width: `${equityPct}%` }}
          />
        </div>
      </div>

      <dl className="mt-4 grid grid-cols-2 divide-x divide-border/70 border-t border-border/70">
        <div className="px-4 py-3">
          <dt className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">Monthly EMI</dt>
          <dd className="mt-1 font-display text-base font-semibold tabular-nums">{fmtINR(property.monthlyEmi)}</dd>
        </div>
        <div className="px-4 py-3">
          <dt className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">Rent · yield</dt>
          <dd className="mt-1 font-display text-base font-semibold tabular-nums">
            {fmtINR(property.monthlyRent)}
            <span className="ml-1.5 text-[11px] font-medium text-muted-foreground">{property.rentalYield}%</span>
          </dd>
        </div>
      </dl>
    </section>
  );
}

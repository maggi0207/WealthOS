import { Repeat, TrendingUp } from "lucide-react";

import { CompactStat } from "@/components/ui-kit/compact-stat";
import { fmtINR, fmtINRShort, investmentSummary, investments } from "@/lib/wealth-data";

const gain = investmentSummary.current - investmentSummary.invested;
const gainPct = (gain / investmentSummary.invested) * 100;

/** Investment allocation weights + performance snapshot. */
export function InvestmentCards() {
  return (
    <div className="grid gap-3">
      <div className="grid grid-cols-2 gap-3">
        <CompactStat label="Invested" value={fmtINRShort(investmentSummary.invested)} icon={Repeat} />
        <CompactStat
          label="Current"
          value={fmtINRShort(investmentSummary.current)}
          delta={gainPct}
          icon={TrendingUp}
        />
      </div>

      <section className="surface-tile p-4">
        <div className="grid grid-cols-[minmax(0,1fr)_auto] items-center gap-3">
          <div className="min-w-0">
            <p className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
              Portfolio XIRR
            </p>
            <p className="mt-1 font-display text-2xl font-semibold tabular-nums text-success">
              {investmentSummary.xirr}%
            </p>
          </div>
          <p className="shrink-0 text-right text-[11px] text-muted-foreground">
            Monthly SIP
            <span className="block text-[13px] font-semibold tabular-nums text-foreground">
              {fmtINR(investmentSummary.monthlySip)}
            </span>
          </p>
        </div>

        <ul className="mt-4 grid gap-3">
          {investments.map((row) => (
            <li key={row.id}>
              <div className="grid grid-cols-[minmax(0,1fr)_auto] items-baseline gap-2">
                <p className="min-w-0 truncate text-[13px] font-medium">{row.name}</p>
                <p className="shrink-0 text-[13px] font-semibold tabular-nums">{fmtINRShort(row.value)}</p>
              </div>
              <div className="mt-1.5 h-1.5 w-full overflow-hidden rounded-full bg-muted">
                <div
                  className="h-full rounded-full bg-primary transition-[width] duration-700 ease-out"
                  style={{ width: `${Math.min(100, row.weight * 3)}%` }}
                />
              </div>
              <p className="mt-1 text-[11px] tabular-nums text-muted-foreground">
                {row.sleeve} · {row.weight}% of portfolio · XIRR {row.xirr}%
              </p>
            </li>
          ))}
        </ul>
      </section>
    </div>
  );
}

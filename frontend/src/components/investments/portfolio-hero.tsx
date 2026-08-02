import { ArrowDownRight, ArrowUpRight } from "lucide-react";

import { useCountUp } from "@/hooks/use-count-up";
import {
  fmtINR,
  fmtINRShort,
  fmtPctSigned,
  portfolioReturn,
  portfolioReturnPct,
  portfolioSummary,
} from "@/lib/investments-data";
import { cn } from "@/lib/utils";

/** Portfolio summary hero — invested, current value, day move, return and XIRR. */
export function PortfolioHero() {
  const animated = useCountUp(portfolioSummary.current, 1100);
  const up = portfolioSummary.todayChange >= 0;

  return (
    <section className="surface-hero overflow-hidden">
      <div className="px-4 pt-4 sm:px-5 sm:pt-5">
        <p className="text-[11px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
          Current value
        </p>
        <p className="mt-1 truncate font-display text-[1.75rem] font-semibold leading-none tabular-nums sm:text-4xl">
          {fmtINR(Math.round(animated))}
        </p>
        <p className="mt-2 flex flex-wrap items-center gap-x-2 gap-y-1 text-xs">
          <span
            className={cn(
              "inline-flex items-center gap-0.5 rounded-full px-2 py-0.5 font-semibold tabular-nums",
              up ? "bg-success/12 text-success" : "bg-destructive/12 text-destructive",
            )}
          >
            {up ? <ArrowUpRight className="size-3.5" /> : <ArrowDownRight className="size-3.5" />}
            {fmtINR(Math.abs(portfolioSummary.todayChange))} ({Math.abs(portfolioSummary.todayChangePct)}%)
          </span>
          <span className="text-muted-foreground">today</span>
        </p>
      </div>

      <dl className="mt-4 grid grid-cols-2 divide-x divide-y divide-border/70 border-t border-border/70">
        <Cell label="Total invested" value={fmtINRShort(portfolioSummary.invested)} />
        <Cell
          label="Overall return"
          value={fmtINRShort(portfolioReturn)}
          sub={fmtPctSigned(portfolioReturnPct)}
          tone="positive"
        />
        <Cell label="XIRR" value={`${portfolioSummary.xirr}%`} sub="Placeholder" />
        <Cell label="Accounts" value="4" sub="2 connected (mock)" />
      </dl>
    </section>
  );
}

function Cell({
  label,
  value,
  sub,
  tone,
}: {
  label: string;
  value: string;
  sub?: string;
  tone?: "positive";
}) {
  return (
    <div className="px-4 py-3 sm:px-5">
      <dt className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">{label}</dt>
      <dd className="mt-1 truncate font-display text-lg font-semibold tabular-nums">{value}</dd>
      {sub && (
        <p className={cn("text-[11px] font-medium tabular-nums", tone === "positive" ? "text-success" : "text-muted-foreground")}>
          {sub}
        </p>
      )}
    </div>
  );
}

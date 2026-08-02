import { ArrowUpRight, TrendingUp } from "lucide-react";

import { useCountUp } from "@/hooks/use-count-up";
import {
  businessProfit,
  cashFlow,
  fmtINR,
  fmtINRShort,
  savings,
  savingsRate,
  totalIncome,
} from "@/lib/business-data";
import { cn } from "@/lib/utils";

/** Monthly cash flow hero — total income, salary, business revenue, profit, savings rate. */
export function CashFlowHero() {
  const animated = useCountUp(totalIncome, 1100);

  return (
    <section className="surface-hero overflow-hidden">
      <div className="px-4 pt-4 sm:px-5 sm:pt-5">
        <p className="text-[11px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
          Total income · {cashFlow.periodLabel}
        </p>
        <p className="mt-1 truncate font-display text-[1.75rem] font-semibold leading-none tabular-nums sm:text-4xl">
          {fmtINR(Math.round(animated))}
        </p>
        <p className="mt-2 flex flex-wrap items-center gap-x-2 gap-y-1 text-xs">
          <span className="inline-flex items-center gap-0.5 rounded-full bg-success/12 px-2 py-0.5 font-semibold text-success tabular-nums">
            <ArrowUpRight className="size-3.5" />
            {savingsRate.toFixed(0)}% saved
          </span>
          <span className="text-muted-foreground">{fmtINR(savings)} kept this month</span>
        </p>
      </div>

      <dl className="mt-4 grid grid-cols-2 divide-x divide-y divide-border/70 border-t border-border/70">
        <Cell label="Salary" value={fmtINRShort(cashFlow.salaryIncome)} sub="2 members" />
        <Cell label="Business revenue" value={fmtINRShort(cashFlow.businessRevenue)} sub="3 active clients" />
        <Cell label="Business profit" value={fmtINRShort(businessProfit)} sub="39% margin" tone="positive" />
        <Cell label="Savings rate" value={`${savingsRate.toFixed(0)}%`} sub={fmtINRShort(savings)} tone="positive" />
      </dl>

      <p className="flex items-center gap-1.5 border-t border-border/70 px-4 py-2.5 text-[11px] text-muted-foreground sm:px-5">
        <TrendingUp className="size-3.5 shrink-0 text-success" />
        Business revenue up 7.6% vs June
      </p>
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
        <p
          className={cn(
            "truncate text-[11px] font-medium tabular-nums",
            tone === "positive" ? "text-success" : "text-muted-foreground",
          )}
        >
          {sub}
        </p>
      )}
    </div>
  );
}

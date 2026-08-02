import { ArrowUpRight, TrendingUp } from "lucide-react";

import { HeroSkeleton } from "@/components/ui-kit/skeletons";
import { useIncomeOverview } from "@/hooks/api/use-income";
import { useCountUp } from "@/hooks/use-count-up";
import { fmtINR, fmtINRShort } from "@/lib/business-data";
import { cn } from "@/lib/utils";

/** Monthly cash flow hero — total income, salary, business revenue, profit, savings rate. */
export function CashFlowHero() {
  const { data, isPending, isError, refetch, isFetching } = useIncomeOverview();
  const cf = data?.cashFlow;
  const animated = useCountUp(cf?.totalIncome ?? 0, 1100);

  if (isPending) {
    return <HeroSkeleton className="min-h-[14rem]" />;
  }

  if (isError || !cf) {
    return (
      <section className="surface-hero overflow-hidden p-4 sm:p-5">
        <p className="text-sm font-medium">Unable to load income</p>
        <button
          type="button"
          onClick={() => void refetch()}
          disabled={isFetching}
          className="press mt-3 inline-flex min-h-11 items-center rounded-full bg-primary px-4 text-xs font-semibold text-primary-foreground"
        >
          Retry
        </button>
      </section>
    );
  }

  return (
    <section className="surface-hero overflow-hidden">
      <div className="px-4 pt-4 sm:px-5 sm:pt-5">
        <p className="text-[11px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
          Total income · {cf.periodLabel}
        </p>
        <p className="mt-1 truncate font-display text-[1.75rem] font-semibold leading-none tabular-nums sm:text-4xl">
          {fmtINR(Math.round(animated))}
        </p>
        <p className="mt-2 flex flex-wrap items-center gap-x-2 gap-y-1 text-xs">
          <span className="inline-flex items-center gap-0.5 rounded-full bg-success/12 px-2 py-0.5 font-semibold text-success tabular-nums">
            <ArrowUpRight className="size-3.5" />
            {cf.savingsRate.toFixed(0)}% saved
          </span>
          <span className="text-muted-foreground">{fmtINR(cf.savings)} kept this month</span>
        </p>
      </div>

      <dl className="mt-4 grid grid-cols-2 divide-x divide-y divide-border/70 border-t border-border/70">
        <Cell
          label="Salary"
          value={fmtINRShort(cf.salaryIncome)}
          sub={`${data.salaries.length} members`}
        />
        <Cell
          label="Business revenue"
          value={fmtINRShort(cf.businessRevenue)}
          sub={`${cf.activeClientCount} active clients`}
        />
        <Cell
          label="Business profit"
          value={fmtINRShort(cf.businessProfit)}
          sub={`${cf.marginPct}% margin`}
          tone="positive"
        />
        <Cell
          label="Savings rate"
          value={`${cf.savingsRate.toFixed(0)}%`}
          sub={fmtINRShort(cf.savings)}
          tone="positive"
        />
      </dl>

      <p className="flex items-center gap-1.5 border-t border-border/70 px-4 py-2.5 text-[11px] text-muted-foreground sm:px-5">
        <TrendingUp className="size-3.5 shrink-0 text-success" />
        Portfolio cash flow for {cf.periodLabel}
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
      <dt className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
        {label}
      </dt>
      <dd
        className={cn(
          "mt-1 font-display text-[1.05rem] font-semibold tabular-nums sm:text-lg",
          tone === "positive" && "text-success",
        )}
      >
        {value}
      </dd>
      {sub ? <p className="mt-0.5 text-[11px] text-muted-foreground">{sub}</p> : null}
    </div>
  );
}

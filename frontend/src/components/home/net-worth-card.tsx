import { Link } from "@tanstack/react-router";
import { ArrowDownRight, ArrowUpRight, ChevronRight } from "lucide-react";
import { Area, AreaChart, ResponsiveContainer } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import { HeroSkeleton } from "@/components/ui-kit/skeletons";
import { useNetWorth } from "@/hooks/api/use-dashboard";
import { fmtCompact, fmtCurrency, netWorthTrend } from "@/lib/dashboard-data";
import { cn } from "@/lib/utils";

const spark = netWorthTrend.map((point, i) => ({ i, v: point.netWorth }));

/** Wallet-style net worth card: today's move first, breakdown second. */
export function NetWorthCard() {
  const { data, isPending, isError, refetch, isFetching } = useNetWorth();

  if (isPending) {
    return <HeroSkeleton className="min-h-[14rem]" />;
  }

  if (isError || !data) {
    return (
      <section className="surface-hero overflow-hidden p-4 sm:p-5">
        <p className="text-sm font-medium">Unable to load net worth</p>
        <p className="mt-1 text-xs text-muted-foreground">
          Check your connection and try again.
        </p>
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

  const up = data.netWorthToday.amount >= 0;

  return (
    <section className="surface-hero overflow-hidden">
      <div className="px-4 pt-4 sm:px-5 sm:pt-5">
        <div className="grid grid-cols-[minmax(0,1fr)_auto] items-start gap-3">
          <div className="min-w-0">
            <p className="text-[11px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">Net worth</p>
            <p className="mt-1 truncate font-display text-[1.9rem] font-semibold leading-none tabular-nums sm:text-4xl">
              {fmtCurrency(data.netWorth, { currencyCode: data.currencyCode })}
            </p>
            <p className="mt-2 flex flex-wrap items-center gap-x-2 gap-y-1 text-xs">
              <span
                className={cn(
                  "inline-flex items-center gap-0.5 rounded-full px-2 py-0.5 font-semibold tabular-nums",
                  up ? "bg-success/12 text-success" : "bg-destructive/12 text-destructive",
                )}
              >
                {up ? <ArrowUpRight className="size-3.5" /> : <ArrowDownRight className="size-3.5" />}
                {fmtCurrency(Math.abs(data.netWorthToday.amount), {
                  currencyCode: data.currencyCode,
                })} (
                {Math.abs(data.netWorthToday.changePct).toFixed(2)}%)
              </span>
              <span className="text-muted-foreground">today</span>
            </p>
          </div>
          <Link
            to="/reports"
            aria-label="Open reports"
            className="press grid size-11 shrink-0 md:size-9 place-items-center rounded-xl bg-muted/60 text-muted-foreground"
          >
            <ChevronRight className="size-4" />
          </Link>
        </div>
      </div>

      <ChartFrame height={92} mobileHeight={76}>
        <ResponsiveContainer width="100%" height="100%">
          <AreaChart data={spark} margin={{ top: 8, right: 0, bottom: 0, left: 0 }}>
            <defs>
              <linearGradient id="nw-hero" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stopColor="var(--color-primary)" stopOpacity={0.45} />
                <stop offset="100%" stopColor="var(--color-primary)" stopOpacity={0} />
              </linearGradient>
            </defs>
            <Area
              type="monotone"
              dataKey="v"
              stroke="var(--color-primary)"
              strokeWidth={2}
              fill="url(#nw-hero)"
              isAnimationActive={false}
              dot={false}
            />
          </AreaChart>
        </ResponsiveContainer>
      </ChartFrame>

      <div className="grid grid-cols-2 divide-x divide-border/50 border-t border-border/50">
        <Link to="/assets" className="press px-4 py-3 sm:px-5">
          <p className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">Assets</p>
          <p className="mt-0.5 truncate font-display text-base font-semibold tabular-nums">
            {fmtCompact(data.assetValue, data.currencyCode)}
          </p>
        </Link>
        <Link to="/loans" className="press px-4 py-3 sm:px-5">
          <p className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">Liabilities</p>
          <p className="mt-0.5 truncate font-display text-base font-semibold tabular-nums">
            {fmtCompact(data.liabilityValue, data.currencyCode)}
          </p>
        </Link>
      </div>
    </section>
  );
}

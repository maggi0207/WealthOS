import { Link } from "@tanstack/react-router";
import { ArrowUpRight, ChevronRight } from "lucide-react";
import { Area, AreaChart, ResponsiveContainer } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import { useCountUp } from "@/hooks/use-count-up";
import { fmtINR, fmtINRShort, netWorthSeries, wealthSummary } from "@/lib/wealth-data";

const spark = netWorthSeries.map((point, i) => ({ i, v: point.value }));

/** Wealth hero: total net worth in INR with today's move and the split. */
export function NetWorthHero() {
  const animatedNetWorth = useCountUp(wealthSummary.netWorth, 1100);

  return (

    <section className="surface-hero overflow-hidden">
      <div className="px-4 pt-4 sm:px-5 sm:pt-5">
        <div className="grid grid-cols-[minmax(0,1fr)_auto] items-start gap-3">
          <div className="min-w-0">
            <p className="text-[11px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
              Total net worth
            </p>
            <p className="mt-1 truncate font-display text-[1.75rem] font-semibold leading-none tabular-nums sm:text-4xl">
              {fmtINR(Math.round(animatedNetWorth))}
            </p>
            <p className="mt-2 flex flex-wrap items-center gap-x-2 gap-y-1 text-xs">
              <span className="inline-flex items-center gap-0.5 rounded-full bg-success/12 px-2 py-0.5 font-semibold tabular-nums text-success">
                <ArrowUpRight className="size-3.5" />
                {fmtINR(wealthSummary.todayChange)} ({wealthSummary.todayChangePct}%)
              </span>
              <span className="text-muted-foreground">today · YTD +{wealthSummary.ytdChangePct}%</span>
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
              <linearGradient id="wealth-hero" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stopColor="var(--color-primary)" stopOpacity={0.45} />
                <stop offset="100%" stopColor="var(--color-primary)" stopOpacity={0} />
              </linearGradient>
            </defs>
            <Area
              type="monotone"
              dataKey="v"
              stroke="var(--color-primary)"
              strokeWidth={2}
              fill="url(#wealth-hero)"
              isAnimationActive
              animationDuration={900}
            />
          </AreaChart>
        </ResponsiveContainer>
      </ChartFrame>

      <dl className="grid grid-cols-2 divide-x divide-border/70 border-t border-border/70">
        <div className="px-4 py-3 sm:px-5">
          <dt className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">Assets</dt>
          <dd className="mt-1 font-display text-lg font-semibold tabular-nums">
            {fmtINRShort(wealthSummary.assets)}
          </dd>
        </div>
        <div className="px-4 py-3 sm:px-5">
          <dt className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">Liabilities</dt>
          <dd className="mt-1 font-display text-lg font-semibold tabular-nums">
            {fmtINRShort(wealthSummary.liabilities)}
          </dd>
        </div>
      </dl>
    </section>
  );
}

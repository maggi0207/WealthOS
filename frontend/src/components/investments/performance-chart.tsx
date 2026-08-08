import { useState } from "react";
import { Area, AreaChart, CartesianGrid, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import { ChartTooltip } from "@/components/dashboard/chart-tooltip";
import { TileSkeleton } from "@/components/ui-kit/skeletons";
import { useInvestmentPerformance } from "@/hooks/api/use-investments";
import { cn } from "@/lib/utils";

const ranges = ["1M", "6M", "1Y", "All"] as const;
type PerfRange = (typeof ranges)[number];

const rangeQuery: Record<PerfRange, string> = {
  "1M": "OneMonth",
  "6M": "SixMonths",
  "1Y": "OneYear",
  All: "All",
};

/** Portfolio performance over time from real snapshots (₹ lakh). */
export function PerformanceChart() {
  const [range, setRange] = useState<PerfRange>("6M");
  const { data, isPending, isError, refetch, isFetching } = useInvestmentPerformance(rangeQuery[range]);

  if (isPending) return <TileSkeleton className="h-[16rem]" />;

  if (isError) {
    return (
      <section className="surface-tile px-4 py-6 text-center">
        <p className="text-sm font-medium">Unable to load performance</p>
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

  const points = data?.points ?? [];

  return (
    <section className="surface-tile p-4">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <p className="text-[13px] font-semibold">Portfolio value</p>
        <div className="inline-flex rounded-full bg-secondary/70 p-1" role="tablist" aria-label="Performance range">
          {ranges.map((r) => (
            <button
              key={r}
              type="button"
              role="tab"
              aria-selected={range === r}
              onClick={() => setRange(r)}
              className={cn(
                "press min-h-9 rounded-full px-3 text-[12px] font-semibold transition-colors",
                range === r ? "bg-primary text-primary-foreground" : "text-muted-foreground",
              )}
            >
              {r}
            </button>
          ))}
        </div>
      </div>

      {points.length === 0 ? (
        <p className="mt-6 pb-2 text-center text-sm text-muted-foreground">
          No performance history yet. Add holdings or sync Angel One to build this chart.
        </p>
      ) : (
        <div className="mt-3">
          <ChartFrame height={220} mobileHeight={180}>
            <ResponsiveContainer width="100%" height="100%">
              <AreaChart data={points} margin={{ top: 8, right: 4, left: -8, bottom: 0 }}>
                <defs>
                  <linearGradient id="perfFill" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="0%" stopColor="var(--color-primary)" stopOpacity={0.36} />
                    <stop offset="100%" stopColor="var(--color-primary)" stopOpacity={0} />
                  </linearGradient>
                </defs>
                <CartesianGrid stroke="var(--color-border)" strokeDasharray="3 3" vertical={false} />
                <XAxis
                  dataKey="label"
                  tickLine={false}
                  axisLine={false}
                  tick={{ fontSize: 11, fill: "var(--color-muted-foreground)" }}
                />
                <YAxis
                  tickLine={false}
                  axisLine={false}
                  width={54}
                  tick={{ fontSize: 11, fill: "var(--color-muted-foreground)" }}
                  tickFormatter={(v: number) => `${v}L`}
                />
                <Tooltip content={<ChartTooltip formatter={(v) => `₹${Number(v).toFixed(1)} L`} />} />
                <Area
                  type="monotone"
                  dataKey="value"
                  name="Value"
                  stroke="var(--color-primary)"
                  strokeWidth={2}
                  fill="url(#perfFill)"
                  isAnimationActive
                  animationDuration={800}
                />
              </AreaChart>
            </ResponsiveContainer>
          </ChartFrame>
        </div>
      )}
    </section>
  );
}

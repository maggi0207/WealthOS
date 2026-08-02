import { useState } from "react";
import { Area, AreaChart, CartesianGrid, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import { ChartTooltip } from "@/components/dashboard/chart-tooltip";
import { perfRanges, performanceSeries, type PerfRange } from "@/lib/investments-data";
import { cn } from "@/lib/utils";

/** Portfolio performance over time, in ₹ lakh. */
export function PerformanceChart() {
  const [range, setRange] = useState<PerfRange>("6M");
  const data = performanceSeries[range];

  return (
    <section className="surface-tile p-4">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <p className="text-[13px] font-semibold">Portfolio value</p>
        <div className="inline-flex rounded-full bg-secondary/70 p-1" role="tablist" aria-label="Performance range">
          {perfRanges.map((r) => (
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

      <div className="mt-3">
        <ChartFrame height={220} mobileHeight={180}>
          <ResponsiveContainer width="100%" height="100%">
            <AreaChart data={data} margin={{ top: 8, right: 4, left: -8, bottom: 0 }}>
              <defs>
                <linearGradient id="perfFill" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="0%" stopColor="var(--color-primary)" stopOpacity={0.36} />
                  <stop offset="100%" stopColor="var(--color-primary)" stopOpacity={0} />
                </linearGradient>
              </defs>
              <CartesianGrid stroke="var(--color-border)" strokeDasharray="3 3" vertical={false} />
              <XAxis dataKey="label" tickLine={false} axisLine={false} tick={{ fontSize: 11, fill: "var(--color-muted-foreground)" }} />
              <YAxis
                tickLine={false}
                axisLine={false}
                width={54}
                tick={{ fontSize: 11, fill: "var(--color-muted-foreground)" }}
                tickFormatter={(v: number) => `${v}L`}
              />
              <Tooltip content={<ChartTooltip formatter={(v) => `₹${v.toFixed(1)} L`} />} />
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
    </section>
  );
}

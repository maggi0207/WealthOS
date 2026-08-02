import { Area, AreaChart, CartesianGrid, Line, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import { ChartTooltip } from "@/components/dashboard/chart-tooltip";
import { valueSeries } from "@/lib/property-data";

/** Purchase price vs market value over time (₹ lakh). */
export function PropertyValueChart() {
  return (
    <section className="surface-tile p-3 pr-4 sm:p-4">
      <div className="mb-1 flex flex-wrap items-center gap-x-4 gap-y-1 px-1 text-[11px] font-medium">
        <span className="inline-flex items-center gap-1.5">
          <span className="size-2 rounded-full bg-[var(--color-chart-1)]" />
          <span className="text-muted-foreground">Market value</span>
        </span>
        <span className="inline-flex items-center gap-1.5">
          <span className="size-2 rounded-full bg-muted-foreground/60" />
          <span className="text-muted-foreground">Purchase price</span>
        </span>
      </div>

      <ChartFrame height={220} mobileHeight={168}>
        <ResponsiveContainer width="100%" height="100%">
          <AreaChart data={valueSeries} margin={{ top: 8, right: 4, bottom: 0, left: -18 }}>
            <defs>
              <linearGradient id="prop-value" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stopColor="var(--color-chart-1)" stopOpacity={0.4} />
                <stop offset="100%" stopColor="var(--color-chart-1)" stopOpacity={0} />
              </linearGradient>
            </defs>
            <CartesianGrid vertical={false} stroke="var(--color-border)" strokeOpacity={0.5} />
            <XAxis
              dataKey="year"
              tickLine={false}
              axisLine={false}
              tick={{ fontSize: 10, fill: "var(--color-muted-foreground)" }}
              interval="preserveStartEnd"
            />
            <YAxis
              tickLine={false}
              axisLine={false}
              width={46}
              tick={{ fontSize: 10, fill: "var(--color-muted-foreground)" }}
              tickFormatter={(v: number) => `${v}L`}
            />
            <Tooltip
              cursor={{ stroke: "var(--color-border)" }}
              content={<ChartTooltip formatter={(v) => `₹${v} L`} />}
            />
            <Area
              type="monotone"
              dataKey="market"
              name="Market value"
              stroke="var(--color-chart-1)"
              strokeWidth={2}
              fill="url(#prop-value)"
              animationDuration={900}
            />
            <Line
              type="monotone"
              dataKey="purchase"
              name="Purchase price"
              stroke="var(--color-muted-foreground)"
              strokeWidth={1.5}
              strokeDasharray="4 4"
              dot={false}
              animationDuration={900}
            />
          </AreaChart>
        </ResponsiveContainer>
      </ChartFrame>
    </section>
  );
}

import { Bar, BarChart, CartesianGrid, Legend, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import { ChartTooltip } from "@/components/dashboard/chart-tooltip";
import { incomeTrend } from "@/lib/business-data";

/** Income trend by month — salary vs business revenue, in ₹ thousand. */
export function IncomeTrendChart() {
  return (
    <section className="surface-tile p-4">
      <p className="text-[13px] font-semibold">Income by month</p>
      <p className="mt-0.5 text-[11px] text-muted-foreground">Salary vs business revenue (₹ thousand)</p>

      <div className="mt-3">
        <ChartFrame height={230} mobileHeight={190}>
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={incomeTrend} margin={{ top: 8, right: 4, left: -12, bottom: 0 }}>
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
                width={46}
                tick={{ fontSize: 11, fill: "var(--color-muted-foreground)" }}
                tickFormatter={(v: number) => `${v}K`}
              />
              <Tooltip cursor={{ fill: "var(--color-secondary)", opacity: 0.4 }} content={<ChartTooltip formatter={(v) => `₹${v} K`} />} />
              <Legend wrapperStyle={{ fontSize: 11 }} iconType="circle" iconSize={8} />
              <Bar dataKey="salary" name="Salary" stackId="a" fill="var(--color-primary)" radius={[0, 0, 0, 0]} isAnimationActive animationDuration={750} />
              <Bar dataKey="business" name="Business" stackId="a" fill="var(--color-success)" radius={[6, 6, 0, 0]} isAnimationActive animationDuration={750} />
            </BarChart>
          </ResponsiveContainer>
        </ChartFrame>
      </div>
    </section>
  );
}

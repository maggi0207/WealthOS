import { Area, AreaChart, CartesianGrid, Line, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import { ChartTooltip } from "@/components/dashboard/chart-tooltip";
import { ChartSkeleton } from "@/components/ui-kit/skeletons";
import { usePrimaryProperty } from "@/hooks/api/use-properties";

/** Purchase price vs market value over time (₹ lakh). */
export function PropertyValueChart() {
  const { data, isPending, isError, refetch, isFetching } = usePrimaryProperty();

  if (isPending) {
    return <ChartSkeleton height={220} />;
  }

  if (isError || !data) {
    return (
      <section className="surface-tile px-4 py-5 text-center">
        <p className="text-sm font-medium">Unable to load value chart</p>
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

  const valueSeries = data.valueSeries;

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

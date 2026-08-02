import { Area, AreaChart, CartesianGrid, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import { ChartTooltip } from "@/components/dashboard/chart-tooltip";
import { PanelCard } from "@/components/dashboard/panel-card";
import { Badge } from "@/components/ui/badge";
import { fmtCompact, fmtCurrency, netWorthTrend } from "@/lib/dashboard-data";

const axisProps = {
  stroke: "var(--color-muted-foreground)",
  fontSize: 11,
  tickLine: false,
  axisLine: false,
} as const;

export function NetWorthTrendChart() {
  return (
    <PanelCard
      title="Net worth trend"
      subtitle="Assets vs liabilities over the last 9 months"
      actions={<Badge variant="secondary">YTD +12.4%</Badge>}
    >
      <ChartFrame height={280} mobileHeight={190}>
        <ResponsiveContainer width="100%" height="100%">
          <AreaChart data={netWorthTrend} margin={{ top: 8, right: 8, left: -12, bottom: 0 }}>
            <defs>
              <linearGradient id="nw-area" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stopColor="var(--color-primary)" stopOpacity={0.4} />
                <stop offset="100%" stopColor="var(--color-primary)" stopOpacity={0.02} />
              </linearGradient>
              <linearGradient id="lb-area" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stopColor="var(--color-chart-3)" stopOpacity={0.28} />
                <stop offset="100%" stopColor="var(--color-chart-3)" stopOpacity={0.02} />
              </linearGradient>
            </defs>
            <CartesianGrid strokeDasharray="3 3" stroke="var(--color-border)" vertical={false} />
            <XAxis dataKey="month" {...axisProps} />
            <YAxis {...axisProps} width={56} tickFormatter={(v: number) => fmtCompact(v)} />
            <Tooltip
              cursor={{ stroke: "var(--color-border)" }}
              content={<ChartTooltip formatter={(v) => fmtCurrency(v)} />}
            />
            <Area
              type="monotone"
              dataKey="netWorth"
              name="Net worth"
              stroke="var(--color-primary)"
              strokeWidth={2.2}
              fill="url(#nw-area)"
            />
            <Area
              type="monotone"
              dataKey="liabilities"
              name="Liabilities"
              stroke="var(--color-chart-3)"
              strokeWidth={1.8}
              fill="url(#lb-area)"
            />
          </AreaChart>
        </ResponsiveContainer>
      </ChartFrame>
    </PanelCard>
  );
}

import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import { ChartTooltip } from "@/components/dashboard/chart-tooltip";
import { PanelCard } from "@/components/dashboard/panel-card";
import { assetAllocation, fmtCompact, fmtCurrency } from "@/lib/dashboard-data";

const COLORS = [
  "var(--color-chart-1)",
  "var(--color-chart-2)",
  "var(--color-chart-3)",
  "var(--color-chart-4)",
  "var(--color-chart-5)",
];

const total = assetAllocation.reduce((sum, slice) => sum + slice.value, 0);

export function AssetAllocationChart() {
  return (
    <PanelCard title="Asset allocation" subtitle={`${fmtCompact(total)} across 5 classes`}>
      <div className="grid gap-4 sm:grid-cols-[minmax(0,180px)_minmax(0,1fr)] sm:items-center xl:grid-cols-1 2xl:grid-cols-[minmax(0,180px)_minmax(0,1fr)]">
        <div className="relative">
          <ChartFrame height={180} mobileHeight={160}>
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Tooltip content={<ChartTooltip formatter={(v) => fmtCurrency(v)} />} />
                <Pie
                  data={assetAllocation}
                  dataKey="value"
                  nameKey="name"
                  innerRadius={54}
                  outerRadius={80}
                  paddingAngle={2}
                  stroke="none"
                >
                  {assetAllocation.map((slice, i) => (
                    <Cell key={slice.key} fill={COLORS[i % COLORS.length]} />
                  ))}
                </Pie>
              </PieChart>
            </ResponsiveContainer>
          </ChartFrame>
          <div className="pointer-events-none absolute inset-0 grid place-items-center">
            <div className="text-center">
              <p className="text-[10px] uppercase tracking-widest text-muted-foreground">Total</p>
              <p className="font-display text-lg font-semibold tabular-nums">{fmtCompact(total)}</p>
            </div>
          </div>
        </div>

        <ul className="grid gap-2 xl:grid-cols-2 2xl:grid-cols-1">
          {assetAllocation.map((slice, i) => (
            <li key={slice.key} className="grid grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-2 text-sm">
              <span className="size-2.5 shrink-0 rounded-full" style={{ backgroundColor: COLORS[i % COLORS.length] }} />
              <span className="min-w-0 truncate">{slice.name}</span>
              <span className="shrink-0 tabular-nums text-muted-foreground">
                {Math.round((slice.value / total) * 100)}%
              </span>
            </li>
          ))}
        </ul>
      </div>
    </PanelCard>
  );
}

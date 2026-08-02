import { Bar, BarChart, CartesianGrid, Legend, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import { ChartTooltip } from "@/components/dashboard/chart-tooltip";
import { PanelCard } from "@/components/dashboard/panel-card";
import { Badge } from "@/components/ui/badge";
import { fmtCompact, fmtCurrency, incomeVsExpenses } from "@/lib/dashboard-data";

const axisProps = {
  stroke: "var(--color-muted-foreground)",
  fontSize: 11,
  tickLine: false,
  axisLine: false,
} as const;

const avgSavings = Math.round(
  (incomeVsExpenses.reduce((s, m) => s + (m.income - m.expenses) / m.income, 0) / incomeVsExpenses.length) * 100,
);

export function IncomeVsExpensesChart() {
  return (
    <PanelCard
      title="Income vs expenses"
      subtitle="Last 6 months of cashflow"
      actions={<Badge variant="secondary">{avgSavings}% avg savings</Badge>}
    >
      <ChartFrame height={260} mobileHeight={185}>
        <ResponsiveContainer width="100%" height="100%">
          <BarChart data={incomeVsExpenses} margin={{ top: 8, right: 8, left: -12, bottom: 0 }} barGap={6}>
            <CartesianGrid strokeDasharray="3 3" stroke="var(--color-border)" vertical={false} />
            <XAxis dataKey="month" {...axisProps} />
            <YAxis {...axisProps} width={56} tickFormatter={(v: number) => fmtCompact(v)} />
            <Tooltip
              cursor={{ fill: "var(--color-muted)", opacity: 0.35 }}
              content={<ChartTooltip formatter={(v) => fmtCurrency(v)} />}
            />
            <Legend
              iconType="circle"
              iconSize={8}
              wrapperStyle={{ fontSize: 12, color: "var(--color-muted-foreground)", paddingTop: 8 }}
            />
            <Bar dataKey="income" name="Income" fill="var(--color-chart-1)" radius={[6, 6, 0, 0]} maxBarSize={26} />
            <Bar dataKey="expenses" name="Expenses" fill="var(--color-chart-5)" radius={[6, 6, 0, 0]} maxBarSize={26} />
          </BarChart>
        </ResponsiveContainer>
      </ChartFrame>
    </PanelCard>
  );
}

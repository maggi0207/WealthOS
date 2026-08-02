import { Bar, BarChart, CartesianGrid, Cell, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import { ChartTooltip } from "@/components/dashboard/chart-tooltip";
import { PanelCard } from "@/components/dashboard/panel-card";
import { fmtCompact, fmtCurrency, loanBreakdown } from "@/lib/dashboard-data";

const COLORS = ["var(--color-chart-2)", "var(--color-chart-4)", "var(--color-chart-3)", "var(--color-chart-5)"];

const totalDebt = loanBreakdown.reduce((s, l) => s + l.outstanding, 0);
const totalEmi = loanBreakdown.reduce((s, l) => s + l.emi, 0);

export function LoanBreakdownChart() {
  return (
    <PanelCard
      title="Loan breakdown"
      subtitle={`${fmtCompact(totalDebt)} outstanding · ${fmtCurrency(totalEmi)} monthly EMI`}
    >
      <ChartFrame height={200} mobileHeight={170}>
        <ResponsiveContainer width="100%" height="100%">
          <BarChart
            data={loanBreakdown}
            layout="vertical"
            margin={{ top: 4, right: 12, left: 8, bottom: 0 }}
          >
            <CartesianGrid strokeDasharray="3 3" stroke="var(--color-border)" horizontal={false} />
            <XAxis
              type="number"
              stroke="var(--color-muted-foreground)"
              fontSize={11}
              tickLine={false}
              axisLine={false}
              tickFormatter={(v: number) => fmtCompact(v)}
            />
            <YAxis
              type="category"
              dataKey="name"
              stroke="var(--color-muted-foreground)"
              fontSize={11}
              tickLine={false}
              axisLine={false}
              width={104}
            />
            <Tooltip
              cursor={{ fill: "var(--color-muted)", opacity: 0.35 }}
              content={<ChartTooltip formatter={(v) => fmtCurrency(v)} />}
            />
            <Bar dataKey="outstanding" name="Outstanding" radius={[0, 6, 6, 0]} maxBarSize={22}>
              {loanBreakdown.map((loan, i) => (
                <Cell key={loan.name} fill={COLORS[i % COLORS.length]} />
              ))}
            </Bar>
          </BarChart>
        </ResponsiveContainer>
      </ChartFrame>

      <ul className="mt-3 space-y-1.5 border-t border-border/60 pt-3">
        {loanBreakdown.map((loan) => (
          <li key={loan.name} className="grid grid-cols-[minmax(0,1fr)_auto] items-center gap-3 text-xs">
            <span className="min-w-0 truncate text-muted-foreground">
              {loan.name} · {loan.ratePct}% APR
            </span>
            <span className="shrink-0 tabular-nums">{fmtCurrency(loan.emi)}/mo</span>
          </li>
        ))}
      </ul>
    </PanelCard>
  );
}

import { Cell, Pie, PieChart, ResponsiveContainer } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import {
  equity,
  equityPct,
  fmtINRShort,
  homeLoan,
  propertyDetail,
} from "@/lib/property-data";

const data = [
  { name: "Your equity", value: equity, color: "var(--color-chart-1)" },
  { name: "Loan outstanding", value: homeLoan.outstanding, color: "var(--color-chart-4)" },
];

/** Ownership + equity vs loan split. */
export function EquityCard() {
  return (
    <section className="surface-tile p-4">
      <div className="grid grid-cols-[132px_minmax(0,1fr)] items-center gap-3 sm:grid-cols-[160px_minmax(0,1fr)]">
        <div className="relative">
          <ChartFrame height={140} mobileHeight={132}>
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie
                  data={data}
                  dataKey="value"
                  innerRadius="66%"
                  outerRadius="94%"
                  paddingAngle={2}
                  stroke="none"
                  startAngle={90}
                  endAngle={-270}
                  animationDuration={900}
                >
                  {data.map((slice) => (
                    <Cell key={slice.name} fill={slice.color} />
                  ))}
                </Pie>
              </PieChart>
            </ResponsiveContainer>
          </ChartFrame>
          <div className="pointer-events-none absolute inset-0 grid place-items-center">
            <div className="text-center">
              <p className="font-display text-xl font-semibold tabular-nums">{equityPct}%</p>
              <p className="text-[10px] font-medium text-muted-foreground">owned</p>
            </div>
          </div>
        </div>

        <dl className="min-w-0 space-y-3">
          <div>
            <dt className="flex items-center gap-1.5 text-[11px] text-muted-foreground">
              <span className="size-2 rounded-full bg-[var(--color-chart-1)]" />
              Your equity
            </dt>
            <dd className="mt-0.5 font-display text-base font-semibold tabular-nums">
              {fmtINRShort(equity)}
            </dd>
          </div>
          <div>
            <dt className="flex items-center gap-1.5 text-[11px] text-muted-foreground">
              <span className="size-2 rounded-full bg-[var(--color-chart-4)]" />
              Loan outstanding
            </dt>
            <dd className="mt-0.5 font-display text-base font-semibold tabular-nums">
              {fmtINRShort(homeLoan.outstanding)}
            </dd>
          </div>
        </dl>
      </div>

      <p className="mt-3 border-t border-border/70 pt-3 text-[12px] text-muted-foreground">
        Ownership · <span className="font-medium text-foreground">{propertyDetail.owners}</span> ·{" "}
        {propertyDetail.ownershipPct}% share
      </p>
    </section>
  );
}

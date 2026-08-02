import { Cell, Pie, PieChart, ResponsiveContainer } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import { ChartSkeleton } from "@/components/ui-kit/skeletons";
import { usePrimaryProperty } from "@/hooks/api/use-properties";
import { fmtINRShort } from "@/lib/property-data";

/** Ownership + equity vs loan split. */
export function EquityCard() {
  const { data, isPending, isError, refetch, isFetching } = usePrimaryProperty();

  if (isPending) {
    return <ChartSkeleton height={200} />;
  }

  if (isError || !data) {
    return (
      <section className="surface-tile px-4 py-5 text-center">
        <p className="text-sm font-medium">Unable to load equity</p>
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

  const chartData = [
    { name: "Your equity", value: Math.max(data.equity, 0), color: "var(--color-chart-1)" },
    {
      name: "Loan outstanding",
      value: Math.max(data.loanOutstanding, 0),
      color: "var(--color-chart-4)",
    },
  ].filter((slice) => slice.value > 0);

  const pieData =
    chartData.length > 0
      ? chartData
      : [{ name: "Your equity", value: 1, color: "var(--color-chart-1)" }];

  return (
    <section className="surface-tile p-4">
      <div className="grid grid-cols-[132px_minmax(0,1fr)] items-center gap-3 sm:grid-cols-[160px_minmax(0,1fr)]">
        <div className="relative">
          <ChartFrame height={140} mobileHeight={132}>
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie
                  data={pieData}
                  dataKey="value"
                  innerRadius="66%"
                  outerRadius="94%"
                  paddingAngle={2}
                  stroke="none"
                  startAngle={90}
                  endAngle={-270}
                  animationDuration={900}
                >
                  {pieData.map((slice) => (
                    <Cell key={slice.name} fill={slice.color} />
                  ))}
                </Pie>
              </PieChart>
            </ResponsiveContainer>
          </ChartFrame>
          <div className="pointer-events-none absolute inset-0 grid place-items-center">
            <div className="text-center">
              <p className="font-display text-xl font-semibold tabular-nums">{data.equityPct}%</p>
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
              {fmtINRShort(data.equity)}
            </dd>
          </div>
          <div>
            <dt className="flex items-center gap-1.5 text-[11px] text-muted-foreground">
              <span className="size-2 rounded-full bg-[var(--color-chart-4)]" />
              Loan outstanding
            </dt>
            <dd className="mt-0.5 font-display text-base font-semibold tabular-nums">
              {fmtINRShort(data.loanOutstanding)}
            </dd>
          </div>
        </dl>
      </div>

      <p className="mt-3 border-t border-border/70 pt-3 text-[12px] text-muted-foreground">
        Ownership · <span className="font-medium text-foreground">{data.ownersLabel}</span> ·{" "}
        {data.ownershipPct}% share
      </p>
    </section>
  );
}

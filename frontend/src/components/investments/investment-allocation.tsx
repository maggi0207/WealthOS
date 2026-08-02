import { useEffect, useState } from "react";
import { Cell, Pie, PieChart, ResponsiveContainer } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import { fmtINRShort, investmentAllocation, investmentAllocationTotal } from "@/lib/investments-data";
import { cn } from "@/lib/utils";

/** Animated allocation donut with a tappable legend. */
export function InvestmentAllocationDonut() {
  const [active, setActive] = useState<number | null>(null);
  const [ready, setReady] = useState(false);

  useEffect(() => {
    const id = window.setTimeout(() => setReady(true), 60);
    return () => window.clearTimeout(id);
  }, []);

  const focus = active === null ? null : investmentAllocation[active];

  return (
    <section className="surface-tile p-4">
      <div className="relative">
        <ChartFrame height={210} mobileHeight={186}>
          <ResponsiveContainer width="100%" height="100%">
            <PieChart>
              <Pie
                data={investmentAllocation}
                dataKey="value"
                nameKey="name"
                innerRadius="66%"
                outerRadius={ready ? "94%" : "70%"}
                paddingAngle={2}
                stroke="none"
                isAnimationActive
                animationDuration={900}
                animationEasing="ease-out"
                onClick={(_, index) => setActive((prev) => (prev === index ? null : index))}
              >
                {investmentAllocation.map((slice, index) => (
                  <Cell
                    key={slice.name}
                    fill={slice.color}
                    opacity={active === null || active === index ? 1 : 0.28}
                    className="cursor-pointer transition-opacity duration-300"
                  />
                ))}
              </Pie>
            </PieChart>
          </ResponsiveContainer>
        </ChartFrame>

        <div className="pointer-events-none absolute inset-0 grid place-items-center">
          <div className="text-center">
            <p className="text-[10px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
              {focus ? focus.name : "Portfolio"}
            </p>
            <p className="mt-0.5 font-display text-xl font-semibold tabular-nums sm:text-2xl">
              {fmtINRShort(focus ? focus.value : investmentAllocationTotal)}
            </p>
          </div>
        </div>
      </div>

      <ul className="mt-3 grid gap-1.5">
        {investmentAllocation.map((slice, index) => {
          const pct = (slice.value / investmentAllocationTotal) * 100;
          return (
            <li key={slice.name}>
              <button
                type="button"
                onClick={() => setActive((prev) => (prev === index ? null : index))}
                className={cn(
                  "press grid min-h-11 w-full grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-2.5 rounded-xl px-2 py-2 text-left transition-colors",
                  active === index ? "bg-muted/60" : "hover:bg-muted/35",
                )}
              >
                <span className="size-2.5 shrink-0 rounded-full" style={{ background: slice.color }} />
                <span className="min-w-0 truncate text-[13px] font-medium">{slice.name}</span>
                <span className="shrink-0 text-right text-[13px] font-semibold tabular-nums">
                  {pct.toFixed(0)}%
                  <span className="ml-2 text-[11px] font-medium text-muted-foreground">
                    {fmtINRShort(slice.value)}
                  </span>
                </span>
              </button>
            </li>
          );
        })}
      </ul>
    </section>
  );
}

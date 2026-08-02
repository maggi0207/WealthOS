import { ArrowDownRight, ArrowUpRight } from "lucide-react";
import { Area, AreaChart, ResponsiveContainer } from "recharts";

import { ChartFrame } from "@/components/dashboard/chart-frame";
import { assetCards, fmtINRShort } from "@/lib/wealth-data";
import { cn } from "@/lib/utils";

/** Swipeable, snap-scrolling asset cards — one-hand friendly on phones. */
export function AssetCardRail() {
  return (
    <div className="no-scrollbar bleed-gutter page-gutter flex snap-x snap-mandatory gap-3 overflow-x-auto pb-1 sm:mx-0 sm:grid sm:grid-cols-2 sm:px-0 lg:grid-cols-3">
      {assetCards.map((asset) => {
        const gain = asset.value - asset.invested;
        const gainPct = (gain / asset.invested) * 100;
        const up = gain >= 0;
        const data = asset.spark.map((v, i) => ({ i, v }));

        return (
          <article
            key={asset.id}
            className="surface-tile press w-[76vw] max-w-[300px] shrink-0 snap-start overflow-hidden sm:w-auto sm:max-w-none"
          >
            <div className="px-4 pt-3.5">
              <p className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
                {asset.category}
              </p>
              <h3 className="mt-1 truncate text-[15px] font-semibold">{asset.name}</h3>
              <p className="mt-1.5 font-display text-xl font-semibold tabular-nums">{fmtINRShort(asset.value)}</p>
              <p
                className={cn(
                  "mt-1 inline-flex items-center gap-0.5 text-[12px] font-medium tabular-nums",
                  up ? "text-success" : "text-destructive",
                )}
              >
                {up ? <ArrowUpRight className="size-3.5" /> : <ArrowDownRight className="size-3.5" />}
                {fmtINRShort(Math.abs(gain))} ({Math.abs(gainPct).toFixed(1)}%)
              </p>
            </div>

            <ChartFrame height={56} mobileHeight={48}>
              <ResponsiveContainer width="100%" height="100%">
                <AreaChart data={data} margin={{ top: 6, right: 0, bottom: 0, left: 0 }}>
                  <defs>
                    <linearGradient id={`spark-${asset.id}`} x1="0" y1="0" x2="0" y2="1">
                      <stop
                        offset="0%"
                        stopColor={up ? "var(--color-success)" : "var(--color-destructive)"}
                        stopOpacity={0.4}
                      />
                      <stop
                        offset="100%"
                        stopColor={up ? "var(--color-success)" : "var(--color-destructive)"}
                        stopOpacity={0}
                      />
                    </linearGradient>
                  </defs>
                  <Area
                    type="monotone"
                    dataKey="v"
                    stroke={up ? "var(--color-success)" : "var(--color-destructive)"}
                    strokeWidth={2}
                    fill={`url(#spark-${asset.id})`}
                    isAnimationActive
                    animationDuration={700}
                  />
                </AreaChart>
              </ResponsiveContainer>
            </ChartFrame>
          </article>
        );
      })}
    </div>
  );
}

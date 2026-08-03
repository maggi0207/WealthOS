import type { AllocationSlice } from "@/lib/assets-utils";
import { formatAssetMoney } from "@/lib/assets-utils";
import { cn } from "@/lib/utils";

const BAR_TONES = [
  "bg-primary",
  "bg-chart-2",
  "bg-chart-3",
  "bg-chart-4",
  "bg-chart-5",
  "bg-emerald-500",
  "bg-amber-500",
  "bg-sky-500",
  "bg-rose-500",
];

/** Section 2 — allocation breakdown with progress bars. */
export function AssetsAllocation({
  slices,
  currencyCode,
  loading,
}: {
  slices: AllocationSlice[];
  currencyCode: string;
  loading?: boolean;
}) {
  if (loading) {
    return (
      <div className="surface-tile space-y-3 p-4">
        {Array.from({ length: 4 }).map((_, i) => (
          <div key={i} className="h-10 animate-pulse rounded-lg bg-muted/40" />
        ))}
      </div>
    );
  }

  if (slices.length === 0) {
    return (
      <div className="surface-tile flex min-h-[140px] flex-col items-center justify-center gap-1 p-6 text-center">
        <p className="text-sm font-medium">No allocation yet</p>
        <p className="text-[12px] text-muted-foreground">
          Add properties, investments or manual assets to see your mix.
        </p>
      </div>
    );
  }

  return (
    <div className="surface-tile space-y-3.5 p-4">
      {slices.map((slice, index) => (
        <div key={slice.category} className="space-y-1.5">
          <div className="flex items-baseline justify-between gap-3">
            <p className="text-[13px] font-semibold">{slice.category}</p>
            <div className="text-right">
              <p className="text-[13px] font-semibold tabular-nums">
                {formatAssetMoney(slice.value, currencyCode)}
              </p>
              <p className="text-[11px] tabular-nums text-muted-foreground">
                {slice.percent.toFixed(1)}%
              </p>
            </div>
          </div>
          <div className="h-2 overflow-hidden rounded-full bg-muted/60">
            <div
              className={cn("h-full rounded-full transition-all", BAR_TONES[index % BAR_TONES.length])}
              style={{ width: `${Math.max(slice.percent, 1.5)}%` }}
            />
          </div>
        </div>
      ))}
    </div>
  );
}

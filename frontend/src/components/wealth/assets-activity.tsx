import type { AssetActivity } from "@/lib/assets-utils";
import { formatAssetMoney, formatRelativeDate } from "@/lib/assets-utils";

/** Section 4 — recent asset activity from live timestamps. */
export function AssetsActivity({
  activities,
  currencyCode,
  loading,
}: {
  activities: AssetActivity[];
  currencyCode: string;
  loading?: boolean;
}) {
  if (loading) {
    return (
      <div className="surface-tile space-y-3 p-4">
        {Array.from({ length: 4 }).map((_, i) => (
          <div key={i} className="h-12 animate-pulse rounded-lg bg-muted/40" />
        ))}
      </div>
    );
  }

  if (activities.length === 0) {
    return (
      <div className="surface-tile flex min-h-[140px] flex-col items-center justify-center gap-1 p-6 text-center">
        <p className="text-sm font-medium">No recent activity</p>
        <p className="text-[12px] text-muted-foreground">
          Asset changes will appear here as you add or update holdings.
        </p>
      </div>
    );
  }

  return (
    <ul className="surface-tile divide-y divide-border/60 overflow-hidden">
      {activities.map((item) => (
        <li key={item.id} className="flex items-center justify-between gap-3 px-4 py-3">
          <div className="min-w-0">
            <p className="truncate text-[13px] font-semibold">{item.title}</p>
            <p className="truncate text-[11px] text-muted-foreground">{item.detail}</p>
            <p className="mt-0.5 text-[10px] text-muted-foreground">
              {formatRelativeDate(item.occurredAt)}
            </p>
          </div>
          <p className="shrink-0 text-[13px] font-semibold tabular-nums">
            {formatAssetMoney(item.amount, currencyCode)}
          </p>
        </li>
      ))}
    </ul>
  );
}

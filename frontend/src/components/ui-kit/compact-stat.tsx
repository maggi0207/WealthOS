import type { LucideIcon } from "lucide-react";

import { cn } from "@/lib/utils";

/** Compact metric tile — the replacement for bulky KPI cards. */
export function CompactStat({
  label,
  value,
  delta,
  icon: Icon,
  tone = "neutral",
}: {
  label: string;
  value: string;
  delta?: number;
  icon?: LucideIcon;
  tone?: "positive" | "negative" | "neutral";
}) {
  const up = (delta ?? 0) >= 0;
  const good = tone === "neutral" ? up : tone === "positive" ? up : !up;

  return (
    <div className="surface-tile px-3.5 py-3">
      <div className="flex items-center gap-1.5">
        {Icon && <Icon className="size-3.5 shrink-0 text-muted-foreground" />}
        <p className="truncate text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
          {label}
        </p>
      </div>
      <p className="mt-1.5 truncate font-display text-[1.05rem] font-semibold leading-tight tabular-nums sm:text-xl">
        {value}
      </p>
      {delta !== undefined && (
        <p
          className={cn(
            "mt-0.5 text-[11px] font-medium tabular-nums",
            good ? "text-success" : "text-destructive",
          )}
        >
          {up ? "▲" : "▼"} {Math.abs(delta).toFixed(1)}%
        </p>
      )}
    </div>
  );
}

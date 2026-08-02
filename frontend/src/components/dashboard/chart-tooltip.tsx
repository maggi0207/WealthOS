import type { TooltipProps } from "recharts";

/** Shared tooltip styled with design tokens. */
export function ChartTooltip({
  active,
  payload,
  label,
  formatter,
}: TooltipProps<number, string> & { formatter?: (value: number) => string }) {
  if (!active || !payload?.length) return null;
  const format = formatter ?? ((v: number) => String(v));

  return (
    <div className="min-w-36 rounded-lg border border-border bg-popover/95 px-3 py-2 shadow-elevate backdrop-blur">
      {label !== undefined && (
        <p className="mb-1 text-xs font-medium text-muted-foreground">{String(label)}</p>
      )}
      <div className="space-y-1">
        {payload.map((entry) => (
          <div key={String(entry.dataKey ?? entry.name)} className="flex items-center gap-2 text-xs">
            <span
              className="size-2 shrink-0 rounded-full"
              style={{ backgroundColor: entry.color ?? "var(--color-primary)" }}
            />
            <span className="min-w-0 flex-1 truncate capitalize text-muted-foreground">
              {String(entry.name ?? entry.dataKey)}
            </span>
            <span className="shrink-0 font-medium tabular-nums">{format(Number(entry.value ?? 0))}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

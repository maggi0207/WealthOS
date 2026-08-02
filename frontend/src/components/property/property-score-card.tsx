import { propertyScore } from "@/lib/property-data";

/** Premium property score card with animated sub-score bars. */
export function PropertyScoreCard() {
  return (
    <section className="surface-hero overflow-hidden p-4">
      <div className="flex items-end justify-between gap-3">
        <div className="min-w-0">
          <p className="text-[11px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
            Overall
          </p>

          <p className="mt-1 font-display text-4xl font-semibold leading-none tabular-nums">
            {propertyScore.overall}
            <span className="ml-1 text-base font-medium text-muted-foreground">/10</span>
          </p>
        </div>
        <span className="shrink-0 rounded-full bg-success/12 px-2.5 py-1 text-[11px] font-semibold text-success">
          {propertyScore.grade}
        </span>
      </div>

      <ul className="mt-4 space-y-3">
        {propertyScore.items.map((item) => (
          <li key={item.label}>
            <div className="flex items-baseline justify-between gap-3">
              <span className="truncate text-[13px] font-medium">{item.label}</span>
              <span className="shrink-0 text-[13px] font-semibold tabular-nums">
                {item.score.toFixed(1)}
                <span className="text-[11px] font-medium text-muted-foreground">/10</span>
              </span>
            </div>
            <div className="mt-1.5 h-1.5 w-full overflow-hidden rounded-full bg-muted">
              <div
                className="h-full rounded-full bg-primary transition-[width] duration-700 ease-out"
                style={{ width: `${item.score * 10}%` }}
              />
            </div>
            <p className="mt-1 truncate text-[11px] text-muted-foreground">{item.note}</p>
          </li>
        ))}
      </ul>
    </section>
  );
}

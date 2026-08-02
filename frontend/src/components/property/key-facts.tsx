import { keyFacts } from "@/lib/property-data";

/** Key facts as scrollable chips — full-bleed on mobile so nothing clips. */
export function KeyFacts() {
  return (
    <div className="bleed-gutter no-scrollbar flex snap-x snap-mandatory gap-2 overflow-x-auto px-[var(--page-gutter)] pb-0.5">
      {keyFacts.map((fact) => (
        <div
          key={fact.label}
          className="surface-tile snap-start whitespace-nowrap px-3 py-2"
        >
          <p className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
            {fact.label}
          </p>
          <p className="mt-0.5 text-[13px] font-semibold tabular-nums">{fact.value}</p>
        </div>
      ))}
    </div>
  );
}
